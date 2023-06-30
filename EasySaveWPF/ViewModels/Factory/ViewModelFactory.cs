using ProjectLibrary.EnvManager;
using ProjectLibrary.ForbiddenFilesLibrary;
using ProjectLibrary.Models;
using ProjectLibrary.TranslationLibrary;
using System;
using System.Linq;
using System.Collections.Generic;
using ProjectLibrary.PriorityLibrary;
using EasySaveWPF.Models;
using ProjectLibrary.CryptLibrary;

namespace EasySaveWPF.ViewModels
{
    class ViewModelFactory
    {
        public static EnvFileManager envFileManager;

        public static TranslationManager translationManager;

        public static ForbiddenFilesManager forbiddenFilesManager;

        public static PriorityManager priorityManager;

        public static WorkModel workModel;

        public static BackupModel backupModel;

        public static CryptedExtensionsManager cryptedExtensionsManager;

        private static IEnumerable<IViewModel> _viewModels;

        public static IEnumerable<IViewModel> ViewModels { get => _viewModels; set => _viewModels = value; }

        public static void InitFactory()
        {
            string envDir = Environment.CurrentDirectory;

            EnvFileManager envFileManager = new EnvFileManager(envDir);
            ViewModelFactory.envFileManager = envFileManager;

            TranslationManager translationManager = new TranslationManager(
                envFileManager.TranslationLocation,
                envFileManager.BaseLocale
            );
            ViewModelFactory.translationManager = translationManager;

            ForbiddenFilesManager forbiddenFilesManager = new ForbiddenFilesManager(envFileManager.ForbiddenFilesLocation);
            ViewModelFactory.forbiddenFilesManager = forbiddenFilesManager;

            PriorityManager PriorityManager = new PriorityManager(envFileManager.PriorityLocation);
            ViewModelFactory.priorityManager = PriorityManager;

            BackupModel backupModel = new BackupModel(envFileManager.BackupLocation);
            ViewModelFactory.backupModel = backupModel;

            CryptedExtensionsManager cryptedExtensionsManager = new CryptedExtensionsManager(envFileManager.CryptedExtensionsLocation);
            ViewModelFactory.cryptedExtensionsManager = cryptedExtensionsManager;

            WorkModel workModel = new WorkModel(forbiddenFilesManager, PriorityManager, envFileManager,cryptedExtensionsManager);
            ViewModelFactory.workModel = workModel;


            InitViewModels();
        }


        private static void InitViewModels()
        {
            ViewModelFactory._viewModels = new IViewModel[]
            {
                new LaunchSavViewModel(),
                new BannedFilesViewModel(),
                new SavManagementViewModel(),
                new SettingsViewModel(),
                new LaunchStatusViewModel(),
                new PriorityViewModel(),
                new CryptedFilesViewModel()
            };
        }

        public static IViewModel GetViewModel(object parameter)
        {
            return ViewModelFactory._viewModels.FirstOrDefault(x => x.GetType().Name == parameter.ToString());
        }
    }
}
