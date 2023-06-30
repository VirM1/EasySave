using EasySaveWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectLibrary.Models;

namespace EasySaveWPF.Views
{
    /// <summary>
    /// Logique d'interaction pour LaunchStatusView.xaml
    /// </summary>
    public partial class LaunchStatusView : UserControl
    {
        public LaunchStatusView()
        {
            InitializeComponent();

            DataContext = ViewModelFactory.GetViewModel("LaunchStatusViewModel");
        }
    }
}
