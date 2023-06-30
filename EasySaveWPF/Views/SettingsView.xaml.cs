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

using System.Threading;
using System.IO;
using System.Text.Json;

using ProjectLibrary.TranslationLibrary;
using ProjectLibrary.Models;

using System.Transactions;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using ProjectLibrary.LogLibrary;

namespace EasySaveWPF.Views
{
	/// <summary>
	/// Logique d'interaction pour SettingsView.xaml
	/// </summary>
	public partial class SettingsView : UserControl
	{

		private string input;


		private TranslationManager translationManager;

		public SettingsView()
		{
			InitializeComponent();
		}

		void _Refresh(object sender, RoutedEventArgs e)
		{
			//this.Refresh();
		}
	}
}

