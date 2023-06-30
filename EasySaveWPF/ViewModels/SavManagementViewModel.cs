using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Caliburn.Micro;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasySaveWPF.Commands;
using ProjectLibrary.Models;
using ProjectLibrary.TranslationLibrary;
using Microsoft.Win32;
using System.Reflection.PortableExecutable;
using System.Windows.Forms;

namespace EasySaveWPF.ViewModels
{
	public class SavManagementViewModel : Notified, IViewModel
    {


        //Forms private
        private string _label;
        private string _source;
        private string _destination;
        private BackupType _backupType;

        //Content datagrid
        private BindableCollection<Backup> _backUpList;

        //Models
        private TranslationManager _translationManager;
        private BackupModel _backupModel;

        public SavManagementViewModel()
        {
            this._translationManager = ViewModelFactory.translationManager;
            this._backupModel = ViewModelFactory.backupModel;
            this._backUpList = new BindableCollection<Backup>(this._backupModel.GetAvailableBackups());
            this.BackUpList.CollectionChanged += BackUpListChanged;
            this.AddBackupCommand = new AddBackUpCommand(this);
        }

        //Commands
        public ICommand AddBackupCommand { get; set; }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(param => DeleteBackUps(param), param => true);
                }
                return _deleteCommand;
            }
        }



        private ICommand _fileCommand;

        public ICommand FileCommand
        {
            get
            {
                if (_fileCommand == null)
                {
                    _fileCommand = new RelayCommand(param => ChooseFile(param), param => true);
                }
                return _fileCommand;
            }
        }



        //Forms public
        public BackupType BackupType { get => _backupType; set => _backupType = value; }
        public string Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                OnPropertyChanged("Destination");
            }
        }
        public string Source
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged("Source");
            }
        }
        public string Label { get => _label; set => _label = value ; }


        //getters
        public BackupModel BackupModel { get => _backupModel; set => _backupModel = value; }
        public TranslationManager TranslationManager { get => _translationManager; set => _translationManager = value; }
        public BindableCollection<Backup> BackUpList { get => _backUpList; set => _backUpList = value; }

        //Translated text
        public string GridHeader { get { return this._translationManager.Translate("headers.backupManagement"); } }

        public string SaveButton { get { return this._translationManager.Translate("btns.save"); } }

        public string DeleteButton { get { return this._translationManager.Translate("btns.delete"); } }

        public string BackUpLabel { get { return this._translationManager.Translate("labels.backupLabel"); } }
        public string SourceLabel { get { return this._translationManager.Translate("labels.backupSource"); } }
        public string DestinationLabel { get { return this._translationManager.Translate("labels.backupDestination"); } }
        public string BackUpTypeLabel { get { return this._translationManager.Translate("labels.backupType"); } }


        //Collection changed
        private void BackUpListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Backup item in e.OldItems)
                {
                    this.BackupModel.DeleteBackup(item.Label);
                }
            }
        }

        public void update(Backup update)
        {
            this._label = update.Label;
        }

        public void DeleteBackUps(object obj)
        {
            IList<object> selectedItems = obj as IList<object>;
            if (selectedItems != null)
            {
                List<Backup> itemsToRemove = selectedItems.Cast<Backup>().ToList();
                foreach(Backup item in itemsToRemove)
                {
                    this.BackUpList.Remove(item);
                }
            }

            this.BackUpList.Refresh();
        }

        private void ChooseFile(object obj)
        {
            string returned = this.openExplorer();

            if (obj.ToString() == "src")
            {
                this.Source = returned;
            }
            else if (obj.ToString() == "dest")
            {
                this.Destination = returned;
            }
        }



        private string openExplorer()
        {
            string selectedFolder = "";
			
			
			var OFD = new FolderBrowserDialog();
			OFD.ShowNewFolderButton = true;
			// Show the FolderBrowserDialog.  
			DialogResult result = OFD.ShowDialog();
			if (result == DialogResult.OK)
			{
				selectedFolder = OFD.SelectedPath;
				Environment.SpecialFolder root = OFD.RootFolder;
			}
			return selectedFolder ?? "";
        }

        public void OnShow()
        {

        }
    }
}
