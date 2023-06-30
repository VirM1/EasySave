using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectLibrary.Models;
using ProjectLibrary.FileLibrary;

namespace ProjectLibrary.LogLibrary.Serializers
{
    class JsonLogSerializer : ISerializer
    {
        private LogExtension _logExtension;

        private JsonSerializerOptions _serializerOptions;

        public LogExtension LogExtension
        {
            get => _logExtension;
            set => _logExtension = value;
        }

        public JsonLogSerializer()
        {
            this._logExtension = LogExtension.json;
            this._serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters =
                {
                            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }


        public string SerializeLogList(LogList logList)
        {
            return JsonSerializer.Serialize(logList, this._serializerOptions);
        }


        public string SerializeWorkList(SerializableDictionary<string, List<Work>> workList)
        {
            return JsonSerializer.Serialize(workList, this._serializerOptions);
        }

        public LogList GetLogList(string fullPath)
        {
            return JsonSerializer.Deserialize<LogList>(FileManager.GetStream(fullPath));
        }

        public SerializableDictionary<string, List<Work>> GetWorkList(string fullPath)
        {
            return JsonSerializer.Deserialize<SerializableDictionary<string, List<Work>>>(FileManager.GetStream(fullPath));
        }
    }
}
