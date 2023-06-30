using ProjectLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using Caliburn.Micro;
using EasySaveClientPanel.ViewModels;
using System.Threading;
using EasySaveWPF.ViewModels.Proxys;

namespace EasySaveClientPanel.Models
{
	public class Client
	{
		private const int port = 8888;
		private string serverAddress;
		private TcpClient client;
		private NetworkStream stream;
		private string test;
		public BindableCollection<Backup> backups;
		private Backup backup;
		
		private WorkProxy workProxy;

		public WorkProxy WorkProxy { get => workProxy; set => workProxy = value; }

		public Client()
		{
				// Se connecter au serveur
				this.workProxy = new WorkProxy();
				this.backup = new Backup();
				this.backup.Label = "";
				this.workProxy.Backup = this.backup;

		}
		public void start(string ip)
		{
			try
			{
				this.serverAddress = ip;
				this.client = new TcpClient(serverAddress, port);
				this.stream = this.client.GetStream();
				MessageBox.Show("Connecté au serveur.");
				ReceptMsg();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		public void stop()
		{
			try
			{
				this.client.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void SendAndReceptPrgr(object msg) 
		{
			SendMsg(msg);
			Thread progressThread = new Thread(() =>
			{
				ReceptProgress();

			});
			progressThread.Start();
		}

		public void SendString(string msg)
		{
			byte[] Bytes = Encoding.Default.GetBytes(msg);
			this.stream.Write(Bytes, 0, Bytes.Length);
		}
		public void SendMsg(object msg)
		{

			string jsonWork = JsonSerializer.Serialize(msg);
			byte[] Bytes = Encoding.Default.GetBytes(jsonWork);
			this.stream.Write(Bytes, 0, Bytes.Length);
		}

		public void ReceptProgress()
		{
			try
			{
				while (true)
				{
					var buffer = new byte[4096];
					var bytesRead = this.stream.Read(buffer, 0, buffer.Length);
					var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
					if (message != "")
					{
						WorkProxy receivedProxy = JsonSerializer.Deserialize<WorkProxy>(message);
						this.workProxy.CurrentSourcePath = receivedProxy.CurrentSourcePath;
						this.workProxy.Status = receivedProxy.Status;
						this.workProxy.CurrentDestinationPath = receivedProxy.CurrentDestinationPath;
						this.workProxy.RemainingFileCount = receivedProxy.RemainingFileCount;
						this.workProxy.RemainingFileSize = receivedProxy.RemainingFileSize;
						this.workProxy.Progress = receivedProxy.Progress;
						this.workProxy.Backup.Label = receivedProxy.Backup.Label;
						//destination
						//status
						//remainingFileCount
						//remainingFileSize

						/*
						var check = Convert.ToInt32(message);
						var dblProgress = Convert.ToDouble(message);
						homeViewModel.wrkProgress = dblProgress;
						if (check == 100)
						{
							break;
						}*/
					}

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void ReceptMsg()
		{
				var buffer = new byte[4096];
				var bytesRead = stream.Read(buffer, 0, buffer.Length);
				var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
				this.backups = JsonSerializer.Deserialize<BindableCollection<Backup>>(message);
				//ObservableCollection<Backup> list = JsonSerializer.Deserialize<ObservableCollection<Backup>>(message.Substring(0, message.Length));
			
		}

	}
}
