using System.Diagnostics;
using System.IO;
using System;
using ProjectLibrary.Models;

namespace ProjectLibrary.CryptLibrary
{
    class CryptFilesManager
    {
        private int count = 0;

        private Process process;

        private Log log;

        private FileInfo fileInfo;

        private DateTime start;

        private WorkContent workContent;

        private WorkModel.DelVoidWork delVoidWork;

        private delCallbackOnceFinished delCallbackOnceFinishedFunc;

        public delegate void delCallbackOnceFinished(Log logFile, FileInfo fileInfo, DateTime start, WorkContent workContent, WorkModel.DelVoidWork delVoidWork);

        public Process Process { get => process; set => process = value; }

        public void LaunchProgram(string fileName, delCallbackOnceFinished callbackFinished, Log logFile, FileInfo fileInfo, DateTime start, WorkContent workContent, WorkModel.DelVoidWork delVoidWork, string argument = "")
        {
            if (File.Exists(fileName))
            {
                this.log = logFile;
                this.fileInfo = fileInfo;
                this.start = start;
                this.workContent = workContent;
                this.delVoidWork = delVoidWork;
                this.delCallbackOnceFinishedFunc = callbackFinished;
                process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = argument;
                process.StartInfo.CreateNoWindow = true;
                process.EnableRaisingEvents = true;
                process.Exited += callback;
                process.Start();
                process.WaitForExit();
            }
        }

        public void callback(object send, EventArgs args)
        {
            this.log.EncryptionTime = (DateTime.Now.Subtract(start)).TotalSeconds;
            File.SetLastWriteTime(this.workContent.Work.CurrentDestinationPath, DateTime.Now);
            this.delCallbackOnceFinishedFunc.Invoke(this.log, this.fileInfo, this.start, this.workContent, this.delVoidWork);
        }

    }
}
