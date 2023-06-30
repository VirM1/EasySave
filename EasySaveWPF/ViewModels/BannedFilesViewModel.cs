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
using ProjectLibrary.ForbiddenFilesLibrary;

namespace EasySaveWPF.ViewModels
{
    public class BannedFilesViewModel : Notified, IViewModel
    {

        private string _appName;

        private BindableCollection<ForbiddenApp> _bannedApps;

        private TranslationManager _translationManager;

        private ForbiddenFilesManager _forbiddenFilesManager;

        public BannedFilesViewModel()
        {
            this._translationManager = ViewModelFactory.translationManager;
            this.ForbiddenFilesManager = ViewModelFactory.forbiddenFilesManager;
            BannedApps = new BindableCollection<ForbiddenApp>(this.ForbiddenFilesManager.ListApps);
            BannedApps.CollectionChanged += BannedAppsChanged;
            AddForbiddenAppCommand = new AddForbiddenAppCommand(this);
        }
        public string AppName
        {
            get { return _appName; }
            set
            {
                _appName = value;
                OnPropertyChanged("AppName");
            }
        }
        public BindableCollection<ForbiddenApp> BannedApps { get => _bannedApps; set => _bannedApps = value; }

        public TranslationManager TranslationManager { get => _translationManager; set => _translationManager = value; }


        //translated strings
        public string GridHeader { get { return this._translationManager.Translate("headers.forbiddenApps"); } }
        public string BannedAppTextBlock { get { return this._translationManager.Translate("textBlock.bannedApp"); } }
        public string SaveButton { get { return this._translationManager.Translate("btns.save"); } }
        public string DeleteButton { get { return this._translationManager.Translate("btns.delete");  } }
        public string ForbiddenAppHeader { get { return this.TranslationManager.Translate("datagrid.headers.forbiddenApps"); } }



        public ICommand AddForbiddenAppCommand { get; set; }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(param => DeleteForbiddenApp(param), param => true);
                }
                return _deleteCommand;
            }
        }


        public ForbiddenFilesManager ForbiddenFilesManager { get => _forbiddenFilesManager; set => _forbiddenFilesManager = value; }

        private void DeleteForbiddenApp(object obj)
        {
            IList<object> selectedItems = obj as IList<object>;
            if (selectedItems != null)
            {
                List<ForbiddenApp> itemsToRemove = selectedItems.Cast<ForbiddenApp>().ToList();
                foreach (ForbiddenApp item in itemsToRemove)
                {
                    this.BannedApps.Remove(item);
                }
            }

            this.BannedApps.Refresh();
        }

        private void BannedAppsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (ForbiddenApp item in e.NewItems)
                {
                    this.ForbiddenFilesManager.ListApps.Add(item);
                }
            }else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (ForbiddenApp item in e.OldItems)
                {
                    this.ForbiddenFilesManager.ListApps.Remove(item);
                }
            }

            this.ForbiddenFilesManager.UpdateFile();
        }

        public void OnShow()
        {
        }
    }
}
