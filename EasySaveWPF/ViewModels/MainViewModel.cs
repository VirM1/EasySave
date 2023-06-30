using EasySaveWPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProjectLibrary.TranslationLibrary;
using EasySaveWPF.Events;

namespace EasySaveWPF.ViewModels
{
    public class MainViewModel : Notified
    {
        private IViewModel _selectedViewModel;

        private TranslationManager _translationManager;

        public IViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        public MainViewModel()
        {
            _translationManager = ViewModelFactory.translationManager;
            this.InitiateMediatorsSubscriptions();
            this.SwitchPage("SavManagementViewModel");
        }


        private ICommand _switchCommand;

        public ICommand SwitchCommand
        {
            get
            {
                if (_switchCommand == null)
                {
                    _switchCommand = new RelayCommand(param => SwitchPage(param), param => true);
                }
                return _switchCommand;
            }
        }

        private void SwitchPage(object param)
        {
            string castedParam = param as String;

            if (castedParam != null)
            {
                Mediator.Notify(castedParam, castedParam);
            }
        }


        //TRANSLATIONS PART
        public string BannedFiles { get { return this._translationManager.Translate("btns.bannedApps"); } }
        public string Settings { get { return this._translationManager.Translate("btns.settings"); } }
        public string BackUpManagement { get { return this._translationManager.Translate("btns.backupManagement"); } }
        public string LaunchBackups { get { return this._translationManager.Translate("btns.launchBackUp"); } }
        public string LaunchStatus { get { return this._translationManager.Translate("btns.launchStatus"); } }
        public string Priority { get { return this._translationManager.Translate("btns.priority"); } }
        public string Crypted { get { return this._translationManager.Translate("btns.crypted"); } }


        public string Instance { get { return this._translationManager.Translate("instance.instanceAlreadyRunning"); } }


        //NAVIGATION PART

        private void OnGoViewModel(object obj)
        {
            string castedParam = obj as String;
            if (castedParam != null)
            {
                var viewM = ViewModelFactory.GetViewModel(obj);
                this.SelectedViewModel = viewM;
                viewM.OnShow();
            }
        }

        private void InitiateMediatorsSubscriptions()
        {
            foreach(IViewModel viewModel in ViewModelFactory.ViewModels)
            {
                Mediator.Subscribe(viewModel.GetType().Name, OnGoViewModel);
            }
        }
    }
}
