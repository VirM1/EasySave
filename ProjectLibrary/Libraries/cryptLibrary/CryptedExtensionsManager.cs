using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using ProjectLibrary.FileLibrary;
using ProjectLibrary.Models;


namespace ProjectLibrary.CryptLibrary
{
    public class CryptedExtensionsManager
    {
        private const string fileName = "crypted.json";

        private HashSet<CryptedFile> _listExtensions;

        private string _filePath;

        private JsonSerializerOptions _serializerOptions;

        public CryptedExtensionsManager(string filePath)
        {
            this._filePath = Path.Combine(filePath, fileName);
            this._serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            this.GetContentOfJsonFile();
        }

        public HashSet<CryptedFile> ListExtentions { get => _listExtensions; set => _listExtensions = value; }


        private void GetContentOfJsonFile()
        {
            if (!File.Exists(this._filePath))
            {
                this._listExtensions = new HashSet<CryptedFile>();
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

        private HashSet<CryptedFile> DeSerializeList()
        {
            return JsonSerializer.Deserialize<HashSet<CryptedFile>>(FileManager.GetStream(this._filePath));
        }

        public void UpdateFile()
        {
            FileManager.WriteAndSaveFile(this._filePath, this.SerializeList());
        }

        public void AddPriority(CryptedFile priority)
        {
            this.ListExtentions.Add(priority);
            this.UpdateFile();
        }

        public void RemovePriority(CryptedFile priority)
        {
            this.ListExtentions.Remove(priority);
            this.UpdateFile();
        }

        public void EditPriority(CryptedFile priority, string newName)
        {
            priority.Name = newName;
            this.UpdateFile();
        }
    }
}
