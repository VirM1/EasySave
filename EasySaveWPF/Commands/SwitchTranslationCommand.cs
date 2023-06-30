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
    class SwitchTranslationCommand : ICommand
    {
        private SettingsViewModel viewModel;
        public SwitchTranslationCommand(SettingsViewModel viewModel)
        {
            this.viewModel = viewModel;
        }
        public SettingsViewModel getMainViewModel()
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
            string[] availableLocales = new string[] { "fr", "en", "es" };
            string convertedParam = Convert.ToString(parameter);
            if (availableLocales.Contains(convertedParam))
            {
                viewModel.ChangeLanguage(Convert.ToString(parameter));
            }
        }
    }
}
