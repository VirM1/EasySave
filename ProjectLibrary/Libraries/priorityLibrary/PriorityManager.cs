using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using ProjectLibrary.FileLibrary;
using ProjectLibrary.Models;

namespace ProjectLibrary.PriorityLibrary
{
    public class PriorityManager
    {
        private const string fileName = "priority.json";

        private HashSet<Priority> _listExtensions;

        private string _filePath;

        private JsonSerializerOptions _serializerOptions;

        public PriorityManager(string filePath)
        {
            this._filePath = Path.Combine(filePath, fileName);
            this._serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            this.GetContentOfJsonFile();
        }

        public HashSet<Priority> ListExtentions { get => _listExtensions; set => _listExtensions = value; }


        private void GetContentOfJsonFile()
        {
            if (!File.Exists(this._filePath))
            {
                this._listExtensions = new HashSet<Priority>();
                FileManager.WriteAndSaveFile(this._filePath, this.SerializeList());
            }
            else
            {
                this._listExtensions = this.DeSerializeList();
            }
        }

        private string SerializeList()
        {
            return JsonSerializer.Serialize(this._listExtensions, this._serializerOptions);
        }

        private HashSet<Priority> DeSerializeList()
        {
            return JsonSerializer.Deserialize<HashSet<Priority>>(FileManager.GetStream(this._filePath));
        }

        public void UpdateFile()
        {
            FileManager.WriteAndSaveFile(this._filePath, this.SerializeList());
        }

        public void AddPriority(Priority priority)
        {
            this.ListExtentions.Add(priority);
            this.UpdateFile();
        }

        public void RemovePriority(Priority priority)
        {
            this.ListExtentions.Remove(priority);
            this.UpdateFile();
        }

        public void EditPriority(Priority priority, string newName)
        {
            priority.Name = newName;
            this.UpdateFile();
        }
    }
}
