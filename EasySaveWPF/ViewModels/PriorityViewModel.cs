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
using ProjectLibrary.PriorityLibrary;
using ProjectLibrary.ForbiddenFilesLibrary;
// using ProjectLibrary.ForbiddenFilesLibrary;

namespace EasySaveWPF.ViewModels
{
    public class PriorityViewModel : Notified, IViewModel
    {

        private string _appName;
        private BindableCollection<Priority> _Priority;
        private PriorityManager _PriorityManager;
        private TranslationManager _translationManager;

        public TranslationManager TranslationManager { get => _translationManager; set => _translationManager = value; }


        //Translated text
        public string GridHeader { get { return this._translationManager.Translate("headers.priority"); } }
        public string SaveButton { get { return this._translationManager.Translate("btns.save"); } }
        public string DeleteButton { get { return this._translationManager.Translate("btns.delete"); } }
        public string ExtensionText { get { return this._translationManager.Translate("labels.extension"); } }

        public PriorityManager PriorityManager { get => _PriorityManager; set => _PriorityManager = value; }
        public PriorityViewModel()
        {
            this._translationManager = ViewModelFactory.translationManager;
            this._PriorityManager = ViewModelFactory.priorityManager;
            Priority = new BindableCollection<Priority>(this._PriorityManager.ListExtentions);
            Priority.CollectionChanged += PriorityChanged;
        }

        public BindableCollection<Priority> Priority { get => _Priority; set => _Priority = value; }

        public string AppName
        {
            get { return _appName; }
            set
            {
                _appName = value;
                OnPropertyChanged("AppName");
            }
        }

        private ICommand _addExtensionCommand;

        public ICommand addExtensionCommand
        {
            get
            {
                if (_addExtensionCommand == null)
                {
                    _addExtensionCommand = new RelayCommand(param => addExtension(param), param => true);
                }
                return _addExtensionCommand;
            }
        }

        private ICommand _deleteExtensionCommand;

        public ICommand deleteExtensionCommand
        {
            get
            {
                if (_deleteExtensionCommand == null)
                {
                    _deleteExtensionCommand = new RelayCommand(param => deleteExtension(param), param => true);
                }
                return _deleteExtensionCommand;
            }
        }

        private void addExtension(object param) {
            if (string.IsNullOrWhiteSpace(this.AppName))
            {
                System.Windows.MessageBox.Show("Extension cannot be null");
            }
            else if (this.PriorityManager.ListExtentions.Any(x => x.Name == this.AppName))
            {
                System.Windows.MessageBox.Show("Extension already prioritized");
            }
            else
            {
                this.Priority.Add((new Priority() { Name = this.AppName }));
            }
        }

        private void deleteExtension(object param) 
        {
            IList<object> selectedItems = param as IList<object>;
            if (selectedItems != null)
            {
                List<Priority> itemsToRemove = selectedItems.Cast<Priority>().ToList();
                foreach (Priority item in itemsToRemove)
                {
                    this.Priority.Remove(item);
                }
            }

            this.Priority.Refresh();
        }

        private void PriorityChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Priority item in e.NewItems)
                {
                    this.PriorityManager.ListExtentions.Add(item);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Priority item in e.OldItems)
                {
                    this.PriorityManager.ListExtentions.Remove(item);
                }
            }

            this._PriorityManager.UpdateFile();
        }


        public void OnShow()
        {

        }
    }
}
