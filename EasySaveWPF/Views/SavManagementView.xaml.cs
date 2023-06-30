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
using EasySaveWPF.ViewModels;
using Microsoft.Win32;
using System.Diagnostics;
using ProjectLibrary.Models;

namespace EasySaveWPF.Views
{
	/// <summary>
	/// Logique d'interaction pour SavManagementView.xaml
	/// </summary>
	public partial class SavManagementView : UserControl
	{
		public SavManagementView()
		{
			InitializeComponent();

            DataContext = ViewModelFactory.GetViewModel("SavManagementViewModel");
        }

        private void BackUpList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
			SavManagementViewModel viewM = DataContext as SavManagementViewModel;
			if(viewM != null)
            {
				DataGrid datagrid = sender as DataGrid;
				if(datagrid != null)
                {
					viewM.update(datagrid.SelectedItem as Backup);
				}

			}
        }
    }
}
