using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Text.Json;
using Caliburn.Micro;
using ProjectLibrary.Models;
using EasySaveWPF.ViewModels;
using EasySaveWPF.ViewModels.Proxys;
using System.Windows.Forms;

namespace EasySaveWPF.Models
{
	public class Server
	{
		private readonly IPAddress ipAddress;
		private readonly int port;
		private ClientHandler handler;
		public BindableCollection<Backup> receptBackups;
		

		public Server(string ipAddressStr = "0.0.0.0", int port = 8888)
		{
			this.ipAddress = IPAddress.Parse(ipAddressStr);
			this.port = port;
		}

		public void Start(object msg, LaunchSavViewModel launchSavViewModel)
		{
			var listener = new TcpListener(ipAddress, port);
			listener.Start();
			Console.WriteLine($"Server listening on {ipAddress}:{port}");

			while (true)
			{
				var client = listener.AcceptTcpClient();
				this.handler = new ClientHandler(client);
				this.handler.Start(msg, launchSavViewModel);
			}
		}

		public void SendMsg(object msg)
		{
			this.handler.SendMsg(msg);
		}
	}


	public class ClientHandler
	{
		private readonly TcpClient client;
		private NetworkStream stream;
		public BindableCollection<Backup> receptBackups;
		public LaunchSavViewModel launchSavViewModel;
		public WorkModel workmodel;
	
		public ClientHandler(TcpClient client)
		{
			this.client = client;
			this.stream = this.client.GetStream();
		}

		public void Start(object msg, LaunchSavViewModel launchSavViewModel)
		{
			this.launchSavViewModel = launchSavViewModel;
			this.stream = client.GetStream();
			SendMsg(msg);
			bool ckeckMsg = ReceptMsg();

			this.workmodel = this.launchSavViewModel.WorkModel;
			Thread ReceptThread = new Thread(() =>
			{
				ReceptPauseResume();
			});
			ReceptThread.Start();
			if (ckeckMsg == true)
			{
				workLaunch();
			}
			Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
		}

		private void workLaunch()
		{
			this.workmodel.GenerateCurrentWorks(this.receptBackups, () => { return new WorkProxy(); });
			WorkStatus[] states = { WorkStatus.Waiting };
			if (this.workmodel.CheckIfLaunchWithStates(states))
			{
				this.workmodel.LaunchGroupedThreaded(updateWork, ErrorHandler);
			}
		}
		private bool ErrorHandler(string key, params string[] args)
		{
			return true;
		}
		private void updateWork(Work work)
		{
			WorkProxy workProxy = work as WorkProxy;
			if (workProxy != null)
			{
				workProxy.Progress = workProxy.getPercentage();
				double progressMsg = workProxy.Progress;
				if (progressMsg != null)
				{
					SendMsg(workProxy);
				}

			}
		}
	
		public void SendProgress(double progress)
		{
			var st = this.client.GetStream();
			var data = Encoding.UTF8.GetBytes(progress.ToString());
			st.Write(data, 0, data.Length);
		}
		public void SendMsg(object msg)
		{

			string jsonWork = JsonSerializer.Serialize(msg);
			byte[] Bytes = Encoding.Default.GetBytes(jsonWork);
			this.stream.Write(Bytes, 0, Bytes.Length);
			
		}

		public void ReceptPauseResume()
		{

			string message = "";
			try
			{
				while (true)
				{
					var buffer = new byte[4096];
					var bytesRead = stream.Read(buffer, 0, buffer.Length);
					message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
					if (message == "Pause")
					{
						break;

						//_workModel.PauseWorkContent(workContent);
					}
				}
			}
			catch (Exception ex)
			{
				//MessageBox.Show(ex.Message);
			}
		}
		public bool ReceptMsg()
		{
			string message = "";
			try
			{
				while (true)
				{
					var buffer = new byte[4096];
					var bytesRead = stream.Read(buffer, 0, buffer.Length);
					message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
					if (message != "")
					{
						
						break;
					}
				}
				this.receptBackups = JsonSerializer.Deserialize<BindableCollection<Backup>>(message);
				return true;
			}
			catch (Exception ex)
			{
				return false;
				//MessageBox.Show(ex.Message);
			}
		}

		public object ReceiveMsg()
		{
			try
			{
				var buffer = new byte[4096];
				var bytesRead = stream.Read(buffer, 0, buffer.Length);
				var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
				return message;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public void read(object msg)
		{
			//var bin = new BinaryFormatter();
			//bin.Serialize(stream, msg);	
		}




			private void customProtocolCompat(string message, string username, Dictionary<string, string> userPasswords, Dictionary<string, bool> userConnected)
		{
			// will need to have some hardening about values that can be parsed 
			if (userConnected[username] == false)
			{
				return; // don't go any further if user isn't connected
			}
			if (message.StartsWith("SIMPLE ECHO"))
			{
				Console.WriteLine($"{username} sent echo: {message.Replace("SIMPLE ECHO", "")}");
			}
		}
	}
}
