using System.Collections.Generic;
using ProjectLibrary.Models;

namespace EasySaveConsole.Controllers
{

    /* 
     *   Class : MenuController 
     *   Usage : Class representing the menu controller, the controller responsible for all the user inputs in the menu, backUP creation, edit and deletion and submenus
     */
    class MenuController : AbstractController
    {

        private BackupModel backupModel;


        /*
         * Constructor : MenuController Constructor
         * Usage       : Constructor of the MenuController class, sets up the needed model as well as the view
         * 
         */
        public MenuController(View view,BackupModel backupModel): base(view)
        {
            this.backupModel = backupModel;
            this.view.SetMenuController(this);
        }

        /*
         * Function    : GetAvailableBackups()
         * Usage       : Calls the model and returns the available backups.
         * 
         */
        public List<Backup> GetAvailableBackups()
        {
            return this.backupModel.GetAvailableBackups();
        }

        /*
         * Function    : CastIntoBackupType(string inputEnum)
         * Usage       : Calls the model and a BackUpType casted from the inputEnum.
         * 
         */
        public BackupType CastIntoBackupType(string inputEnum)
        {
            return this.backupModel.CastIntoBackupType(inputEnum);
        }

        /*
         * Method      : CreateBackup(string inputLabel, string inputSource,string inputDestination,string inputEnum)
         * Usage       : Calls the model and creates+saves a backUp object
         * 
         */
        public void CreateBackup(string inputLabel, string inputSource,string inputDestination,string inputEnum)
        {
            this.backupModel.CreateBackup(inputLabel, inputSource,inputDestination,this.backupModel.CastIntoBackupType(inputEnum));
        }

        /*
         * Method      : EditBackup(string inputSource, string inputDestination, string inputEnum, Backup backup)
         * Usage       : Calls the model and edits+saves a backUp object
         * 
         */
        public void EditBackup(string inputSource, string inputDestination, string inputEnum, Backup backup)
        {
            this.backupModel.EditBackup(backup, inputSource, inputDestination, this.backupModel.CastIntoBackupType(inputEnum));
        }

        /*
         * Function    : GetBackup(string label)
         * Usage       : Calls the model and returns a backUp object.
         * 
         */
        public Backup GetBackup(string label)
        {
            return this.backupModel.GetBackup(label);
        }

        /*
         * Method      : DeleteBackup(string label)
         * Usage       : Calls the model and deletes a backUp object.
         * 
         */
        public void DeleteBackup(string label)
        {
            this.backupModel.DeleteBackup(label);
        }

        /*
         * Function    : CheckElementExists(string input,string mode)
         * Usage       : Calls the needed test to check if the input is valid following the given mode.
         * 
         */
        public string? CheckElementExists(string input,string mode)
        {
            string? returned = null;
            returned = mode switch
            {
                "labelnotexist" => this.backupModel.VerifyLabelDoesntExistAndValid(input),
                "labelexist" => this.backupModel.VerifyLabelExists(input),
                "foldernotexist" => this.backupModel.VerifyFolderExists(input),
                "enumvalid" => this.backupModel.VerifyValidEnum(input),
                _ => "exception.invalidMode",
            };
            return returned; 
            
        }
    }
}
