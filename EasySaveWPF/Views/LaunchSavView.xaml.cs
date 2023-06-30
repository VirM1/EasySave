using EasySaveWPF.ViewModels;
using ProjectLibrary.EnvManager;
using ProjectLibrary.Models;
using ProjectLibrary.TranslationLibrary;
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
	/// Logique d'interaction pour LaunchSavView.xaml
	/// </summary>
	public partial class LaunchSavView : UserControl
	{

		public LaunchSavView()
		{
			InitializeComponent();

            DataContext = ViewModelFactory.GetViewModel("LaunchSavViewModel");
        }


	}
}
