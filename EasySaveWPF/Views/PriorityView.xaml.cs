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

namespace EasySaveWPF.Views
{
    /// <summary>
    /// Interaction logic for PriorityView.xaml
    /// </summary>
    public partial class PriorityView : UserControl
    {
        public PriorityView()
        {
            InitializeComponent();

            DataContext = ViewModelFactory.GetViewModel("PriorityViewModel");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
