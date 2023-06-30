using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectLibrary.TranslationLibrary;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text.Json;

using ProjectLibrary.TranslationLibrary;
using ProjectLibrary.Models;

using System.Transactions;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using ProjectLibrary.LogLibrary;
using EasySaveWPF.Commands;
using ProjectLibrary.EnvManager;


namespace EasySaveWPF.ViewModels
{
    public class SettingsViewModel : Notified, IViewModel
    {

        private TranslationManager _translationManager;

        private WorkModel _workModel;

        private EnvFileManager _envFileManager;

        private bool _isJsonSelected;

        private bool _isXmlSelected;

        private bool _isFrSelected;

        private bool _isEnSelected;

        private bool _isEsSelected;

        private int _komax;

        private string _cryptoSoftLocation;

        public bool IsJsonSelected
        {
            get { return _isJsonSelected; }
            set
            {
                _isJsonSelected = value;
            }
        }

        public bool IsXmlSelected
        {
            get { return _isXmlSelected; }
            set
            {
                _isXmlSelected = value;
            }
        }

        public bool IsFrSelected
        {
            get { return _isFrSelected; }
            set
            {
                _isFrSelected = value;
            }
        }

        public bool IsEnSelected
        {
            get { return _isEnSelected; }
            set
            {
                _isEnSelected = value;
            }
        }

        public bool IsEsSelected
        {
            get { return _isEsSelected; }
            set
            {
                _isEsSelected = value;
            }
        }


        public SettingsViewModel()
        {
            this._translationManager = ViewModelFactory.translationManager;
            this.WorkModel = ViewModelFactory.workModel;
            this.SwitchTranslationCommand = new SwitchTranslationCommand(this);
            this.SwitchSerializerCommand = new SwitchSerializerCommand(this,ViewModelFactory.envFileManager);
            this._envFileManager = ViewModelFactory.envFileManager;
            this.PropertyChanged += this.UpdateTranslationAllTexts;
            Komax = this._envFileManager.BigFileSize;
            CryptoSoftLocation = this._envFileManager.CryptoSoftLocation;

            if (this.WorkModel.GetExtension().ToString() == "json")
            {
                IsJsonSelected = true;
            }
            if (this.WorkModel.GetExtension().ToString() == "xml")
            {
                IsXmlSelected = true;
            }
            if (this._translationManager.GetLanguage() == "fr")
            {
                IsFrSelected = true;
            }
            if (this._translationManager.GetLanguage() == "en")
            {
                IsEnSelected = true;
            }
            if (this._translationManager.GetLanguage() == "es")
            {
                IsEsSelected = true;
            }
        }

        public ICommand SwitchSerializerCommand { get; set; }

        public ICommand SwitchTranslationCommand { get; set; }

        public TranslationManager TranslationManager { get => _translationManager; set => _translationManager = value; }

        public string FrenchButton
        {
            get { return _translationManager.Translate("menuLang.fr"); }
        }

        public string EnglishButton
        {
            get { return _translationManager.Translate("menuLang.en"); }
        }

        public string SpanishButton
        {
            get { return _translationManager.Translate("menuLang.es"); }
        }

        public string Languagetitle
        {
            get { return _translationManager.Translate("menuLang.languagetitle"); }
        }

        public string Logtitle
        {
            get { return _translationManager.Translate("logging.logtitle"); }
        }

        public string Komaxtitle
        {
            get { return _translationManager.Translate("Komax.Komaxtitle"); }
        }


        public string CryptoSoftTitle
        {
            get { return _translationManager.Translate("cryptoSoft.title"); }
        }

        public int Komax {
            get { return _komax; }
            set
            {
                _komax = value;
                this._envFileManager.BigFileSize = value;
                this._envFileManager.updateOrCreateEnvFile();
            }
        }

        public string CryptoSoftLocation
        {
            get { return _cryptoSoftLocation; }
            set
            {
                _cryptoSoftLocation = value;
                OnPropertyChanged(nameof(CryptoSoftLocation));
                this._envFileManager.CryptoSoftLocation = value;
                this._envFileManager.updateOrCreateEnvFile();
            }
        }

        public string GridHeader { get { return this._translationManager.Translate("headers.settings"); } }


        public WorkModel WorkModel { get => _workModel; set => _workModel = value; }

        public void ChangeLanguage(string locale)
        {
            this._translationManager.ChangeTranslation(locale);
            this._envFileManager.BaseLocale = locale;
            this.OnPropertyChanged("ALLTEXTS");
            this._envFileManager.updateOrCreateEnvFile();
        }
        
        private void UpdateTranslationAllTexts(object sender, PropertyChangedEventArgs args)
        {
           

			if (args.PropertyName == "ALLTEXTS")
            {
                foreach(PropertyInfo propertyInfo in this.GetType().GetProperties())
                {
                    if(propertyInfo.PropertyType == typeof(string))
                    {
                        this.OnPropertyChanged(propertyInfo.Name);
                    }
                }
				UpdateMainWinTrad();
			}
        }

        public void UpdateMainWinTrad()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            // Modifier la propriété Title de la fenêtre principale
            //mainWindow.Title = "Nouveau titre de la fenêtre principale";
            Button LaunchSav = mainWindow.FindName("launchSav_btn") as Button;
            Button BannedFiles = mainWindow.FindName("bannedFiles_btn") as Button;
            Button Settings = mainWindow.FindName("settings_btn") as Button;
            Button BackUpManagement = mainWindow.FindName("savManagement_btn") as Button;
            Button LaunchStatus = mainWindow.FindName("launchStatus_btn") as Button;
            Button Priority = mainWindow.FindName("Priority_btn") as Button;
            Button Crypted = mainWindow.FindName("bannedFiles_btn_Copy") as Button;

            LaunchSav.Content = _translationManager.Translate("btns.launchBackUp");
            BannedFiles.Content = _translationManager.Translate("btns.bannedApps");
            Settings.Content = _translationManager.Translate("btns.settings");
            BackUpManagement.Content = _translationManager.Translate("btns.backupManagement");
            LaunchStatus.Content = _translationManager.Translate("btns.launchStatus");
            Priority.Content = _translationManager.Translate("btns.priority");
            Crypted.Content = _translationManager.Translate("btns.crypted");
        }
        public void OnShow()
        {
        }

        private ICommand _cryptoFileCommand;

        public ICommand CryptoFileCommand
        {
            get
            {
                if (_cryptoFileCommand == null)
                {
                    _cryptoFileCommand = new RelayCommand(param => ChooseFile(param), param => true);
                }
                return _cryptoFileCommand;
            }
        }

        private void ChooseFile(object obj)
        {
            this.CryptoSoftLocation = this.openExplorer();
        }

        private string openExplorer()
        {
            string strFiltre = "Tous les fichiers (.exe)|*.exe";
            var OFD = new System.Windows.Forms.OpenFileDialog();
            OFD.Filter = strFiltre;
            OFD.FilterIndex = 1;
            OFD.RestoreDirectory = true;


            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return OFD.FileName;
            }
            else
            {
                return "";
            }
        }
    }
}
