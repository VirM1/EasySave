using System;
using System.IO;
using ProjectLibrary.EnvManager;
using ProjectLibrary.TranslationLibrary;
using ProjectLibrary.Models;
using EasySaveConsole.Controllers;
using ProjectLibrary.ForbiddenFilesLibrary;
using ProjectLibrary.PriorityLibrary;
using ProjectLibrary.CryptLibrary;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using System.Xml.Serialization;

namespace EasySaveConsole
{
    //https://www.tutorialsteacher.com/csharp/csharp-event

    class Program
    {
        static void Main(string[] args)
        {     
            string envDir = Environment.CurrentDirectory;

            EnvFileManager envFileManager = new EnvFileManager(envDir);

            TranslationManager translationManager = new TranslationManager(
                envFileManager.TranslationLocation,
                envFileManager.BaseLocale
            );

            ForbiddenFilesManager forbiddenFilesManager = new ForbiddenFilesManager(envFileManager.ForbiddenFilesLocation);
            BackupModel backupModel = new BackupModel(envFileManager.BackupLocation);

            PriorityManager priorityManager = new PriorityManager(envFileManager.PriorityLocation);

            CryptedExtensionsManager cryptedExtensionsManager = new CryptedExtensionsManager(envFileManager.CryptedExtensionsLocation);

            WorkModel workModel = new WorkModel(forbiddenFilesManager, priorityManager,envFileManager, cryptedExtensionsManager);

            View view = new View(translationManager);

            MenuController menuController = new MenuController(view,backupModel);

            CommandController commandController = new CommandController(view, workModel,backupModel);

            view.InitView();
        }
    }
}
