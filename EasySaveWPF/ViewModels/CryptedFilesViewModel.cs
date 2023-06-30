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
using ProjectLibrary.CryptLibrary;

namespace EasySaveWPF.ViewModels
{
    public class CryptedFilesViewModel : Notified, IViewModel
    {

        private string _appName;
        private BindableCollection<CryptedFile> _CryptedFile;
        private CryptedExtensionsManager _CryptedExtensionManager;
        private TranslationManager _translationManager;

        public TranslationManager TranslationManager { get => _translationManager; set => _translationManager = value; }


        //Translated text
        public string GridHeader { get { return this._translationManager.Translate("headers.crypted"); } }
        public string SaveButton { get { return this._translationManager.Translate("btns.save"); } }
        public string DeleteButton { get { return this._translationManager.Translate("btns.delete"); } }
        public string ExtensionText { get { return this._translationManager.Translate("labels.extension"); } }

        public CryptedExtensionsManager CryptedExtensionsManager { get => _CryptedExtensionManager; set => _CryptedExtensionManager = value; }
        public CryptedFilesViewModel()
        {
            this._translationManager = ViewModelFactory.translationManager;
            this._CryptedExtensionManager = ViewModelFactory.cryptedExtensionsManager;
            CryptedFile = new BindableCollection<CryptedFile>(this._CryptedExtensionManager.ListExtentions);
            CryptedFile.CollectionChanged += CryptedChanged;
        }

        public BindableCollection<CryptedFile> CryptedFile { get => _CryptedFile; set => _CryptedFile = value; }

        public string AppName
        {
            get { return _appName; }
            set
            {
                _appName = value;
                OnPropertyChanged("AppName");
            }
        }

        private ICommand _addCryptedCommand;

        public ICommand AddCryptedCommand
        {
            get
            {
                if (_addCryptedCommand == null)
                {
                    _addCryptedCommand = new RelayCommand(param => AddCrypted(param), param => true);
                }
                return _addCryptedCommand;
            }
        }

        private ICommand _deleteCryptedCommand;

        public ICommand DeleteCryptedCommand
        {
            get
            {
                if (_deleteCryptedCommand == null)
                {
                    _deleteCryptedCommand = new RelayCommand(param => DeleteCrypted(param), param => true);
                }
                return _deleteCryptedCommand;
            }
        }

        private void AddCrypted(object param)
        {
            if (string.IsNullOrWhiteSpace(this.AppName))
            {
                System.Windows.MessageBox.Show("Extension cannot be null");
            }
            else if (this.CryptedExtensionsManager.ListExtentions.Any(x => x.Name == this.AppName))
            {
                System.Windows.MessageBox.Show("Extension already crypted");
            }
            else
            {
                this.CryptedFile.Add((new CryptedFile() { Name = this.AppName }));
            }
        }

        private void DeleteCrypted(object param)
        {
            IList<object> selectedItems = param as IList<object>;
            if (selectedItems != null)
            {
                List<CryptedFile> itemsToRemove = selectedItems.Cast<CryptedFile>().ToList();
                foreach (CryptedFile item in itemsToRemove)
                {
                    this.CryptedFile.Remove(item);
                }
            }

            this.CryptedFile.Refresh();
        }

        private void CryptedChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (CryptedFile item in e.NewItems)
                {
                    this.CryptedExtensionsManager.ListExtentions.Add(item);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (CryptedFile item in e.OldItems)
                {
                    this.CryptedExtensionsManager.ListExtentions.Remove(item);
                }
            }

            this._CryptedExtensionManager.UpdateFile();
        }


        public void OnShow()
        {

        }
    }
}