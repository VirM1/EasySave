using EasySaveClientPanel.Models;
using ProjectLibrary.Models;
using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectLibrary.Models;
using ProjectLibrary.Models;
using System.Windows.Input;
using EasySaveWPF.Commands;
using EasySaveWPF.ViewModels;
using EasySaveWPF.Events;
using System.Windows.Interop;

namespace EasySaveClientPanel.ViewModels
{
	public class HomeViewModel : Notified, IViewModel
	{

		private Client client;
		private BackupModel _backupModel;
		//Content datagrid
		private BindableCollection<Backup> _initialBackupList;

		private BindableCollection<Backup> _launchBackupList;

		public HomeViewModel(Client client)
		{
			this.client = client;
			this._launchBackupList = new BindableCollection<Backup>();

		}

		public BindableCollection<Backup> InitialBackupList
		{
			get { return _initialBackupList; }
			set
			{
				_initialBackupList = value;
				OnPropertyChanged(nameof(InitialBackupList));
			}
		}
		public BindableCollection<Backup> LaunchBackupList
		{
			get { return _launchBackupList; }
			set
			{
				_launchBackupList = value;
				OnPropertyChanged(nameof(LaunchBackupList));
			}
		}
		private ICommand _conectionCommand;

		public ICommand ConectionCommand
		{
			get
			{
				if (_conectionCommand == null)
				{
					_conectionCommand = new RelayCommand(param => Connection(param), param => true);
				}
				return _conectionCommand;
			}
		}
		private ICommand _disconnectCommand;

		public ICommand DisconnectCommand
		{
			get
			{
				if (_disconnectCommand == null)
				{
					_disconnectCommand = new RelayCommand(param => Disconnect(param), param => true);
				}
				return _disconnectCommand;
			}
		}

		private string _ip  = "127.0.0.1";
		public string Ip
		{
			get => _ip;
			set
			{
				_ip = value;
				OnPropertyChanged("Ip");
			}
		}
		private ICommand _movetoinitialCommand;
		private ICommand _movetolaunchCommand;
		private ICommand _launchToServCommand;

		public ICommand MoveToInitialCommand
		{
			get
			{
				if (_movetoinitialCommand == null)
				{
					_movetoinitialCommand = new RelayCommand(param => MoveToInitial(param), param => true);
				}
				return _movetoinitialCommand;
			}
		}
		public ICommand MoveToLaunchCommand
		{
			get
			{
				if (_movetolaunchCommand == null)
				{
					_movetolaunchCommand = new RelayCommand(param => MoveToLaunch(param), param => true);
				}
				return _movetolaunchCommand;
			}
		}
		public ICommand LaunchToServCommand
		{
			get
			{
				if (_launchToServCommand == null)
				{
					_launchToServCommand = new RelayCommand(param => LaunchBackUps(param), param => true);
				}
				return _launchToServCommand;
			}
		}


		private void Connection(object obj)
		{
			this.client.start(Ip);
			this._initialBackupList = this.client.backups;




		}
		private void Disconnect(object obj)
		{
			this.client.stop();
		}
		private void LaunchBackUps(object obj)
		{
			this.client.SendAndReceptPrgr(_launchBackupList);
			Mediator.Notify("StatusViewModel", "StatusViewModel");
		}
		private void MoveToInitial(object obj)
		{
			IList<object> selectedItems = obj as IList<object>;
			if (selectedItems != null)
			{
				List<Backup> itemsToRemove = selectedItems.Cast<Backup>().ToList();
				foreach (Backup item in itemsToRemove)
				{
					this.LaunchBackupList.Remove(item);
					this.InitialBackupList.Add(item);
				}
			}

			this.InitialBackupList.Refresh();
		}

		private void MoveToLaunch(object obj)
		{
			IList<object> selectedItems = obj as IList<object>;
			if (selectedItems != null)
			{
				List<Backup> itemsToRemove = selectedItems.Cast<Backup>().ToList();
				foreach (Backup item in itemsToRemove)
				{
					this.InitialBackupList.Remove(item);
					this.LaunchBackupList.Add(item);
				}
			}

			this.LaunchBackupList.Refresh();
		}
		public void OnShow()
		{
		}
	}
}
