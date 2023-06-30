using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using ProjectLibrary.FileLibrary;

namespace ProjectLibrary.Models
{

    /* 
     *   Class : BackupModel 
     *   Usage : Class representing a BackupModel, used for the different interactions with the Backup object
     */
    public class BackupModel
    {
        private string backupPath;

        private JsonSerializerOptions serializerOptions;

        /*
         * Constructor : BackupModel Constructor
         * Usage       : Constructor of the BackupModel class, takes as a parameter the the backUp path where we will save all the different backup objects.
         * 
         */
        public BackupModel(string backupPath)
        {
            this.backupPath = backupPath;
            this.serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
        }

        /*
         * Function    : GetAvailableBackups()
         * Usage       : Function that returns a List of all the available and correct BackUps from the json files stored in the backupPath.
         * 
         */
        public List<Backup> GetAvailableBackups()
        {
            List<Backup> backups = new List<Backup>();
            List<string> fileNames = FileManager.GetAvailableFileNamesRegex(this.backupPath, "*.json");
            foreach(string fileName in fileNames)
            {
                backups.Add(FileManager.GetBackup(Path.Combine(this.backupPath, fileName)));
            }

            return backups;
        }


        /*
         * Function    : VerifyLabelDoesntExistAndValid(string label)
         * Usage       : Function that returns null or a string by checkig if label is a valid fileName and also if the label isn't already used.
         * 
         */
        public string? VerifyLabelDoesntExistAndValid(string label)
        {
            return this.TestAndGiveErrorMessage((FileManager.DoFileExist(this.ConvertIntoPath(label)) || FileManager.CheckIfValidFileName(label)), "handleBackup.testsException.fileAlreadyExistsOrInvalid");
        }

        /*
         * Function    : VerifyLabelExists(string label)
         * Usage       : Function that returns null or a string by checkig if label exists and is being used by a save.
         * 
         */
        public string? VerifyLabelExists(string label)
        {
            return this.TestAndGiveErrorMessage(!FileManager.DoFileExist(this.ConvertIntoPath(label)), "handleBackup.testsException.fileDoesntExists");
        }


        /*
         * Function    : VerifyFolderExists(string folder)
         * Usage       : Function that returns null or a string by checkig if the given folder exists
         * 
         */
        public string? VerifyFolderExists(string folder)
        {
            return this.TestAndGiveErrorMessage(!Directory.Exists(folder), "handleBackup.testsException.folderDoesntExists");
        }


        /*
         * Function    : VerifyValidEnum(string checkedEnum)
         * Usage       : Function that returns null or a string by checkig if the given string is a castable as a BackupType Enum.
         * 
         */
        public string? VerifyValidEnum(string checkedEnum)
        {
            return this.TestAndGiveErrorMessage(
                    !Enum.GetValues(typeof(BackupType)).Cast<BackupType>().Select(v => v.ToString().ToLower()).ToList().Contains(checkedEnum),
                    "handleBackup.testsException.wrongEnum"
                );
        }

        /*
         * Method      : CreateBackup(string inputLabel,string source, string destination, BackupType inputEnum)
         * Usage       : Method that creates a Backup from the given parameters and also, registers it as a JSON file.
         * 
         */
        public Backup CreateBackup(string inputLabel,string source, string destination, BackupType inputEnum)
        {
            Backup sauvegarde = new Backup
            {
                Label = inputLabel,
                Source = source,
                Destination = destination,
                Type = inputEnum
            };

            this.RegisterBackup(sauvegarde);

            return sauvegarde;
        }


        /*
         * Method      : EditBackup(Backup backup, string source, string destination, BackupType inputEnum)
         * Usage       : Method that edits the given Backup and saves it into the existing JSON file.
         * 
         */
        public void EditBackup(Backup backup, string source, string destination, BackupType inputEnum)
        {
            backup.Source = source;
            backup.Destination = destination;
            backup.Type = inputEnum;

            this.RegisterBackup(backup);
        }


        /*
         * Method      : DeleteBackup(string label)
         * Usage       : Method that tries to delete the corresponding JSON save file if it exists
         * 
         */
        public void DeleteBackup(string label)
        {
            FileManager.DeleteFile(this.ConvertIntoPath(label));
        }

        /*
         * Function    : GetBackup(string label)
         * Usage       : Function that returns a BackUp from a JSON file (the file being parsed and Serialized into a Backup object).
         * 
         */
        public Backup GetBackup(string label)
        {
            return FileManager.GetBackup(this.ConvertIntoPath(label));
        }


        /*
         * Function    : GetBackups(List<string> labels)
         * Usage       : Returns a list of BackUps from the given List of Labels (used for the grouped launch command)
         * 
         */
        public List<Backup> GetBackups(List<string> labels)
        {
            List<Backup> backUpList = new List<Backup>();
            string message;
            foreach(string label in labels)
            {
                message = null;
                message = VerifyLabelExists(label);
                if (message != null)
                {
                    throw new Exception(message);
                }
                backUpList.Add(FileManager.GetBackup(this.ConvertIntoPath(label)));
            }
            return backUpList;
        }

        /*
         * Method      : RegisterBackup(Backup backup)
         * Usage       : Method that tries to register the given BackUp, which is serialized into a JSON and using the options set in the constructor.
         * 
         */
        private void RegisterBackup(Backup backup)
        {
            FileManager.WriteAndSaveFile(this.ConvertIntoPath(backup.Label), JsonSerializer.Serialize(backup, this.serializerOptions));
        }


        /*
         * Function    : CastIntoBackupType(string inputEnum)
         * Usage       : Casts the given string into a BackUpType Enum
         * 
         */
        public BackupType CastIntoBackupType(string inputEnum)
        {
            return (BackupType)Enum.Parse(typeof(BackupType), inputEnum, true);
        }


        /*
         * Function    : TestAndGiveErrorMessage(bool test,string errorMessage)
         * Usage       : Tests the following test and returns an error message if the test is true.
         * 
         */
        private string? TestAndGiveErrorMessage(bool test,string errorMessage)
        {
            if (test)
            {
                return errorMessage;
            }

            return null;
        }

        /*
         * Function    : ConvertIntoPath(string label)
         * Usage       : Converts the given label into a path leading to the JSON save file
         * 
         */
        private string ConvertIntoPath(string label)
        {
            return Path.Combine(this.backupPath, String.Format("{0}.json", label));
        }
    }
}
