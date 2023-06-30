using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using ProjectLibrary.FileLibrary;
using ProjectLibrary.LogLibrary;
using ProjectLibrary.ForbiddenFilesLibrary;
using ProjectLibrary.PriorityLibrary;
using ProjectLibrary.EnvManager;
using ProjectLibrary.CryptLibrary;


namespace ProjectLibrary.Models
{
    /* 
     *   Class : WorkModel 
     *   Usage : Class representing a WorkModel, used for the different interactions with the Work object and also the save job
     */
    public class WorkModel
    {

        private LogManager logManager;

        private List<WorkContent> _currentWorks; 

        private delegate (List<string>, List<string>) DelEligibleFiles(string source, string dest, List<string> listPriority);

        public delegate void DelVoid();

        public delegate void DelVoidWork(Work work);

        public delegate bool DelVoidError(string key, params string[] args);

        private ForbiddenFilesManager forbiddenFilesManager;

        private PriorityManager priorityManager;

        private AutoResetEvent waitGlobalHandle;// for ForbiddenApps

        private AutoResetEvent waitBigFilesHandle;

        private Barrier barrierPriorityFiles;

        private AutoResetEvent waitLogHandle;

        private AutoResetEvent waitWorkHandle;

        private AutoResetEvent waitBindingHandle;

        private List<string> priorityExtensions;

        private EnvFileManager envManager;

        private CryptedExtensionsManager cryptedExtensionsManager;

        private List<string> cryptedExtensions;

        /*
         * Constructor : WorkModel Constructor
         * Usage       : Constructor of the WorkModel class, takes as a parameter the the work path where we will save all the work file, as well as the logPath where we will save the dailylogs
         *               With these parameters we initiate a LogManager that will manage the logging threads.
         * 
         */
        public WorkModel(ForbiddenFilesManager forbiddenFilesManager, PriorityManager priorityManager, EnvFileManager envFileManager, CryptedExtensionsManager cryptedExtensionsManager)
        {
            this.logManager = new LogManager(envFileManager.WorksLocation, envFileManager.LogLocation, envFileManager.LogExtension);
            this.forbiddenFilesManager = forbiddenFilesManager;
            this.priorityManager = priorityManager;
            this._currentWorks = new List<WorkContent>();
            this.envManager = envFileManager;
            this.cryptedExtensionsManager = cryptedExtensionsManager;
        }

        public List<WorkContent> CurrentWorks { get => _currentWorks; set => _currentWorks = value; }

        public void GenerateCurrentWorks(IEnumerable<Backup> backups,Func<Work> delCreateWork)
        {
            WorkStatus[] states = { WorkStatus.Waiting, WorkStatus.Error,WorkStatus.Interrupted, WorkStatus.Paused,WorkStatus.End };
            if (this.CheckIfLaunchWithStates(states)) {
                this._currentWorks = new List<WorkContent>();
                foreach(Backup backup in backups)
                {
                    Work work = delCreateWork.Invoke();

                    work.Backup = backup;
                    work.InitialFileCount = 0;
                    work.RemainingFileCount = 0;
                    work.InitialFileSize = 0;
                    work.RemainingFileSize = 0;
                    work.CurrentDestinationPath = "";
                    work.CurrentSourcePath = "";
                    work.Status = WorkStatus.Waiting;
                    this._currentWorks.Add(new WorkContent(work));
                }
            }
        }


        /*
         * Method      : LaunchWork(Backup backup,int positionView)
         * Usage       : Method that launches a backUp work with the given backup (if the source and destination directory exists) and also the given position on the View (mainly used for the multithreading WIP solution)
         *               Also sets up the delegate that will determinate the eligible files (from the given backUp mode, currently differential or complete)
         * 
         */
        public void LaunchWork(WorkContent workContent, DelVoidWork delVoidWork,DelVoidError delVoidError)
        {
            if (!Directory.Exists(workContent.Work.Backup.Source))
            {
                //todo : return and print msg box
                throw new Exception("Source error");
            }
            if (!Directory.Exists(workContent.Work.Backup.Destination))
            {
                //todo : return and print msg box
                throw new Exception("Destination error");
            }
            DelEligibleFiles delEligibleFiles = this.DeterminateDelegateEligible(workContent.Work);

            this.StartCopyingFiles(delEligibleFiles, workContent, delVoidWork, delVoidError);
        }



        /*
         * Method      : LaunchGroupedThreaded(List<Backup> backups)
         * Usage       : Method that launches a group of backUps in a multithread mode, we initate the logging and workLogging threads, create the different Threads that will run separatly the backUp works
         *               and once it's done we stop the logging and workLogging threads
         */
        public void LaunchGroupedThreaded(DelVoidWork delUpdateWork, DelVoidError delVoidError)
        {//@Virgile how will we handle it ?
            this.logManager.InitiateLogging();

            this.barrierPriorityFiles = new Barrier(_currentWorks.Count);
            this.waitGlobalHandle = new AutoResetEvent(true);
            this.waitLogHandle = new AutoResetEvent(true);
            this.waitWorkHandle = new AutoResetEvent(true);
            this.waitBindingHandle = new AutoResetEvent(true);
            this.waitBigFilesHandle = new AutoResetEvent(true);

            this.priorityExtensions = this.GetListOfPriorityExtensions();
            this.cryptedExtensions = this.GetListOfCryptedExtensions();
            foreach (WorkContent workContent in _currentWorks)
            {
                this.ThreadHandler(workContent, delUpdateWork,delVoidError);
            }

            try
            {

                foreach (WorkContent workContent in _currentWorks)
                {
                    workContent.Thread.Start();
                }

                /*
                foreach (WorkContent workContent in _currentWorks)
                {
                    workContent.Thread.Join();
                }*/
            }
            catch (Exception e)
            {

                foreach (WorkContent workContent in _currentWorks)
                {
                    workContent.Thread.Interrupt();
                    workContent.Thread.Join();
                }
            }
        }


        private List<string> GetListOfPriorityExtensions()
        {
            List<string> listPriority = new List<string>();

            foreach(Priority priority in this.priorityManager.ListExtentions)
            {
                listPriority.Add(priority.Name);
            }

            return listPriority;
        }


        private List<string> GetListOfCryptedExtensions()
        {
            List<string> listCrypted = new List<string>();

            foreach(CryptedFile cryptedExtension in this.cryptedExtensionsManager.ListExtentions)
            {
                listCrypted.Add(cryptedExtension.Name);
            }

            return listCrypted;
        }

        /*
         * Method    : ThreadHandler(Backup backup, int positionView)
         * Usage       : Function that returns a Thread using the ThreadHandler.Run Method which, in term will launch a backUp work and print it out on the view with the positionView
         * 
         */
        private void ThreadHandler(WorkContent workContent, DelVoidWork delVoidWork,DelVoidError delVoidError)
        {
            workContent.ThreadWork = new ThreadWork(this, workContent, delVoidWork,delVoidError);
            workContent.Thread = new Thread(workContent.ThreadWork.Run);
        }

        /*
         * Method      : StartCopyingFiles(DelEligibleFiles delEligibleFiles, Backup backup,int positionView)
         * Usage       : Method that starts copying the files, it follows this process : 
         *                  -determinates the files that have to be copied using the delegate   
         *                  -starts a new work
         *                  -then foreach file from the list :
         *                      -we get the fileInfos of the file
         *                      -we create the Log
         *                      -we try to copy the file while registering the transfer time:
         *                          -if successful we log the unmodified log object (serialized and saved)
         *                          -if not, we set the transfertime to -1 and log the object
         *                      -we update the work object then log it
         *                      -once all files have been treated, we log the work with the End status
         */
        private void StartCopyingFiles(DelEligibleFiles delEligibleFiles,WorkContent workContent, DelVoidWork delVoidWork, DelVoidError delVoidError)
        {
            //todo : catch me
            long totalSize = 0;
            List<string> filenamesListPriority;
            List<string> filenamesListNonPriority;
            (filenamesListPriority, filenamesListNonPriority) = delEligibleFiles.Invoke(workContent.Work.Backup.Source, workContent.Work.Backup.Destination,this.priorityExtensions);
            int index = 0;
            foreach (string filename in filenamesListPriority)
            { 
                totalSize += (new FileInfo(Path.Combine(workContent.Work.Backup.Source, filename))).Length;
                index++;
            }

            foreach (string filename in filenamesListNonPriority)
            {
                totalSize += (new FileInfo(Path.Combine(workContent.Work.Backup.Source, filename))).Length;
                index++;
            }


            workContent.Work.InitialFileCount = index;
            workContent.Work.RemainingFileCount = index;
            workContent.Work.InitialFileSize = totalSize;
            workContent.Work.RemainingFileSize = totalSize;
            workContent.Work.Status = WorkStatus.Active;

            //todo update HERE
            this.UpdateView(workContent, delVoidWork);

            try
            {
                InitiateWork(filenamesListPriority, workContent, delVoidWork,delVoidError);

                //todo : WAIT FOR THE REST
                workContent.Work.FinishedPriority = true;
                this.barrierPriorityFiles.SignalAndWait();

                InitiateWork(filenamesListNonPriority, workContent, delVoidWork, delVoidError);

                workContent.Work.Status = WorkStatus.End;

                //ACTION ---- LOG WORK
                this.UpdateWorkList(workContent.Work);

                this.UpdateView(workContent, delVoidWork);
            }
            catch (Exception e)
            {
                workContent.Work.Status = WorkStatus.Interrupted;
                this.UpdateWorkList(workContent.Work);
                this.UpdateView(workContent, delVoidWork);
            }
        }

        public void ChangeStatusAndUpdateView(WorkContent workContent, WorkStatus wishedStatus)
        {

            WorkStatus[] states = { WorkStatus.Error,WorkStatus.Interrupted,WorkStatus.End };
            if (!this.CheckIfLaunchWithStates(states))
            {
                workContent.Work.Status = wishedStatus;
                this.UpdateView(workContent, workContent.ThreadWork.DelVoidWorkUpdate);
            }
        }

        public void UpdateView(WorkContent workContent,DelVoidWork delVoidWork)
        {
            waitBindingHandle.WaitOne();
            delVoidWork.Invoke(workContent.Work);
            waitBindingHandle.Set();
        }

        public void UpdateWorkList(Work work)
        {
            this.waitWorkHandle.WaitOne();
            this.logManager.AppendWorkToWorkList(work);
            this.waitWorkHandle.Set();
        }

        private void UpdateLogList(Log log)
        {
            this.waitLogHandle.WaitOne();
            this.logManager.AppendLogToLogList(log);
            this.waitLogHandle.Set();
        }

        private void CheckForPause(WorkContent workContent)
        {
            workContent.IndividualWaitHandle.WaitOne();
        }

        private void checkForForbidden(DelVoidError delVoidError)
        {
            waitGlobalHandle.WaitOne();
            NoForbiddenAppsRunning(delVoidError);
        }

        private void InitiateWork(List<string> filenamesList,WorkContent workContent, DelVoidWork delVoidWork, DelVoidError delVoidError)
        {
            string sourcePath;
            Log logFile;
            string destinationPath;
            foreach (string filename in filenamesList)
            {

                this.CheckForPause(workContent);
                this.checkForForbidden(delVoidError);
                this.checkForForbidden(delVoidError);

                sourcePath = Path.Combine(workContent.Work.Backup.Source, filename);
                destinationPath = Path.Combine(workContent.Work.Backup.Destination, filename);
                workContent.Work.CurrentSourcePath = sourcePath;
                workContent.Work.CurrentDestinationPath = destinationPath;
                FileInfo fileInfo = new FileInfo(sourcePath);
                DateTime start = DateTime.Now;
                logFile = new Log()
                {
                    FileSize = fileInfo.Length,
                    StartDate = start,
                    Name = workContent.Work.Backup.Label,
                    FileSource = sourcePath,
                    FileDestination = destinationPath,
                };
                try
                {
                    this.CopyFiles(fileInfo, logFile,workContent, start, sourcePath, destinationPath,delVoidWork);
                }
                catch (Exception e)
                {
                    logFile.TransferTime = -1;
                    UpdateFilesAndView(logFile, delVoidWork, workContent);
                    throw e;
                }
            }
        }

        private void UpdateFilesAndView(Log logFile, DelVoidWork delVoidWork, WorkContent workContent)
        {

            //ACTION ---- LOG FILE TRANSFER
            this.UpdateLogList(logFile);

            //ACTION ---- LOG WORK
            this.UpdateWorkList(workContent.Work);
            this.UpdateView(workContent, delVoidWork);
        }


        /*
         * Function    : DeterminateDelegateEligible(Backup backup)
         * Usage       : Function that returns a delegate that we will use to determinate the eligible files.
         * 
         */
        private DelEligibleFiles DeterminateDelegateEligible(Work work)
        {
            DelEligibleFiles delEligibleFiles;

            if (work.Backup.Type.Equals(BackupType.Differential))
            {
                delEligibleFiles = FileManager.GetDifferentialFiles;
            }
            else
            {
                delEligibleFiles = FileManager.GetCompleteFiles;
            }

            return delEligibleFiles;
        }


        public LogExtension GetExtension()
        {
            return this.logManager.Serializer.LogExtension;
        }

        public void SetExtension(LogExtension extension)
        {
            this.logManager.ChangeSerializer(extension);
        }

        public void PauseWorkContent(WorkContent workContent)
        {

            workContent.IndividualWaitHandle.Reset();

            if (!workContent.Work.FinishedPriority)
            {
                this.barrierPriorityFiles.RemoveParticipant();
                workContent.Work.FinishedPriority = true;
            }

            this.ChangeStatusAndUpdateView(workContent, WorkStatus.Paused);
        }


        public void ResumeWorkContent(WorkContent workContent)
        {
            workContent.IndividualWaitHandle.Set();
            this.ChangeStatusAndUpdateView(workContent, WorkStatus.Active);
        }

        public void InterruptWorkContent(WorkContent workContent)
        {
            workContent.Thread.Interrupt();
            if (!workContent.Work.FinishedPriority)
            {
                this.barrierPriorityFiles.RemoveParticipant();
                workContent.Work.FinishedPriority = true;
            }
            this.ChangeStatusAndUpdateView(workContent, WorkStatus.Interrupted);
        }


        public void NoForbiddenAppsRunning(DelVoidError delVoidError)
        {
            bool appFound = false;
            Process[] processlist = Process.GetProcesses();
            string foundForbiddenApps = "";

            foreach(ForbiddenApp forbiddenApp in this.forbiddenFilesManager.ListApps)
            {
                Process[] processes = Process.GetProcessesByName(forbiddenApp.Name);
                if(processes.Count() > 0)
                {
                    foundForbiddenApps += String.Format(" {0}", forbiddenApp.Name);
                    appFound = true;
                }
            }
            if(appFound)
            {
                if (delVoidError.Invoke("exception.forbiddenAppsRunning", foundForbiddenApps))
                {
                    this.NoForbiddenAppsRunning(delVoidError);
                }
                else
                {
                    this.cancelAllWorks();
                }
            }
            else
            {
                waitGlobalHandle.Set();
            }

        }

        private void cancelAllWorks()
        {
            foreach(WorkContent workContent in CurrentWorks)
            {
                workContent.Thread.Interrupt();
            }
        }

        public bool CheckIfLaunchWithStates(WorkStatus[] states)
        {
            foreach (WorkContent workContent in CurrentWorks)
            {
                if (!states.Contains(workContent.Work.Status))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIfCryptable(string extension)
        {
            return this.cryptedExtensions.Contains(extension);
        }

        private void CopyFiles(FileInfo fileInfo, Log logFile, WorkContent workContent, DateTime start, string sourcePath, string destinationPath, DelVoidWork delVoidWork)
        {

            if (fileInfo.Length >= this.envManager.BigFileSize)
            {
                waitBigFilesHandle.WaitOne();
            }

            if (CheckIfCryptable(fileInfo.Extension))
            {
                var pr = new CryptFilesManager();
                pr.LaunchProgram(this.envManager.CryptoSoftLocation, this.callbackOnceFinished, logFile, fileInfo, start, workContent, delVoidWork, String.Format("\"{0}\" \"{1}\"", sourcePath, destinationPath));
            }
            else
            {
                FileManager.CopyFile(sourcePath, destinationPath);
                callbackOnceFinished(logFile, fileInfo,start, workContent, delVoidWork);
            }
        }

        private void callbackOnceFinished(Log logFile,FileInfo fileInfo, DateTime start, WorkContent workContent, DelVoidWork delVoidWork)
        {
            logFile.TransferTime = (DateTime.Now.Subtract(start)).TotalSeconds;

            if (fileInfo.Length >= this.envManager.BigFileSize)
            {
                waitBigFilesHandle.Set();
            }

            workContent.Work.RemainingFileCount--;
            workContent.Work.RemainingFileSize -= fileInfo.Length;

            UpdateFilesAndView(logFile, delVoidWork, workContent);
        }
    }
}
