using EasySaveWPF.Views;
using EasySaveWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ProjectLibrary.Models;
using ProjectLibrary.TranslationLibrary;
using ProjectLibrary.EnvManager;
using ProjectLibrary.ForbiddenFilesLibrary;
using System.Threading;


namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private Mutex mutex;

        public MainWindow()
        {
            bool isOnlyInstance;
            mutex = new Mutex(true, "EasySaveWPF_Application_Unique_Id", out isOnlyInstance);
            

            try
            {
                
                InitializeComponent();
                Closing += MainWindow_Closing;
                ViewModelFactory.InitFactory();
                DataContext = new MainViewModel();
                if (!isOnlyInstance)
                {
                    MessageBox.Show((DataContext as MainViewModel)?.Instance);
                    Application.Current.Shutdown();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                Application.Current.Shutdown();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mutex.ReleaseMutex();
        }
    }
}
