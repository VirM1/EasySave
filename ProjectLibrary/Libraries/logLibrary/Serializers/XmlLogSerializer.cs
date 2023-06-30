using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ProjectLibrary.Models;
using ProjectLibrary.FileLibrary;
using System.Xml.Serialization;

namespace ProjectLibrary.LogLibrary.Serializers
{
    class XmlLogSerializer : ISerializer
    {
        private LogExtension _logExtension;

        private XmlSerializer _workListserializer;

        private XmlSerializer _logListSerializer;

        public LogExtension LogExtension
        {
            get => _logExtension;
            set => _logExtension = value;
        }

        public XmlLogSerializer()
        {
            this._logExtension = LogExtension.xml;
            this._workListserializer = new XmlSerializer(typeof(SerializableDictionary<string, List<Work>>));
            this._logListSerializer = new XmlSerializer(typeof(LogList));
        }

        public string SerializeLogList(LogList logList)
        {
            StringWriter outStream = new StringWriter();
            _logListSerializer.Serialize(outStream, logList);
            return outStream.ToString();
        }

        public string SerializeWorkList(SerializableDictionary<string, List<Work>> dictionary)
        {
            StringWriter outStream = new StringWriter();
            _workListserializer.Serialize(outStream, dictionary);
            return outStream.ToString();
        }

        public LogList GetLogList(string fullPath)
        {
            LogList logList;
            string stream = FileManager.GetStream(fullPath);
            using TextReader streamReader = new StringReader(stream);
            logList = (LogList)_logListSerializer.Deserialize(streamReader);
            return logList;
        }

        public SerializableDictionary<string, List<Work>> GetWorkList(string fullPath)
        {

            SerializableDictionary<string, List<Work>> workList;
            string stream = FileManager.GetStream(fullPath);
            using TextReader streamReader = new StringReader(stream);
            workList = (SerializableDictionary<string, List<Work>>)_workListserializer.Deserialize(streamReader);
            return workList;
        }
    }
}