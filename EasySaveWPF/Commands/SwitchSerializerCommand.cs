using EasySaveWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProjectLibrary.LogLibrary;
using ProjectLibrary.EnvManager;
using EasySaveWPF.Views;

namespace EasySaveWPF.Commands
{
    class SwitchSerializerCommand : ICommand
    {
        private SettingsViewModel viewModel;

        private EnvFileManager _envFileManager;
        public SwitchSerializerCommand(SettingsViewModel viewModel, EnvFileManager envFileManager)
        {
            this.viewModel = viewModel;
            this._envFileManager = envFileManager;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (Convert.ToString(parameter) == "xml" || Convert.ToString(parameter) == "json")
            {
                LogExtension extension = (LogExtension)Enum.Parse(typeof(LogExtension), Convert.ToString(parameter), true);
                viewModel.WorkModel.SetExtension(extension);//todo @Virgile unsafe.


                this._envFileManager.LogExtension = extension;
                this._envFileManager.updateOrCreateEnvFile();
            }
        }
    }
}
