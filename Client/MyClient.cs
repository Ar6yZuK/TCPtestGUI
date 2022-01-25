using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;

namespace Client
{
	class MyClient : Client
	{
		TcpClient tcpclient;
		public MyClient()
		{
			tcpclient = new TcpClient();
			tcpclient.Connect(IPAddress.Any, 80);
			tcpclient.Close();
		}
		static void Main(string[] args)
		{
			;
		}
	}
}
