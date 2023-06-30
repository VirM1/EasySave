using System.Text;
using System.Linq;
using Caliburn.Micro;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using EasySaveWPF.Commands;
using ProjectLibrary.Models;
using ProjectLibrary.TranslationLibrary;
using ProjectLibrary.ForbiddenFilesLibrary;
using EasySaveWPF.ViewModels.Proxys;
using System.Windows.Forms;

namespace EasySaveWPF.ViewModels
{
    class LaunchStatusViewModel : Notified,IViewModel
    {

        private ObservableCollection<WorkContent> _workContents;

        private TranslationManager _translationManager;

        private WorkModel _workModel;

        public LaunchStatusViewModel()
        {
            this._translationManager = ViewModelFactory.translationManager;
            this._workModel = ViewModelFactory.workModel;
            this.InitiateProxys();
        }

        private void InitiateProxys()
        {
            this._workContents = new ObservableCollection<WorkContent>();
            foreach (WorkContent workContent in WorkModel.CurrentWorks)
            {
                this.WorkContents.Add(workContent);
            }
        }

        public void CheckAndLaunchSaves()
        {
            InitiateProxys();
            WorkStatus[] states = { WorkStatus.Waiting };
            if (this.WorkModel.CheckIfLaunchWithStates(states))
            {
                this.WorkModel.LaunchGroupedThreaded(updateWork,errorPopUp);
            }
        }

        private bool errorPopUp(string key, params string[] args)
        {
            DialogResult dialogResult = MessageBox.Show(
                this._translationManager.Translate(key,args), this._translationManager.Translate("messagebox.info"), MessageBoxButtons.YesNo
                );
            if (dialogResult == DialogResult.Yes)
            {
                return true; 
            }
            else if (dialogResult == DialogResult.No)
            {
                return false;
            }
            return false;
        }

        private void updateWork(Work work)
        {
            WorkProxy workProxy = work as WorkProxy;
            if(workProxy != null)
            {
                workProxy.Progress = workProxy.getPercentage();
                workProxy.OnPropertyChanged("");
            }
        }

        public ObservableCollection<WorkContent> WorkContents { get => _workContents; set => _workContents = value; }

        //translations
        public string GetTrsBackUpName
        { get { return this._translationManager.Translate("workstatus.BackUpName"); } }

        public string GetTrsSource
        { get { return this._translationManager.Translate("workstatus.Source"); } }

        public string GetTrsDestination
        { get { return this._translationManager.Translate("workstatus.Destination"); } }

        public string GetTrsBackUpStatus
        { get { return this._translationManager.Translate("workstatus.BackUpStatus"); } }

        public string GetTrsRemainingFileCount
        { get { return this._translationManager.Translate("workstatus.RemainingFileCount"); } }

        public string GetTrsRemainingSize
        { get { return this._translationManager.Translate("workstatus.RemainingSize"); } }

        public string PauseBtn
        { get { return this._translationManager.Translate("btns.pause"); } }
        public string ResumeBtn
        { get { return this._translationManager.Translate("btns.resume"); } }
        public string StopBtn
        { get { return this._translationManager.Translate("btns.stop"); } }
        public string GridHeader { get { return this._translationManager.Translate("headers.launchStatus"); } }


        //Buttons
        private ICommand _pauseCommand;
        private ICommand _resumeCommand;
        private ICommand _stopCommand;

        public ICommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                {
                    _pauseCommand = new RelayCommand(param => PauseWork(param), param => true);
                }
                return _pauseCommand;
            }
        }

        public ICommand ResumeCommand
        {
            get
            {
                if (_resumeCommand == null)
                {
                    _resumeCommand = new RelayCommand(param => ResumeWork(param), param => true);
                }
                return _resumeCommand;
            }
        }

        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand(param => StopWork(param), param => true);
                }
                return _stopCommand;
            }
        }

        public WorkModel WorkModel { get => _workModel; set => _workModel = value; }

        //Associated commands
        private void StopWork(object obj)
        {
            WorkContent workContent = obj as WorkContent;
            if(obj != null)
            {
                this._workModel.InterruptWorkContent(workContent);
            }
        }
        private void ResumeWork(object obj)
        {
            WorkContent workContent = obj as WorkContent;
            if (workContent != null)
            {
                _workModel.ResumeWorkContent(workContent);
            }
        }
        private void PauseWork(object obj)
        {
            WorkContent workContent = obj as WorkContent;
            if (workContent != null)
            {
                _workModel.PauseWorkContent(workContent);
            }
        }

        public void OnShow()
        {
            this.CheckAndLaunchSaves();
        }
    }
}
