using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using ProjectLibrary.FileLibrary;
using ProjectLibrary.Models;

namespace ProjectLibrary.ForbiddenFilesLibrary
{
    public class ForbiddenFilesManager
    {
        private const string fileName = "forbiddenFiles.json";

        private HashSet<ForbiddenApp> _listFiles;

        private string _filePath;

        private JsonSerializerOptions _serializerOptions;

        public ForbiddenFilesManager(string filePath)
        {
            this._filePath = Path.Combine(filePath, fileName);
            this._serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            this.GetContentOfJsonFile();
        }

        public HashSet<ForbiddenApp> ListApps { get => _listFiles; set => _listFiles = value; }
    

        private void GetContentOfJsonFile()
        {
            if (!File.Exists(this._filePath))
            {
                this._listFiles = new HashSet<ForbiddenApp>();
                FileManager.WriteAndSaveFile(this._filePath, this.SerializeList());
            }
            else
            {
                this._listFiles = this.DeSerializeList();
            }
        }

        private string SerializeList()
        {
            return JsonSerializer.Serialize(this._listFiles, this._serializerOptions);
        }

        private HashSet<ForbiddenApp> DeSerializeList()
        {
            return JsonSerializer.Deserialize<HashSet<ForbiddenApp>>(FileManager.GetStream(this._filePath));
        }

        public void UpdateFile()
        {
            FileManager.WriteAndSaveFile(this._filePath, this.SerializeList());
        }

        public void AddForbiddenFile(ForbiddenApp forbiddenApp)
        {
            this.ListApps.Add(forbiddenApp);
            this.UpdateFile();
        }

        public void RemoveForbiddenFile(ForbiddenApp forbiddenApp)
        {
            this.ListApps.Remove(forbiddenApp);
            this.UpdateFile();
        }

        public void EditForbiddenFile(ForbiddenApp forbiddenApp, string newName)
        {
            forbiddenApp.Name = newName;
            this.UpdateFile();
        }
    }
}
