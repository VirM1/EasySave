using EasySaveClientPanel.Models;
using EasySaveWPF.Commands;
using EasySaveWPF.ViewModels;
using EasySaveWPF.ViewModels.Proxys;
using ProjectLibrary.Models;
using ProjectLibrary.TranslationLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Threading;
using EasySaveWPF.Models;

namespace EasySaveClientPanel.ViewModels
{
	public class StatusViewModel : Notified,IViewModel
	{
		public Client client;

		private ObservableCollection<WorkProxy> _workProxys;

		private TranslationManager _translationManager;

		private WorkModel _workModel;
		private Backup backup;
		public StatusViewModel(Client client)
		{
			this.client = client;
			//this.backup = this.client.backups;
			//this._workModel = client.workModel;
			
		}


		

		private void InitiateProxys()
		{
				var wrk = new ObservableCollection<WorkProxy>();
				wrk.Add(this.client.WorkProxy);

				this._workProxys = wrk;
				
		}

		

		public ObservableCollection<WorkProxy> WorProxyss { get => _workProxys; set => _workProxys = value; }

		


		//Buttons
		private ICommand _pauseCommand;
		private ICommand _resumeCommand;
		private ICommand _stopCommand;

		public ICommand PauseCommand
		{
			get
			{
				if (_pauseCommand == null)
				{
					_pauseCommand = new RelayCommand(param => PauseWork(param), param => true);
				}
				return _pauseCommand;
			}
		}

		public ICommand ResumeCommand
		{
			get
			{
				if (_resumeCommand == null)
				{
					_resumeCommand = new RelayCommand(param => ResumeWork(param), param => true);
				}
				return _resumeCommand;
			}
		}

		public ICommand StopCommand
		{
			get
			{
				if (_stopCommand == null)
				{
					_stopCommand = new RelayCommand(param => StopWork(param), param => true);
				}
				return _stopCommand;
			}
		}

		public WorkModel WorkModel { get => _workModel; set => _workModel = value; }

		//Associated commands
		private void StopWork(object obj)
		{
			WorkContent workContent = obj as WorkContent;
			if (obj != null)
			{
				Debug.WriteLine("StopWork");
				Debug.WriteLine(workContent.Work.Backup.Label);
			}
		}
		private void ResumeWork(object obj)
		{
			WorkContent workContent = obj as WorkContent;
			if (workContent != null)
			{
				_workModel.ResumeWorkContent(workContent);
			}
		}
		private void PauseWork(object obj)
		{
			this.client.SendString("Pause");
				//_workModel.PauseWorkContent(workContent);
		}

		public void OnShow()
		{
			
				this.InitiateProxys();

			
		}
	}
}

