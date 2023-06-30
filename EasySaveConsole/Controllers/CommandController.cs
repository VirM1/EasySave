using System;
using System.Collections.Generic;
using ProjectLibrary.Models;
using ProjectLibrary.LogLibrary;

namespace EasySaveConsole.Controllers
{

    /* 
     *   Class : CommandController 
     *   Usage : Class representing a command controller, the controller responsible for all the user inputs in the command prompts
     */
    class CommandController : AbstractController
    {

        private WorkModel workModel;

        private BackupModel backupModel;


        /*
         * Constructor : CommandController Constructor
         * Usage       : Constructor of the CommandController class, sets up the two needed models, as well as the view and initiate a commandProcessor
         * 
         */
        public CommandController(View view, WorkModel workModel,BackupModel backupModel) : base(view)
        {
            this.workModel = workModel;
            this.backupModel = backupModel;
            this.view.SetCommandController(this);
        }

        /*
         * Method      : LaunchBackup(List<string> listLabels, bool threaded)
         * Usage       : Initiate the backups given with the list 
         * 
         */
        public void LaunchBackup(List<string> listLabels, bool threaded)
        {
            string message;
            this.view.ClearConsole();
            try { 
                List<Backup> backups = this.backupModel.GetBackups(listLabels);

                this.view.InitiateProgressBar();
                this.workModel.GenerateCurrentWorks(backups,()=> { return new Work(); });
                //this.workModel.LaunchGroupedThreaded(this.view.UpdateProgress);
                message = "success";
            }
            catch (Exception e)
            {
                //todo @Virgile Handle exceptions
                message = e.ToString();
                message = this.ExceptionHandler(message);
            }
            this.view.WriteTemporaryMessageAndGoMenu(message, 5000);
        }



        /*
         * Function    : ExceptionHandler(string message)
         * Usage       : Handle the exceptions given from the WorkModel.
         * 
         */
        public string ExceptionHandler(string message)
        {
            if (message.Contains("Could not find file"))
            {
                message = "fichier non existant";
            }
            else
            {
                if (message.Contains("Source error"))
                {
                    message = "la source n'existe plus";
                }
                else
                {
                    if (message.Contains("Destination error"))
                    {
                        message = "la destination n'existe plus";
                    }
                    else
                    {
                        message = "erreur";
                    }
                }
            }
            return message;
        }


        public string? DoesExtensionExists(string extension)
        {
            return Enum.IsDefined(typeof(LogExtension), extension) ? null : "Extension undefined";
        }

        public LogExtension GetExtension()
        {
            return this.workModel.GetExtension();
        }

        public void SetExtension(LogExtension extension)
        {
            this.workModel.SetExtension(extension);
        }
    }
}
