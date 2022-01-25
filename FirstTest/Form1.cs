using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace Server2
{
	public partial class Form1 : Form
	{
		int ClientCount = 0;
		enum MyEnum
		{
			ServerNotStarted = -1, ServerStartedAllOk = 0, portIsNoValible = 1, IpIsNotValible = 2
		}
		public Form1()
		{
			InitializeComponent();
			label1.ForeColor = Color.Red;
		}
		TcpClient[] TcpClientsss = new TcpClient[5];
		public void textBox1_TextChanged(object sender, EventArgs e)
		{

		}
		private void button1_Click(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Class1 a = new Class1();
			a.Test(this);
		}
		
		/// <summary>
		/// Возвращает количество присвоенных iP
		/// </summary>
		/// <param name="iPv4Adresses">Массив в который будут записаны действующие iPv4 адреса</param>>
		IPAddress[] GetAllIpv4()
		{
			string HostName = Dns.GetHostName();
			IPAddress[] AlliPAddresses = Dns.GetHostAddresses(HostName);
			int CountIp = AlliPAddresses.Length;

			int j = 0;
			for (int i = 0; i < CountIp; i++)
			{
				if (AlliPAddresses[i].AddressFamily == AddressFamily.InterNetwork)
				{
					j++;
				}
			}
			int CountIpv4 = j;
			IPAddress[] iPv4Adresses = new IPAddress[CountIpv4];
			for (int i = 0; i < CountIpv4; i++)
			{
				iPv4Adresses[i] = AlliPAddresses[i];
			}
			
			return iPv4Adresses;
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			ServerNoConnected();
			IPAddress[] iPv4 = GetAllIpv4();
			//string[] iPv4_port = new string[iPv4.Length];
			//for (int i = 0; i < iPv4.Length; i++)
			//{
			//	iPv4_port[i] = iPv4[i].ToString() + ":80";
			//}

			comboBox1.Items.AddRange(iPv4);
			if(comboBox1.Items.Count != 0)
				comboBox1.SelectedItem = comboBox1.Items[1];
			comboBox1.Items.Add(IPAddress.Loopback);

			label2.Text += " 0";

			label3.Text = "Здесь приходит текст от клиентов:";
			label4.Text = "Пиши сюда какой текст ты хочешь отправить клиентам:";
		}
		
		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			//new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder().AddArgument("Action1", "viewConversation2").Show();
		}

		void ThreadClient(object StateInfo)
		{
			TcpClient tcpClient = (TcpClient)StateInfo;
			NetworkStream netstream = tcpClient.GetStream();
			ClientCount++;
			//byte[] test = new byte[44];
			//test = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\n<meta http-equiv=\"Refresh\" content=\"80\">\nContent-type: text/html\nContent-Length:44\n\n<html><body><h1>Itworks!</h1></body></html>");
			//netstream.Write(test, 0, test.Length);
			
			label2.Invoke(new Action(() => label2.Text = "Clients connected: " + ClientCount));

			//richTextBox1.Invoke(new Action(() => richTextBox1.Text = "Client connected"));

			byte[] Buffer/* = Encoding.UTF8.GetBytes("Hello world!")*/;
			//netstream.Write(Buffer, 0,Buffer.Length);
			Buffer = new byte[256];
			int countbyte;
			string StrBuffer;
			try
			{
				while (tcpClient.Connected)
				{
					countbyte = netstream.Read(Buffer, 0, Buffer.Length);

					if (countbyte == 0 && Buffer[0] == 0)
					{
						ClientCount--;
						TcpClientsss[ClientCount] = null;
						tcpClient.Close();
						break;
					}
					StrBuffer = null;
					StrBuffer = Encoding.UTF8.GetString(Buffer, 0, countbyte);

					ReadLog(StrBuffer);

					Invoke(new Action(()=>richTextBox1.Text = StrBuffer));
					Thread.Sleep(1);
					Buffer = new byte[4092];
				}
			}
			catch (System.IO.IOException)
			{
				ClientCount--;
				TcpClientsss[ClientCount] = null;
				tcpClient.Close();
			}
		}
		FileStream fileLog;
		string nameFile = "file.log";
		public void ReadLog(string StrToLog)
		{
			fileLog = File.Open(nameFile, FileMode.Append);
			StrToLog += " [";
			StrToLog += DateTime.Now.ToString();
			StrToLog += "]\n";
			byte[] Buffer = Encoding.UTF8.GetBytes(StrToLog);
			fileLog.Write(Buffer, 0, Buffer.Length);
			fileLog.Close();
		}
		void ThreadServerSendToClient()
		{
			byte[] Buffer;
			for (int i = 0; i < TcpClientsss.Length && TcpClientsss[i] != null; i++) {
				//Buffer = new byte[4092];
				pictureBox2.Visible = true;
				Buffer = Encoding.UTF8.GetBytes(richTextBox2.Text);
				
				TcpClientsss[i].GetStream().Write(Buffer, 0, Buffer.Length);
				pictureBox3.Visible = false;
				if (richTextBox2.Text.Length > 0)
				{
					pictureBox2.Visible = true;
				}
				if (!timer2.Enabled)
				{
					timer2.Enabled = true;
				}
			} 
		}
		private void StartServer()
		{
			IPAddress iP = ParamsForServer.iP;
			int port = ParamsForServer.port;

			TcpListener tcpListener = new TcpListener(iP, port);
			
			tcpListener.Start();
			
			label1.Invoke(new Action(ServerStarted));

			TcpClient tcpClient;
			Thread thread;
			while (true)
			{
				tcpClient = tcpListener.AcceptTcpClient();
				for (int i = 0; i < TcpClientsss.Length; i++)
				{
					if (TcpClientsss[i] == null)
					{
						TcpClientsss[i] = tcpClient;
						break;
					}
				}
				thread = new Thread(ThreadClient);
				thread.IsBackground = true;

				thread.Start(tcpClient);
				Thread.Sleep(100);
			}
		}
		void ServerStarted()
		{
			label1.ForeColor = Color.Lime;
			label1.Text = "Connected";
		}
		void ServerNoConnected()
		{
			label1.ForeColor = Color.Red;
			label1.Text = "No connected";
		}
		IPAddress ParseIpByComboBox(ComboBox cb)
		{
			string ip = "";
			for(int i = 0; i < cb.SelectedItem.ToString().Length; i++)
			{
				if (cb.SelectedItem.ToString()[i] == ':')
				{
					break;
				}
				ip += cb.SelectedItem.ToString()[i];
			}
			return IPAddress.Parse(ip);
		}
		MyEnum ThreadServer()
		{
			IPAddress iP;
			int port;
			if (comboBox1.SelectedItem != null)
			{
				if (IPAddress.TryParse(comboBox1.SelectedItem.ToString(), out iP))
				{
					try
					{
						port = Convert.ToInt32(textBox1.Text);
						if (!(port > 0 && port < 65535))
						{
							return MyEnum.portIsNoValible;
						}
					}
					catch (FormatException)
					{
						return MyEnum.portIsNoValible;
					}
					ParamsForServer paramsForServer = new ParamsForServer(iP, port);

					Thread thread = new Thread(StartServer);

					thread.Start();
					thread.IsBackground = true;
					if (thread.IsAlive)
					{
						button3.Visible = false;
						return MyEnum.ServerStartedAllOk;
					}
					else
					{
						button3.Visible = true;
						return MyEnum.ServerNotStarted;
					}
				}
				else
				{
					button3.Visible = true;
					return MyEnum.IpIsNotValible;
				}
			}
			else
			{
				return MyEnum.IpIsNotValible;
			}
		}
		MyEnum Server_isOnline = MyEnum.ServerNotStarted;
		private void button3_Click_1(object sender, EventArgs e)
		{
			if (Server_isOnline == MyEnum.ServerNotStarted)
			{
				Server_isOnline = ThreadServer();
				if (Server_isOnline == MyEnum.IpIsNotValible)
				{
					Console.Beep(40, 30);
					ServerNoConnected();
				}
				else if(Server_isOnline == MyEnum.portIsNoValible)
				{
					Console.Beep(40, 30);
					ServerNoConnected();
				}
				else if(Server_isOnline == MyEnum.ServerStartedAllOk)
				{
					Console.Beep(3000, 30);
					ServerStarted();
				}
			}
			else if(Server_isOnline == MyEnum.IpIsNotValible)
			{
				Server_isOnline = ThreadServer();
				if (Server_isOnline == MyEnum.IpIsNotValible)
				{
					Console.Beep(40, 30);
					ServerNoConnected();
				}
				else if (Server_isOnline == MyEnum.portIsNoValible)
				{
					Console.Beep(40, 30);
					ServerNoConnected();
				}
				else if (Server_isOnline == MyEnum.ServerStartedAllOk)
				{
					Console.Beep(3000, 30);
					ServerStarted();
				}
			}
			else if (Server_isOnline == MyEnum.portIsNoValible)
			{

			}
		}

		class ParamsForServer
		{
			static public IPAddress iP;
			static public int port;
			public ParamsForServer(IPAddress _iP, int _port)
			{
				iP = _iP;
				port = _port;
			}
		}
		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (label2.Text[label2.Text.Length - 1] != ClientCount)
			{
				label2.Text = "Clients connected: " + ClientCount;
			}
		}

		private void label4_Click(object sender, EventArgs e)
		{

		}
		
		private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			ThreadServerSendToClient();
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			pictureBox2.Visible = false;
			pictureBox3.Visible = false;
			timer2.Enabled = false;
		}
	}
}