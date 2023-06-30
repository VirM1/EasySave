using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Collections.Concurrent;
using ProjectLibrary.FileLibrary;
using ProjectLibrary.Models;
using ProjectLibrary.LogLibrary.Serializers;

namespace ProjectLibrary.LogLibrary
{


    /* 
     *   Class : LogManager 
     *   Usage : Class representing a LogManager, used for to manage the logging System 
     */
    class LogManager
    {
        private const string WorkFileName = "works";

        private string logPath;

        private string workPath;

        private string logFileName;

        private ISerializer _serializer;

        private ISerializer _workSerializer;

        private LogList _logList;

        private string _currentTimeStamp;

        private SerializableDictionary<string, List<Work>>  _workList;

        internal ISerializer Serializer { get => _serializer; set => _serializer = value; }

        /*
         * Constructor : LogManager Constructor
         * Usage       : Constructor of the LogManager class, takes as a parameter the the work path where we will save all the work file, as well as the logPath where we will save the dailylogs.
         * 
         */
        public LogManager(string workPath,string logPath,LogExtension logExtension)
        {
            this.workPath = workPath;
            this.logPath = logPath;
            this._serializer = SerializerFactory.GetSerializer(logExtension);
            this._workSerializer = SerializerFactory.GetSerializer(LogExtension.json);
            this.SetAndCreateLogFile();
            this.SetAndCreateWorkFile();
            this._logList = GetLogList();
            this._workList = GetWorkList();
        }

        /*
         * 
         * Method : ChangeSerializer(LogExtension logExtension)
         * Usage  : Changes the Serializer used by the LogManager (with a default value as Json, if nothing is found)
         * 
         */
        public void ChangeSerializer(LogExtension logExtension)
        {
            this._serializer = SerializerFactory.GetSerializer(logExtension);
        }

        /*
         * Method      : InitiateLogging()
         * Usage       : Initiates the logging, by setting empty fileQueues, tries to set and/or create the log files, after that we create the ThreadHandler (referencing the fileQueues of the LogManager) and in the end we create the Threads and launch them.
         * 
         */
        public void InitiateLogging()
        {
            this.SetAndCreateLogFile();
            this.SetAndCreateWorkFile();
            this._currentTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            _workList.Add(this._currentTimeStamp, new List<Work>());
        }

        /*
         * Method      : SetAndCreateLogFile()
         * Usage       : Sets and create the dailylogFie.
         * 
         */
        private void SetAndCreateLogFile()
        {
            this.logFileName = String.Format("{0}-dailylog.{1}", DateTime.Now.ToString("yyyyMMdd"),this._serializer.LogExtension.ToString());
            string fullPath = Path.Combine(this.logPath, this.logFileName);
            if (!File.Exists(fullPath))
            {
                FileManager.WriteAndSaveFile(fullPath, this.InitiateLogFile());
            }
        }


        /*
         * Method      : SetAndCreateLogFile()
         * Usage       : Sets and create the workFile.
         * 
         */
        private void SetAndCreateWorkFile()
        {
            string fullPath = Path.Combine(this.workPath, String.Format("{0}.{1}", WorkFileName, this._workSerializer.LogExtension.ToString()));
            if (!File.Exists(fullPath))
            {
                FileManager.WriteAndSaveFile(fullPath, this.InitiateWorkFile());
            }
        }

        /*
         * Function    : InitiateWorkFile()
         * Usage       : Initiate the work File content with a Dictionnary serialized into a JSON.
         * 
         */
        private string InitiateWorkFile()
        {
            SerializableDictionary<string, List<Work>> workDictionnary = new SerializableDictionary<string, List<Work>>();
            return this._workSerializer.SerializeWorkList(workDictionnary);
        }

        /*
         * Function    : InitiateLogFile()
         * Usage       : Initiate the log file with a LogList object serialized into a JSON.
         * 
         */
        private string InitiateLogFile()
        {
            LogList logList = new LogList();
            return this._serializer.SerializeLogList(logList);
        }

        /*
         * Method      : AppendLogToLogList(Log log)
         * Usage       : Method that adds a Log Object to the Log queue (which is iterated by the logging thread)
         * 
         */
        public void AppendLogToLogList(Log log)
        {
            this._logList.AddLog(log);
            FileManager.WriteAndSaveFile(Path.Combine(this.logPath, this.logFileName), this._serializer.SerializeLogList(this._logList));
        }



        /*
         * Method      : AppendWorkToWorkList(Work work)
         * Usage       : Method that adds a Work Object to the Work queue (which is iterated by the loggingWork thread)
         * 
         */
        public void AppendWorkToWorkList(Work work)
        {
            int index = _workList[this._currentTimeStamp].FindIndex(x => x.Backup.Label == work.Backup.Label);
            if (index == -1)
            {
                _workList[this._currentTimeStamp].Add(work);
            }
            else
            {
                _workList[this._currentTimeStamp][index] = work;//todo @Virgile, replace or update? do comparaison
            }
            FileManager.WriteAndSaveFile(Path.Combine(this.workPath, String.Format("{0}.{1}", WorkFileName, LogExtension.json.ToString())), this._workSerializer.SerializeWorkList(this._workList));
        }

        public SerializableDictionary<string, List<Work>> GetWorkList()
        {
            return this._workSerializer.GetWorkList(Path.Combine(this.workPath, String.Format("{0}.{1}",WorkFileName ,LogExtension.json.ToString())));
        }

        private LogList GetLogList()
        {
            return this._serializer.GetLogList(Path.Combine(this.logPath, this.logFileName));
        }
    }
}
