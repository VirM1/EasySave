using EasySaveWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EasySaveWPF.Views;

namespace EasySaveWPF.Commands
{
    public class UpdateViewCommand : ICommand
    {
        private MainViewModel viewModel;
        private MainWindow mainWindow;
        public UpdateViewCommand(MainViewModel viewModel, MainWindow mainWindow)
        {
            this.viewModel = viewModel;
            this.mainWindow = mainWindow;
			viewModel.SelectedViewModel = ViewModelFactory.GetViewModel("LaunchSavViewModel");
        }
        public MainViewModel getMainViewModel()
        {
            return this.viewModel;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            viewModel.SelectedViewModel = ViewModelFactory.GetViewModel(parameter);
            if (viewModel.SelectedViewModel is LaunchSavViewModel)
            {
                LaunchSavViewModel viewM = viewModel.SelectedViewModel as LaunchSavViewModel;
                viewM.UpdateList();
            }else if (viewModel.SelectedViewModel is LaunchStatusViewModel)
            {
                LaunchStatusViewModel viewM = viewModel.SelectedViewModel as LaunchStatusViewModel;
                viewM.CheckAndLaunchSaves();
            }
        }
    }
}
