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
    class AddForbiddenAppCommand : ICommand
    {
        private BannedFilesViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        public AddForbiddenAppCommand(BannedFilesViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            // Validation logic here. For example:
            if (string.IsNullOrWhiteSpace(_viewModel.AppName)){
                System.Windows.MessageBox.Show(_viewModel.TranslationManager.Translate("validation.sizeName"));
            }else if (_viewModel.ForbiddenFilesManager.ListApps.Any(x=>x.Name == _viewModel.AppName))
            {
                System.Windows.MessageBox.Show(_viewModel.TranslationManager.Translate("validation.alreadyExistingForbiddenApp"));
            }
            else
            {
                _viewModel.BannedApps.Add((new ForbiddenApp() { Name = _viewModel.AppName }));
            }
        }
    }
}
