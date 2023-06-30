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
using EasySaveWPF.Models;
using System.Threading;
using EasySaveWPF.ViewModels.Proxys;
using EasySaveWPF.Events;

namespace EasySaveWPF.ViewModels
{
    public class LaunchSavViewModel : Notified, IViewModel
    {
        private BackupModel _backupModel;
        private WorkModel _workModel;
        private TranslationManager _translationManager;
        private ICommand _movetoinitialCommand;
        private ICommand _movetolaunchCommand;
        private Server _server;
        private ICommand _launchBackUpsCommand;


        //Content datagrid
        private BindableCollection<Backup> _initialBackupList;

        private BindableCollection<Backup> _launchBackupList;

        public LaunchSavViewModel()
        {
            this._workModel = ViewModelFactory.workModel;
            this._backupModel = ViewModelFactory.backupModel;
            this.TranslationManager = ViewModelFactory.translationManager;
            this.UpdateList();
			_server = new Server();
            Thread clientThread = new Thread(() =>
            {
                _server.Start(InitialBackupList, this);

            });
            clientThread.Start();	
		}


        //Getters
        public ICommand LaunchBackUpsCommand
        {
            get
            {
                if (_launchBackUpsCommand == null)
                {
                    _launchBackUpsCommand = new RelayCommand(param => InitiateLaunch(param), param => true);
                }
                return _launchBackUpsCommand;
            }
        }

        public ICommand MoveToInitialCommand
        {
            get
            {
                if (_movetoinitialCommand == null)
                {
                    _movetoinitialCommand = new RelayCommand(param => MoveToInitial(param), param => true);
                }
                return _movetoinitialCommand;
            }
        }
        public ICommand MoveToLaunchCommand
        {
            get
            {
                if (_movetolaunchCommand == null)
                {
                    _movetolaunchCommand = new RelayCommand(param => MoveToLaunch(param), param => true);
                }
                return _movetolaunchCommand;
            }
        }
        public BindableCollection<Backup> InitialBackupList
        {
            get { return _initialBackupList; }
            set
            {
                _initialBackupList = value;
                OnPropertyChanged(nameof(InitialBackupList));
            }
        }
        public BindableCollection<Backup> LaunchBackupList
        {
            get { return _launchBackupList; }
            set
            {
                _launchBackupList = value;
                OnPropertyChanged(nameof(LaunchBackupList));
            }
        }
        public WorkModel WorkModel { get => _workModel; set => _workModel = value; }
        public BackupModel BackupModel { get => _backupModel; set => _backupModel = value; }
        public TranslationManager TranslationManager { get => _translationManager; set => _translationManager = value; }

        public void UpdateList()
        {
            this._initialBackupList = new BindableCollection<Backup>(this._backupModel.GetAvailableBackups());
            this._launchBackupList = new BindableCollection<Backup>();
        }

        private void InitiateLaunch(object obj)
        {
            this.WorkModel.GenerateCurrentWorks(this.LaunchBackupList,() => { return new WorkProxy(); });
            Mediator.Notify("LaunchStatusViewModel", "LaunchStatusViewModel");
        }

        private void MoveToInitial(object obj)
        {
            IList<object> selectedItems = obj as IList<object>;
            if (selectedItems != null)
            {
                List<Backup> itemsToRemove = selectedItems.Cast<Backup>().ToList();
                foreach (Backup item in itemsToRemove)
                {
                    this.LaunchBackupList.Remove(item);
                    this.InitialBackupList.Add(item);
                }
            }

            this.InitialBackupList.Refresh();
        }

        private void MoveToLaunch(object obj)
        {
            IList<object> selectedItems = obj as IList<object>;
            if (selectedItems != null)
            {
                List<Backup> itemsToRemove = selectedItems.Cast<Backup>().ToList();
                foreach (Backup item in itemsToRemove)
                {
                    this.InitialBackupList.Remove(item);
                    this.LaunchBackupList.Add(item);
                }
            }

            this.LaunchBackupList.Refresh();
        }


        //Translated text
        public string GridHeader { get { return this.TranslationManager.Translate("headers.launchBackUp"); } }
        public string LaunchCommand { get { return this.TranslationManager.Translate("btns.launch"); } }
        public string BackUpLabel { get { return this._translationManager.Translate("labels.backupLabel"); } }
        public string BackUpTypeLabel { get { return this._translationManager.Translate("labels.backupType"); } }

        public void OnShow()
        {
            this.UpdateList();
        }
    }
}
