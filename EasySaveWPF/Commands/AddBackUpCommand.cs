using EasySaveWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EasySaveWPF.Views;
using ProjectLibrary.Models;

namespace EasySaveWPF.Commands
{
    class AddBackUpCommand : ICommand
    {
        private SavManagementViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        public AddBackUpCommand(SavManagementViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            bool canSave = true;
            canSave = checkIndividuallyAndReturnErrorsLabel(canSave, this._viewModel.BackupModel.VerifyLabelDoesntExistAndValid(this._viewModel.Label), this._viewModel.Label);
            canSave = checkIndividuallyAndReturnErrors(canSave,this._viewModel.BackupModel.VerifyFolderExists(this._viewModel.Source), this._viewModel.Source);
            canSave = checkIndividuallyAndReturnErrors(canSave, this._viewModel.BackupModel.VerifyFolderExists(this._viewModel.Destination), this._viewModel.Destination);

            if (canSave)
            {
                Backup backup = this._viewModel.BackupModel.CreateBackup(
                    this._viewModel.Label,
                    this._viewModel.Source,
                    this._viewModel.Destination,
                    this._viewModel.BackupType
                    );
                int index = this._viewModel.BackUpList.IndexOf(backup);
                if(index != -1)
                {
                    this._viewModel.BackUpList[index] = backup;
                }
                else
                {
                    this._viewModel.BackUpList.Add(backup);
                }
                this._viewModel.BackUpList.Refresh();
            }
        }

        private bool checkIndividuallyAndReturnErrorsLabel(bool currentState, string? test, string testedValue)
        {
            if (!(test is null))
            {
                System.Windows.MessageBox.Show(_viewModel.TranslationManager.Translate(test, testedValue));
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(_viewModel.TranslationManager.Translate("handleBackup.testsException.update"), "Confirmation", System.Windows.MessageBoxButton.OKCancel);

                if (result == System.Windows.MessageBoxResult.OK)
                {
                    // Action si l'utilisateur clique sur OK
                }
                else
                {
                    // Action si l'utilisateur clique sur Annuler ou ferme la boîte de dialogue
                    currentState = false;
                }
                //currentState = false;
            }
            return currentState;
        }

        private bool checkIndividuallyAndReturnErrors(bool currentState,string? test,string testedValue)
        {
            if(!(test is null))
            {
                System.Windows.MessageBox.Show(_viewModel.TranslationManager.Translate(test, testedValue));
                currentState = false;
            }
            return currentState;
        }
    }
}
