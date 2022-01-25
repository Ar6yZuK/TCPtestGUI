using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Windows.Forms;

namespace Server
{
	class MyServer
	{
		TcpListener tcplistener;

		public MyServer()
		{
			tcplistener = new TcpListener(IPAddress.Any, 80);
			tcplistener.Start();

			while (true)
			{
				tcplistener.AcceptTcpClient();
			}
		}
		~MyServer()
		{
			if (tcplistener != null)
			{
				tcplistener.Stop();
			}
		}
		static void Main(string[] args)
        {
			
			new MyServer();
        }
	}
}
