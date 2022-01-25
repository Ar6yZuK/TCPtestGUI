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

namespace ClientGUITest
{
	public partial class ClientTest1 : Form
	{
		public ClientTest1()
		{
			InitializeComponent();
		}

		private void ClientTest1_Load(object sender, EventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MyClient.ParseAll(this);
			MyClient a = new MyClient(this);
		}

		private void ClientTest1_FormClosed(object sender, FormClosedEventArgs e)
		{
			//Close();
		}

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
    class MyClient
	{
		static ClientTest1 form1;
		static TcpClient[] tcpClients = new TcpClient[5];
		static IPAddress iP;
		static int port;
		public int Port
		{
			get { return port; }
		}
		public IPAddress IP
		{
			get { return iP; }
		}
		public ClientTest1 FORM1
		{
			get
			{
				return form1;
			}
			protected set
			{
				form1 = value;
			}
		}
		public MyClient(ClientTest1 _form1)
		{
			FORM1 = _form1;
		}
		public static void ParseAll(ClientTest1 Form1)
		{
			if (IPAddress.TryParse(Form1.textBox1.Text, out iP))
			{
				try
				{
					port = Convert.ToInt32(Form1.textBox2.Text);
					if (port > 0 && port < 65535)
					{
						Console.Beep(4000, 30);
						ConnectToServer(iP, port, Form1);
					}
					else
					{
						Console.Beep(40, 30);
					}
				}
				catch (FormatException)
				{
					Console.Beep(40, 30);
				}
			}
			else
			{
				Console.Beep(40, 30);
			}
		}
		static public void ConnectToServer(IPAddress iP, int port, ClientTest1 Form1)
		{
			TcpClient tcpClient = new TcpClient();
			try
			{
				tcpClient.Connect(iP, port);
			}
			catch (SocketException)
			{

			}
			for (int i = 0; i < tcpClients.Length; i++)
			{
				if (tcpClients[i] == null)
				{
					tcpClients[i] = tcpClient;
					break;
				}
			}

			Form1.label4.Text = tcpClient.Connected.ToString();
			Form1.label4.Refresh();

			//Thread threadForAcceptSend = new Thread(ThreadAcceptSend);
			//threadForAcceptSend.IsBackground = true;
			//threadForAcceptSend.Start(tcpClient);
			while (true)
			{
				ThreadAcceptSend(tcpClient);
			}

			tcpClient.Close();
		}
		~MyClient()
		{
			for (int i = 0; i < tcpClients.Length && tcpClients[i] != null; i++)
			{
				tcpClients[i].Close();
			}
		}
		static void ThreadAcceptSend(object StateInfo)
		{
			TcpClient tcpClient = (TcpClient)StateInfo;
			byte[] Buffer = new byte[256];
			int CountByte;
			try
			{
				CountByte = tcpClient.GetStream().Read(Buffer, 0, Buffer.Length);
				form1.richTextBox1.Text = Encoding.UTF8.GetString(Buffer, 0, CountByte);
				Buffer = new byte[4092];
			}
			catch (System.IO.IOException)
			{
				Application.Exit();
				Environment.Exit(0);
			}
		}
	}
}
