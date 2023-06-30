using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EasySaveWPF.ViewModels;
using EasySaveWPF.Events;
using EasySaveClientPanel.Models;
using EasySaveWPF.Commands;

namespace EasySaveClientPanel.ViewModels
{
    public class MainViewModel : Notified
    {
        private IViewModel _selectedViewModel;
        private HomeViewModel _homeViewModel;
        private StatusViewModel _statusViewModel;
        private Client client;
        public IViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        public MainViewModel()
        {
            this.client = new Client();
            _homeViewModel = new HomeViewModel(client);
            _statusViewModel = new StatusViewModel(client);
			this.SelectedViewModel = _homeViewModel;
            InitiateSubscribers();
		}

		private ICommand _switchCommand;

		public ICommand SwitchCommand
		{
			get
			{
				if (_switchCommand == null)
				{
					_switchCommand = new RelayCommand(param => SwitchPage(param), param => true);
				}
				return _switchCommand;
			}
		}

		private void SwitchPage(object param)
		{
			string castedParam = param as String;

			if (castedParam != null)
			{
				Mediator.Notify(castedParam, castedParam);
			}
		}

		public void InitiateSubscribers()
		{
            Mediator.Subscribe("StatusViewModel",SwitchToStatus);
			Mediator.Subscribe("HomeViewModel", SwitchToHome);
		}
		private void SwitchToStatus(object obj)
		{
			this.SelectedViewModel = _statusViewModel;
			_statusViewModel.OnShow();
		}

		private void SwitchToHome(object obj)
		{
			this.SelectedViewModel = _homeViewModel;
			_homeViewModel.OnShow();
		}

		
	}
}
