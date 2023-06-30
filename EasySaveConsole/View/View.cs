using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Text;
using ProjectLibrary.TranslationLibrary;
using ProjectLibrary.Models;
using EasySaveConsole.Controllers;
using System.Transactions;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using ProjectLibrary.LogLibrary;

namespace EasySaveConsole
{
    /*
     *  Class : View
     *  Usage : Class representing the View of the MVC architecture, this class manage the console interface
     */
    class View
    {

        // Attribute initialization

        private string input;
        private List<ViewOption> viewOptions;
        private List<ViewOption> oldMenu;

        private TranslationManager translationManager;

        private MenuController menuController;

        private CommandController commandController;

        private string fileCountTranslation;

        private string fileSizeTranslation;

        private string statusTranslation;

        private string labelTranslation;

        private int cursorPos = 0;

        /*
         *  Constructor : View Constructor
         *  Usage       : Constructor of the View class, create objectes for print the menu on the CLI
         */
        public View(TranslationManager translationManager)
        {
            this.translationManager = translationManager;

            this.GenerateBaseMenu();
        }

        /*
         * Methode       : InitView() 
         * Usage         : Method manage the menu arrow animation and key action.
         */

        public void InitView()
        {
            // Set the default index of the selected item to be the first
            int index = 0;

            // Write the menu out
            this.WriteMenu(viewOptions, viewOptions[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < viewOptions.Count)
                    {
                        index++;
                        this.WriteMenu(viewOptions, viewOptions[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        this.WriteMenu(viewOptions, viewOptions[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    viewOptions[index].SelectedAction.Invoke();
                    index = 0;
                }
                if (keyinfo.Key == ConsoleKey.Escape)
                {

                    SwitchMenu("MenuBase");
                    index = 0;
                }
                this.cursorPos = index;
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();
        }

        /*
         * Methode       : SetMenuController(MenuController menuController) 
         * Usage         : Methode that sets the property menuController
         */

        public void SetMenuController(MenuController menuController)
        {
            this.menuController = menuController;
        }

        /*
         * Methode       : SetCommandController(CommandController commandController)
         * Usage         : Methode that sets the property commandController
         */

        public void SetCommandController(CommandController commandController)
        {
            this.commandController = commandController;
        }

        /*
         * Methode       : WriteMenu(List<ViewOption> options, ViewOption selectedOption)
         * Usage         : Print in the CLI sur EZSAVE logo
         *                 manage cursor animation in the menu
         */

        private void WriteMenu(List<ViewOption> options, ViewOption selectedOption)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" _______  ________      _______.     ___   ____    ____  _______ \n|   ____||       /     /       |    /   \\  \\   \\  /   / |   ____|\n|  |__   `---/  /     |   (----`   /  ^  \\  \\   \\/   /  |  |__   \n|   __|     /  /       \\   \\      /  /_\\  \\  \\      /   |   __|  \n|  |____   /  /----.----)   |    /  _____  \\  \\    /    |  |____ \n|_______| /________|_______/    /__/     \\__\\  \\__/     |_______|\n                                     ");
            foreach (ViewOption option in options)
            {
                Console.ForegroundColor = option.Selected ? ConsoleColor.Green : ConsoleColor.White;
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }
                Console.WriteLine(option.Name);
            }
        }

        /*
         * Methode       : ask(string msg)
         * Usage         : Methode that ask a question and read the answer
         */

        private string Ask(string msg)
        {
            Console.Clear();
            Console.Write(msg);
            return Console.ReadLine() ?? string.Empty;
        }

        /*
         * Methode       : WriteTemporaryMessage(string message, int sleepTime = 1000)
         * Usage         : Methode that print a temporaly message for informe the user
         */

        public void WriteTemporaryMessage(string message, int sleepTime = 1000)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(sleepTime);
            this.WriteMenu(viewOptions, viewOptions[0]);
        }

        /*
         * Methode       : WriteTemporaryMessageAndGoMenu(string message, int sleepTime = 1000)
         * Usage         : Methode that print a temporaly message for informe the user
         */

        public void WriteTemporaryMessageAndGoMenu(string message, int sleepTime = 1000)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(sleepTime);
            this.SwitchMenu("MenuLaunchBackUp");
        }

        /*
         * Methode       : SwitchMenu(string newMenu)
         * Usage         : Methode that change the submenu in fonction of the user choise
         */

        //submen
        private void SwitchMenu(string newMenu)
        {
            this.oldMenu = this.viewOptions;

            switch (newMenu)
            {
                case "MenuLang":
                    this.GenerateLanguageSubMenu();
                    break;
                case "MenuBackupIndiv":
                    this.GenerateBackupSubMenu();
                    break;
                case "MenuListBackup":
                    this.GenerateListBackupSubMenu();
                    break;
                case "MenuListBackupUpdate":
                    this.GenerateListBackupEditMenu();
                    break;
                case "MenuBase":
                    this.GenerateBaseMenu();
                    break;
                case "MenuLaunchBackUp":
                    this.GenerateLaunchSaveMenu();
                    break;
            }
            
            Console.Clear();
            Thread.Sleep(300);
            this.WriteMenu(viewOptions, viewOptions[0]);
        }

        /*
         * Methode       : GenerateListBackupSubMenu()
         * Usage         : Method that generates a list with the backup information that is label, source, destination and type.
         */

        private void GenerateListBackupSubMenu()
        {
            List<Backup> backups = this.menuController.GetAvailableBackups();
            this.viewOptions = new List<ViewOption>();
            this.viewOptions.Add(new ViewOption(this.translationManager.Translate("menu.retour"), "menu.retour", () => this.backOldMenu()));
            foreach (Backup backup in backups)
            {
                this.viewOptions.Add(
                    new ViewOption(
                        backup.Label, 
                        () => this.WriteTemporaryMessage(this.translationManager.Translate("listBackup.showBackup",
                            backup.Label,
                            backup.Source,
                            backup.Destination,
                            backup.Type.ToString()
                            ),2000)
                        )
                    );
            }

        }

        /*
         * Methode       : GenerateListBackupEditMenu()
         * Usage         : Method that generates a list for modify backup information that is source, destination and type.
         */

        private void GenerateListBackupEditMenu()
        {
            List<Backup> backups = this.menuController.GetAvailableBackups();
            this.viewOptions = new List<ViewOption>();
            this.viewOptions.Add(new ViewOption(this.translationManager.Translate("menu.retour"), "menu.retour", () => this.SwitchMenu("MenuBackupIndiv")));
            foreach (Backup backup in backups)
            {
                this.viewOptions.Add(
                    new ViewOption(
                        backup.Label,
                        () => EditBackup(backup.Label)
                        )
                    );
            }
        }

        private void GenerateLaunchSaveMenu()
        {
            List<Backup> backups = this.menuController.GetAvailableBackups();
            this.viewOptions = new List<ViewOption>();
            this.viewOptions.Add(new ViewOption(this.translationManager.Translate("menu.retour"), "menu.retour", () => this.SwitchMenu("MenuBase")));
            this.viewOptions.Add(new ViewOption(this.translationManager.Translate("menu.launchBackUp"), "menu.launchBackUp", () => this.LaunchSave()));
            foreach(Backup backup in backups)
            {
                ViewOption selectableViewOption = new ViewOption(backup.Label, () => {  });
                selectableViewOption.SelectedAction = () => { 
                    selectableViewOption.Selected = !selectableViewOption.Selected;
                    this.WriteMenu(viewOptions, viewOptions[this.cursorPos]);
                };
                this.viewOptions.Add(selectableViewOption);
            }
        }

        private void LaunchSave()
        {
            List<string> labels = new List<string>();
            foreach(ViewOption viewOption in this.viewOptions.FindAll(x => x.Selected))
            {
                labels.Add(viewOption.Name);
            }
            this.commandController.LaunchBackup(labels, false);
        }

        /*
         * Methode       : GenerateLanguageSubMenu()
         * Usage         : Method that generates the language submenu and change the language.
         */

        private void GenerateLanguageSubMenu()
        {
            List<string> translations = this.translationManager.GetAvailableTranslations();
            this.viewOptions = new List<ViewOption>();
            string key;
            foreach(string translation in translations)
            {
                key = String.Format("menuLang.{0}", translation);
                this.viewOptions.Add(new ViewOption(this.translationManager.Translate(key), key, () => this.ChangeLanguage(translation)));
            }
            this.viewOptions.Add(new ViewOption(this.translationManager.Translate("menu.retour"), "menu.retour", () => this.backOldMenu()));
        }

        /*
         * Methode       : GenerateBackupSubMenu()
         * Usage         : Method that generates the backup submenu.
         */

        private void GenerateBackupSubMenu()
        {
            this.viewOptions = new List<ViewOption>
            {
                new ViewOption(this.translationManager.Translate("menuBackup.createBackup"),"menuBackup.createBackup", () => this.CreateBackup()),
                new ViewOption(this.translationManager.Translate("menuBackup.editBackup"),"menuBackup.editBackup", () => SwitchMenu("MenuListBackupUpdate")),
                new ViewOption(this.translationManager.Translate("menuBackup.deleteBackup"),"menuBackup.deleteBackup", () => this.DeleteBackup()),
                new ViewOption(this.translationManager.Translate("menu.retour"), "menu.retour", () => SwitchMenu("MenuBase")),
            };
        }

        /*
         * Methode       : GenerateBaseMenu()
         * Usage         : Methode that generate the base menu.
         */

        private void GenerateBaseMenu()
        {
            this.viewOptions = new List<ViewOption>
            {
                new ViewOption(translationManager.Translate("menu.backup"),"menu.backup", () => this.SwitchMenu("MenuBackupIndiv")),
                new ViewOption(translationManager.Translate("menu.listBackup"),"menu.listBackup", () =>  SwitchMenu("MenuListBackup")),
                new ViewOption(translationManager.Translate("menu.launchBackUp"),"menu.launchBackUp", () => SwitchMenu("MenuLaunchBackUp")),
                new ViewOption(translationManager.Translate("menu.languages"),"menu.languages", () =>  this.SwitchMenu("MenuLang")),
                new ViewOption(translationManager.Translate("menu.modeLog"),"menu.modeLog", () =>  this.ChangeModeLog()),
                new ViewOption(translationManager.Translate("menu.exit"),"menu.exit", () => Environment.Exit(0)),
            };
        }

        /*
         * Methode       : ChangeLanguage(string locale)
         * Usage         : Methode that change the language. For this, the methode update the menu and print a message for informe the user.
         */

        private void ChangeLanguage(string locale)
        {
            Console.Clear();
            this.translationManager.ChangeTranslation(locale);
            UpdateMenus();
            WriteTemporaryMessage(this.translationManager.Translate("menuLang.msgSucces"));

        }

        /*
         * Methode       : UpdateMenus()
         * Usage         : Methode that update the menu. This methode is used for the language change.
         */

        private void UpdateMenus()
        {
            foreach (ViewOption option in this.viewOptions)
            {
                option.Name = this.translationManager.Translate(option.Key);
            }
        }

        /*
         * Methode       : BackOldMenu()
         * Usage         : Methode that change the menu by the previous menu. It's used for the back action.
         */

        private void backOldMenu()
        {
            Console.Clear();
            this.viewOptions = this.oldMenu;
            UpdateMenus();
            Thread.Sleep(300);
            this.WriteMenu(viewOptions, viewOptions[0]);
        }


        private void ChangeModeLog()
        {
            Console.Clear();
            string message;
            try {
                string extension = this.commandController.GetExtension().ToString();
                extension = WriteAndCheck(this.translationManager.Translate("logging.changeMode", extension), () => this.commandController.DoesExtensionExists(this.input), extension);
                this.commandController.SetExtension((LogExtension)Enum.Parse(typeof(LogExtension), extension, true));
                message = "success";
            }
            catch (Exception e){
                message = "failure";
            }
            this.WriteTemporaryMessage(message,2000);
        }


        /*
         * Methode       : CreateBackup()
         * Usage         : Methode that create the backup with the information that the user had previously give.
         */
        private void CreateBackup()
        {
            Console.Clear();
            try
            {
                string label = WriteAndCheck(this.translationManager.Translate("handleBackup.inputLabel", ""), () => this.menuController.CheckElementExists(this.input, "labelnotexist"));
                
                string folderSource = WriteAndCheck(this.translationManager.Translate("handleBackup.inputSourceFolder", ""), () => this.menuController.CheckElementExists(this.input, "foldernotexist"));
               
                string folderDestination = WriteAndCheck(this.translationManager.Translate("handleBackup.inputDestinationFolder", ""), () => this.menuController.CheckElementExists(this.input, "foldernotexist"));
                
                string backupType = WriteAndCheck(this.translationManager.Translate("handleBackup.inputBackupType", ""), () => this.menuController.CheckElementExists(this.input, "enumvalid"));
                
                this.InvokeActionAndHandleExceptions(() => this.menuController.CreateBackup(label, folderSource, folderDestination, backupType), "handleBackup.fileStatus.backupd", "handleBackup.fileStatus.backupException");
            }
            catch (Exception e){
                return;
            } 
        }

        /*
         * Methode       : EditBackup(string label)
         * Usage         : Methode that change the information of a backup if the user want.
         */
        private void EditBackup(string label)
        {
            Console.Clear();
            //string label = writeAndCheck(this.translationManager.Translate("handleBackup.inputLabelSearch", ""), () => this.menuController.checkElementExists(this.input, "labelexist"));
            try
            {
                Backup backup = this.menuController.GetBackup(label);

                string folderSource = WriteAndCheck(this.translationManager.Translate("handleBackup.inputSourceFolder", String.Format("({0})", backup.Source)), () => this.menuController.CheckElementExists(this.input, "foldernotexist"), backup.Source);

                string folderDestination = WriteAndCheck(this.translationManager.Translate("handleBackup.inputDestinationFolder", String.Format("({0})", backup.Destination)), () => this.menuController.CheckElementExists(this.input, "foldernotexist"), backup.Destination);

                string backupType = WriteAndCheck(this.translationManager.Translate("handleBackup.inputBackupType", String.Format("({0})", backup.Type.ToString())), () => this.menuController.CheckElementExists(this.input, "enumvalid"), backup.Type.ToString());
                this.InvokeActionAndHandleExceptions(() => this.menuController.EditBackup(folderSource, folderDestination, backupType, backup), "handleBackup.fileStatus.backupd", "handleBackup.fileStatus.backupException");
            }
            catch (Exception e)
            {
                return;
            }
        }

        /*
         * Methode       : DeleteBackup()
         * Usage         : Methode that delete backup due to the label of the backup
         */
        private void DeleteBackup()
        {
            Console.Clear();
            try
            {
                string label = WriteAndCheck(this.translationManager.Translate("handleBackup.inputLabel", ""), () => menuController.CheckElementExists(this.input, "labelexist"));
                this.InvokeActionAndHandleExceptions(() => this.menuController.DeleteBackup(label), "handleBackup.fileStatus.deleted", "handleBackup.fileStatus.deleteException");
            }
            catch (Exception e){
                return;
            } 
        }

        /*
         * Methode       : InvokeActionAndHandleExceptions(Action action,string success,string exception)
         * Usage         : Methode that manage the success and exception message of the console menu
         */
        private void InvokeActionAndHandleExceptions(Action action,string success,string exception)
        {
            try
            {
                action.Invoke();
                WriteTemporaryMessage(this.translationManager.Translate(success));
            }
            catch (Exception e)//todo @Virgile handle Exceptions
            {
                WriteTemporaryMessage(this.translationManager.Translate(exception));
            }
        }

        /*
         * Methode       : WriteAndCheck(string shownQuestion, Func<string> verificationAction = null, string? baseValue = null)
         * Usage         : Methode that manage the question answer. It's used for the backup functionality.
         */
        private string WriteAndCheck(string shownQuestion, Func<string> verificationAction = null, string? baseValue = null)
        {
            bool valid = false;
            string errorMessage;
            do
            {

                this.input = Ask(shownQuestion);

                if (this.input.Length != 0)
                {
                    if(verificationAction != null)
                    {
                        if (this.input == "exit")
                        {
                            this.SwitchMenu("MenuBase");
                            throw new Exception();
                            //return input;s
                        }
                        errorMessage = verificationAction.Invoke();
                        if (errorMessage != null)
                        {
                            WriteTemporaryMessage(this.translationManager.Translate(errorMessage, this.input));
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                    else
                    {
                        valid = true;
                    }
                }
                else
                {
                    if (baseValue != null)
                    {
                        valid = true;
                        this.input = baseValue;
                    }
                }
            } while (!valid);
            return this.input;
        }


        /*
         * Methode       : AddProgress(string value,string translation,int positionView)
         * Usage         : Methode that manage ths progression
         */

        private void AddProgress(string value,string translation,int positionView)
        {
            Console.SetCursorPosition(0, positionView);
            Console.Write(new String(' ', 80));
            Console.SetCursorPosition(0, positionView);
            Console.Write(translation + value);
        }

        /*
         * Methode       : UpdateProgress(Work work)
         * Usage         : Methode that manage and write the progress of the sauvegarde
         */

        public void UpdateProgress(Work work)
        {
            int positionView = work.Position;

            this.AddProgress(work.Backup.Label,this.labelTranslation, positionView * 5);
            this.AddProgress(work.Status.ToString(), this.statusTranslation, positionView * 5 + 1);
            this.AddProgress(work.RemainingFileCount.ToString(), this.fileCountTranslation, positionView * 5 + 2);
            this.AddProgress(work.RemainingFileSize.ToString(), this.fileSizeTranslation, positionView * 5 + 3);

            Console.SetCursorPosition(0, positionView * 5 + 4);
            Console.Write("=========================================");

        }

        /*
         * Methode       : InitiateProgressBar()
         * Usage         : Methode that initiate the progress bar
         */

        public void InitiateProgressBar()
        {
            this.fileCountTranslation = this.translationManager.Translate("console.progress.remainingFiles");
            this.fileSizeTranslation = this.translationManager.Translate("console.progress.remainingSize");
            this.labelTranslation = this.translationManager.Translate("console.progress.save");
            this.statusTranslation = this.translationManager.Translate("console.progress.status");
        }

        /*
         * Methode       : ClearConsole()
         * Usage         : Methode that clear the CLI
         */

        public void ClearConsole()
        {
            Console.Clear();
        }
    }
}