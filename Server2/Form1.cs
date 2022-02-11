using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
			StartpointPanel2 = panel2.Location;
			//userControl11.GeneralText.Click += GeneralText1_Click;
			AllColors = Enum.GetNames(typeof(KnownColor));

			label1.ForeColor = Color.Red;
			UsersMessages[0] = UserFromMessage;
			////panel2.AutoScrollMinSize = new Size(0, w[0].Size.Height);
			//panel2.VerticalScroll.Enabled = false;
			flowLayoutPanel1.VerticalScroll.LargeChange = UserFromMessage.Height;
			flowLayoutPanel2.VerticalScroll.LargeChange = UserFromMessage.Height;
			flowLayoutPanel1.VerticalScroll.SmallChange = UserFromMessage.Height;
			flowLayoutPanel2.VerticalScroll.SmallChange = UserFromMessage.Height;
			//panel2.VerticalScroll.Enabled = true;
			//panel2.MouseWheel += Panel2_MouseWheel1;
			//vScrollBar1.
			//panel2.MouseWheel += Panel2_MouseWheel;

			//this.DoubleBuffered = true;
		}

		TcpClient[] TcpClientsss = new TcpClient[5];

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

		//new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder().AddArgument("Action1", "viewConversation2").Show();

		void ThreadClient(object StateInfo)
		{
			TcpClient tcpClient = (TcpClient)StateInfo;
			NetworkStream netstream = tcpClient.GetStream();
			ClientCount++;
			label2.Invoke(new Action(() => timer1.Enabled = true));
			//byte[] test = new byte[44];
			//test = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\n<meta http-equiv=\"Refresh\" content=\"80\">\nContent-type: text/html\nContent-Length:44\n\n<html><body><h1>Itworks!</h1></body></html>");
			//netstream.Write(test, 0, test.Length);

			label2.Invoke(new Action(() => label2.Text = "Clients connected: " + ClientCount));

			AcceptMessage(tcpClient);
		}
		async void AcceptMessage(TcpClient tcpClient)
		{
			NetworkStream netstream = tcpClient.GetStream();
			//HistoryChat.

			await Task.Run(() =>
			{
				byte[] Buffer/* = Encoding.UTF8.GetBytes("Hello world!")*/;
				Buffer = new byte[256];
				int countbyteInt;
				string StrBuffer;

				string[] CheckNameCountFileStr;
				string CheckNameCountFileStr2;
				FileStream file1 = null;
				try
				{
					while (tcpClient.Connected)
					{
						CheckNameCountFileStr = null;
						CheckNameCountFileStr2 = null;
						StrBuffer = null;

						countbyteInt = tcpClient.Client.Receive(Buffer, 0, Buffer.Length, SocketFlags.None);

						StrBuffer = Encoding.UTF8.GetString(Buffer, 0, countbyteInt);
						if (countbyteInt == 0 && StrBuffer.Length == 0 && Buffer[0] == 0)
						{
							ClientCount--;
							TcpClientsss[ClientCount] = null;
							tcpClient.Close();
							break;
						}

						MatchCollection FileReceived = Regex.Matches(StrBuffer, "▲FILE▲");
						MatchCollection TextReceived = Regex.Matches(StrBuffer, "▲TEXT▲");

						//string A = Regex.Split();
						//if (a.Count == 2)
						//{
						//}
						//CheckNameCountFileStr[0] = " ";

						//FileStream file1 = null;
						const string AcceptedFiles = @"Accepted files\";
						string NameFileStr;
						string newBufferStr;
						int CountInt;

						if (Buffer.Length > 0 && StrBuffer.Length > 0 && FileReceived.Count == 2)
						{
							CheckNameCountFileStr2 = StrBuffer.Remove(0, FileReceived[0].Index + FileReceived[0].Length);
							//newBufferStr = CheckNameCountFileStr2.Remove(FileReceived[1].Index - FileReceived[0].Length, FileReceived[0].Length);
							CheckNameCountFileStr2 = CheckNameCountFileStr2.Remove(FileReceived[1].Index - FileReceived[0].Length);
							newBufferStr = CheckNameCountFileStr2.Remove(0, CheckNameCountFileStr2.Length);

							CheckNameCountFileStr = CheckNameCountFileStr2.Split(' ');
							NameFileStr = ParseNameFile(CheckNameCountFileStr);
							CountInt = Convert.ToInt32(CheckNameCountFileStr[CheckNameCountFileStr.Length - 1]);

							if (!Directory.Exists(AcceptedFiles))
							{
								Directory.CreateDirectory(AcceptedFiles);
							}
							FileStream file2;
							file2 = File.Create(AcceptedFiles + NameFileStr);
							byte[] newBuffer = Encoding.UTF8.GetBytes(newBufferStr);

							if (newBuffer.Length > 0)
							{
								file2.Write(newBuffer, 0, newBuffer.Length);
							}
							file2.Close();
							file1 = File.OpenWrite(AcceptedFiles + NameFileStr);

							int CountIntWrite = 0;
							int CountInTheFileInt = 0;

							Stopwatch stopwatch = new Stopwatch();
							//Stopwatch BPSStopWatch = new Stopwatch();
							SocketError se;
							double Propgress1 = 0;

							byte[] BufferForFile = new byte[5242880];
							//BinaryReader a = new BinaryReader(netstream);
							while (file1.Length < CountInt)
							{
								//tcpClient.Client.ReceiveBufferSize = 22888;
								//a.BaseStream.Flush();
								//CountIntWrite = a.Read(BufferForFile, 0, CountInt);
								//CountIntWrite = netstream.Read(BufferForFile, 0, CountInt);

								CountIntWrite = tcpClient.Client.Receive(BufferForFile, 0, 5242880, SocketFlags.None, out se);
								//_ = Encoding.UTF8.GetString(BufferForFile, 0, 256);
								//BPSStopWatch.Start();

								stopwatch.Start();
								Invoke(new Action(() => textBox2.Text += " " + CountIntWrite));
								Invoke(new Action(() => textBox2.SelectionStart = textBox2.Text.Length));
								file1.Write(BufferForFile, 0, CountIntWrite);
								CountInTheFileInt += CountIntWrite;
								file1.Close();
								Invoke(new Action(() =>
								{
									textBox6.Text = stopwatch.Elapsed.TotalSeconds.ToString("0.0"); // Секунд прошло
									textBox5.Text = ((CountInt - CountInTheFileInt) / 1024.0 / 1024.0).ToString("0.000"); // Сколько осталось MB
									textBox4.Text = (CountInTheFileInt / 1024.0 / 1024.0 / stopwatch.Elapsed.TotalSeconds).ToString("0.00"); // Скорость в секунду MB/s
									label5.Font = Control.DefaultFont;
									label5.Text = ((int)(((CountInt - CountInTheFileInt) / 1024.0 / 1024.0) / (CountInTheFileInt / 1024.0 / 1024.0 / stopwatch.Elapsed.TotalSeconds))).ToString(); // Сколько времени осталось
								}));


								try
								{
									Propgress1 = 100 / (CountInt / (double)CountInTheFileInt);
									Invoke(new Action(() =>
									{
										progressBar1.Value = (int)Propgress1;
										label10.Text = ((int)Propgress1).ToString() + "%";
									}));
								}
								catch (System.Exception ex)
								{
									if (ex is DivideByZeroException)
									{
										Invoke(new Action(() => progressBar1.Value = (int)Propgress1));
									}
									else if (ex is System.ArgumentOutOfRangeException)
									{
										byte[] BufferForError = Encoding.UTF8.GetBytes("▲TEXT▲" + "Не нажимай на кнопку отправить файл несколько раз, пока файл не пришел!" + "▲TEXT▲");
										netstream.Write(BufferForError, 0, BufferForError.Length);
										Invoke(new Action(() =>
										{
											progressBar1.Value = 0;
											label10.Text = 0.ToString() + "%";
										}));
									}
									else
									{
										throw;
									}
								}

								if (stopwatch.Elapsed.TotalMinutes > 60)
								{
									MessageBox.Show("Принятие файла затянулось > 60 минут");
									file1.Close();
									break;
								}

							}

							stopwatch.Stop();
							//Array.Clear(BufferForFile, 0, BufferForFile.Length);
							//GC.Collect();
							//GC.WaitForPendingFinalizers();
							//tcpClient.Close();
							//tcpListener.Server.Dispose();
							file1.Close();

							for (int i = 0; i < TcpClientsss.Length; i++)
							{
								if (TcpClientsss[i] == tcpClient && TcpClientsss[i] != null)
								{
									SendFile(AcceptedFiles + NameFileStr, tcpClient);
								}
							}
							ReadLog("Файл принят: " + NameFileStr);

							//MessageBox.Show("Файл принят: " + NameFileStr + ". Файл принят за " + stopwatch.Elapsed.Minutes.ToString() + " минут " + stopwatch.Elapsed.Seconds.ToString() + " секунд " + stopwatch.Elapsed.Milliseconds.ToString() + " миллисекунд");
						}
						else if (StrBuffer.Length > 0 && TextReceived.Count == 2)
						{
							CheckNameCountFileStr2 = StrBuffer.Remove(0, TextReceived[0].Index + TextReceived[0].Length);
							newBufferStr = CheckNameCountFileStr2.Remove(TextReceived[1].Index - TextReceived[0].Length, TextReceived[0].Length);

							if (!string.IsNullOrWhiteSpace(newBufferStr))
							{
								Invoke(new Action(() => {
									for (int i = 0; i < UsersMessages.Length; i++)
									{
										if (UsersMessages[i] == null)
										{
											if (AllColors.Length <= IndexForColor)
											{
												IndexForColor = 0;
											}
											while (CheckColorNoBlack())
											{
												if (AllColors.Length <= IndexForColor)
												{
													IndexForColor = 0;
												}
												else
												{
													IndexForColor++;
													ColorsSkips++;
												}
											}
											if (AllColors[IndexForColor] == "Transparent")
												IndexForColor++;

											c = Color.FromName(AllColors[IndexForColor]);
											IndexForColor++;

											if (UserFromMessage.GetGeneralText.Length == 0)
											{
												UserFromMessage.GetGeneralText = newBufferStr;
												if (UserFromMessage.DateUp)
												{
													UserFromMessage.GetIPTextUp = "От: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
													UserFromMessage.GetDateTimeTextUp = DateTime.Now.ToString();
												}
												else if (!UserFromMessage.DateUp)
												{
													UserFromMessage.GetIPTextDown = "От: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
													UserFromMessage.GetDateTimeTextDown = DateTime.Now.ToString();
												}
											}
											else
											{
												CreateNewText(flowLayoutPanel2, newBufferStr, "От " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address, DateTime.Now, c);
											}
											//UserFromMessages[i].GetGeneralText = newBufferStr;
											break;
										}
									}
								}));

								ReadLog(newBufferStr);
							}
						}
						else
						{
							Exception se = new Exception("KTOTO HE TTPABUJLHO UCTTOJLSYET TTPOrPAMMY");

							throw se;
						}

						Thread.Sleep(1);
						Buffer = new byte[256];
					}
				}
				catch (Exception ex)
				{
					if (ex is IOException || ex is SocketException || ex.Message == "KTOTO HE TTPABUJLHO UCTTOJLSYET TTPOrPAMMY")
					{
						if (file1 != null)
						{
							file1.Close();
						}
						ClientCount--;
						if (TcpClientsss[ClientCount] != null)
						{
							TcpClientsss[ClientCount].Dispose();
							TcpClientsss[ClientCount].Close();
							TcpClientsss[ClientCount] = null;
						}
					}
					else
					{
						throw;
					}
				}
			});
		}

		string ParseNameFile(string[] File)
		{
			List<string> FileList = File.ToList();
			//FileList.RemoveAt(0);
			FileList.RemoveAt(FileList.Count - 1);
			File = FileList.ToArray();

			string NameFile = "";
			for (int i = 0; i < File.Length; i++)
			{
				NameFile = string.Join(" ", File);
			}
			NameFile = NameFile.Remove(0, 1);
			NameFile = NameFile.Remove(NameFile.Length - 1, 1);
			return NameFile;
		}

		FileStream fileLog;
		string nameFile = "file.log";
		public void ReadLog(string StrToLog)
		{
			fileLog = File.Open(nameFile, FileMode.Append);
			List<char> ListStrToLog = StrToLog.ToList();
			ListStrToLog.Insert(0, '{');
			ListStrToLog.Insert(1, '\n');

			StrToLog = string.Join("", ListStrToLog);

			StrToLog += "\n} [";
			StrToLog += DateTime.Now.ToString();
			StrToLog += "]\n";

			byte[] Buffer = Encoding.UTF8.GetBytes(StrToLog);
			fileLog.Write(Buffer, 0, Buffer.Length);
			fileLog.Close();
		}
		void ThreadServerSendToClient()
		{
			if (!string.IsNullOrWhiteSpace(richTextBox2.Text))
			{
				byte[] Buffer;
				for (int i = 0; i < TcpClientsss.Length && TcpClientsss[i] != null; i++)
				{
					//Buffer = new byte[4092];
					pictureBox3.Visible = true;
					Buffer = Encoding.UTF8.GetBytes("▲TEXT▲" + richTextBox2.Text + "▲TEXT▲");
					if (UserToMessage.GetGeneralText.Length == 0)
					{
						UserToMessage.GetGeneralText = richTextBox2.Text;
						if (UserToMessage.DateUp)
						{
							UserToMessage.GetIPTextUp = "От: " + ((IPEndPoint)TcpClientsss[i].Client.RemoteEndPoint).Address;
							UserToMessage.GetDateTimeTextUp = DateTime.Now.ToString();
						}
						else if (!UserToMessage.DateUp)
						{
							UserToMessage.GetIPTextDown = "От: " + ((IPEndPoint)TcpClientsss[i].Client.RemoteEndPoint).Address;
							UserToMessage.GetDateTimeTextDown = DateTime.Now.ToString();
						}
					}
					else
					{
						CreateNewText(flowLayoutPanel1, richTextBox2.Text, "Для: " + ((IPEndPoint)TcpClientsss[i].Client.RemoteEndPoint).Address.ToString(), DateTime.Now, null);
					}

					TcpClientsss[i].GetStream().Write(Buffer, 0, Buffer.Length);

					pictureBox3.Visible = false;
					if (!string.IsNullOrWhiteSpace(richTextBox2.Text))
					{
						pictureBox2.Visible = true;
					}
					if (!timer2.Enabled)
					{
						timer2.Enabled = true;
					}
				}
			}
		}
		TcpListener tcpListener;
		private void StartServer()
		{
			IPAddress iP = ParamsForServer.iP;
			int port = ParamsForServer.port;

			tcpListener = new TcpListener(iP, port);

			tcpListener.Start();
			Invoke(new Action(() => comboBox1.Enabled = false));
			Invoke(new Action(() => textBox3.Enabled = false));
			Invoke(new Action(() => textBox1.ReadOnly = true));

			label1.Invoke(new Action(ServerStarted));

			TcpClient tcpClient;
			Thread thread;
			while (true)
			{
				try
				{
					while (tcpListener != null)
					{
						if (!tcpListener.Pending())
						{
							Thread.Sleep(50);
						}
						else
						{
							break;
						}
					}
				}
				catch (InvalidOperationException ex)
				{
					if (ex.Message == "Прослушивание не выполняется. Перед вызовом этого метода необходимо вызвать метод Start().")
					{
						Application.Exit();
					}

				}

				try
				{
					if (tcpListener != null)
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
					else
					{
						break;
					}
				}
				catch (InvalidOperationException)
				{
					Application.Exit();
				}

			}
		}
		void IpChangeBox()
		{

		}
		private void button4_Click(object sender, EventArgs e)
		{
			CloseServer();
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
		void CloseServer()
		{
			for (int i = 0; i < TcpClientsss.Length; i++)
			{
				if (TcpClientsss[i] != null)
				{
					TcpClientsss[i].Close();
					TcpClientsss[i] = null;
				}
			}

			tcpListener.Stop();
			tcpListener = null;
			ServerNoConnected();

			comboBox1.Enabled = true;
			textBox3.Enabled = true;
			textBox1.ReadOnly = false;

			button3.Visible = true;
			button4.Visible = false;
			Server_isOnline = MyEnum.ServerNotStarted;
			button5.Enabled = false;
			button2.Enabled = false;
		}
		//IPAddress ParseIpByComboBox(ComboBox cb)
		//{
		//	string ip = "";
		//	for (int i = 0; i < cb.SelectedItem.ToString().Length; i++)
		//	{
		//		if (cb.SelectedItem.ToString()[i] == ':')
		//		{
		//			break;
		//		}
		//		ip += cb.SelectedItem.ToString()[i];
		//	}
		//	return IPAddress.Parse(ip);
		//}
		MyEnum ThreadServer()
		{
			IPAddress iP;
			int port;
			if (checkBox1.Checked == false)
			{
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

						Properties.Settings.Default.SavedPORT = port.ToString();

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
			else if (checkBox1.Checked == true)
			{
				if (textBox3.Text.Length > 0)
				{
					if (IPAddress.TryParse(textBox3.Text, out iP))
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
			else
			{
				return Server_isOnline = MyEnum.ServerNotStarted;
			}
		}
		MyEnum Server_isOnline = MyEnum.ServerNotStarted;

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

		private void Form1_Load_1(object sender, EventArgs e)
		{
			ServerNoConnected();
			IPAddress[] iPv4 = GetAllIpv4();
			//string[] iPv4_port = new string[iPv4.Length];
			//for (int i = 0; i < iPv4.Length; i++)
			//{
			//	iPv4_port[i] = iPv4[i].ToString() + ":80";
			//}

			comboBox1.Items.AddRange(iPv4);
			comboBox1.Items.Add(IPAddress.Loopback);

			if (comboBox1.Items.Count - 1 >= Properties.Settings.Default.IPComboBoxIndex)
			{
				comboBox1.SelectedItem = comboBox1.Items[Properties.Settings.Default.IPComboBoxIndex];
			}
			else
			{
				comboBox1.SelectedItem = 0;
			}

			label2.Text += " 0";

			//label3.Text = "Здесь приходит текст от клиентов:";
			//label4.Text = "Пиши сюда какой текст ты хочешь отправить клиентам:";
			textBox3.Text = comboBox1.Text;

			if (Directory.GetCurrentDirectory() == Environment.GetFolderPath(Environment.SpecialFolder.Desktop) && Directory.GetCurrentDirectory() == Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
			{
				this.Text += " — Рабочий стол";
			}
			else
			{
				this.Text += " — " + Directory.GetCurrentDirectory();
			}
			if (Properties.Settings.Default.SavedPORT.Length > 0)
			{
				textBox1.Text = Properties.Settings.Default.SavedPORT;
			}

		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (Server_isOnline == MyEnum.ServerNotStarted)
			{
				Server_isOnline = ThreadServer();
				if (Server_isOnline == MyEnum.ServerStartedAllOk)
				{
					Console.Beep(4000, 150);
					button4.Visible = true;
					if (FileSend != null && FileSend.Length > 0)
					{
						button5.Enabled = true;
					}
					button2.Enabled = true;

				}
				else if (Server_isOnline == MyEnum.IpIsNotValible)
				{
					Console.Beep(40, 300);
					button2.Enabled = false;
				}
				else if (Server_isOnline == MyEnum.portIsNoValible)
				{
					Console.Beep(40, 300);
					button2.Enabled = false;
				}
			}
		}

		private void button1_Click_2(object sender, EventArgs e)
		{
			ThreadServerSendToClient();
		}

		private void timer1_Tick_1(object sender, EventArgs e)
		{
			if (label2.Text[label2.Text.Length - 1].ToString() != ClientCount.ToString())
			{
				label2.Text = "Clients connected: " + ClientCount;
			}
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			pictureBox2.Visible = false;
			pictureBox3.Visible = false;
			timer2.Enabled = false;
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{

		}
		string CurretNameFile;
		async void SendFile()
		{
			await Task.Run(() =>
			{
				CurretNameFile = ParseNameFile();

				//string[] FILES = Directory.GetFileSystemEntries(FileSend);
				//            if (Directory.Exists(FILES[0]))
				//            {
				//	FILES = Directory.GetFileSystemEntries(FILES[0]);
				//            }

				FileStream filestream = File.OpenRead(FileSend);
				string LENGHTfileStr = filestream.Length.ToString();
				filestream.Dispose();
				filestream.Close();

				byte[] CheckNameCountFileByte = Encoding.UTF8.GetBytes("▲FILE▲\"" + CurretNameFile + "\" " + LENGHTfileStr + "▲FILE▲");
				string TestStr = Encoding.UTF8.GetString(CheckNameCountFileByte);
				Invoke(new Action(() => richTextBox3.Text = CurretNameFile));
				Invoke(new Action(() => richTextBox3.SelectionStart = richTextBox3.Text.Length));

				for (int i = 0; i < TcpClientsss.Length; i++)
				{
					if (TcpClientsss[i] != null && TcpClientsss[i].Connected)
					{
						try
						{
							TcpClientsss[i].GetStream().Write(CheckNameCountFileByte, 0, CheckNameCountFileByte.Length);
							TcpClientsss[i].Client.SendFile(FileSend);
							ReadLog("Файл отправлен: " + CurretNameFile);
						}
						catch (Exception ex)
						{
							if (ex is SocketException || ex is IOException)
							{
								ReadLog("Сбой при отправке файла: " + CurretNameFile);
							}
							else
							{
								throw;
							}
							//Application.Restart();
						}

					}
				}
				Thread.Sleep(1);
			});
		}
		string ParseNameFile()
		{
			string FileName = "";
			for (int i = 0; i < FileSend.Length; i++)
			{
				if (FileSend[FileSend.Length - 1 - i] == '\\')
				{
					break;
				}
				FileName += FileSend[FileSend.Length - 1 - i];
			}
			List<char> a = FileName.ToList();
			a.Reverse();
			FileName = string.Join("", a);

			return FileName;
		}
		async void SendFile(string FileSend, TcpClient tcpclientSender)
		{
			await Task.Run(() =>
			{
				CurretNameFile = ParseNameFile(FileSend);
				byte[] CheckNameCountFileByte;

				FileStream filestream = File.OpenRead(FileSend);
				string LENGHTfileStr = filestream.Length.ToString();
				filestream.Dispose();
				filestream.Close();
				while (true)
				{
					try
					{
						CheckNameCountFileByte = Encoding.UTF8.GetBytes("▲FILE▲\"" + CurretNameFile + "\" " + LENGHTfileStr + "▲FILE▲");
						break;
					}
					catch (System.IO.IOException)
					{
						MessageBox.Show("Ошибки не будет, но лучше не запускай два и более клиента в одной директории");
					}
				}
				string TestStr = Encoding.UTF8.GetString(CheckNameCountFileByte);

				Invoke(new Action(() => {
					richTextBox3.Text = CurretNameFile;
					richTextBox3.SelectionStart = richTextBox3.Text.Length;
					this.Activate();
				}));

				for (int i = 0; i < TcpClientsss.Length; i++)
				{
					if (TcpClientsss[i] != tcpclientSender && TcpClientsss[i] != null && TcpClientsss[i].Connected)
					{
						TcpClientsss[i].GetStream().Write(CheckNameCountFileByte, 0, CheckNameCountFileByte.Length);
						TcpClientsss[i].Client.SendFile(FileSend, null, null, TransmitFileOptions.UseDefaultWorkerThread);
					}
				}
				Thread.Sleep(1);

			});
		}
		async void SendFilesnDir()
		{
			await Task.Run(() =>
			{

			});
		}
		string ParseNameFile(string FileSend)
		{
			string FileName = "";
			for (int i = 0; i < FileSend.Length; i++)
			{
				if (FileSend[FileSend.Length - 1 - i] == '\\')
				{
					break;
				}
				FileName += FileSend[FileSend.Length - 1 - i];
			}
			List<char> a = FileName.ToList();
			a.Reverse();
			FileName = string.Join("", a);

			return FileName;
		}

		string FileSend;
		private void button2_Click(object sender, EventArgs e)
		{
			
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.Save();
			if (tcpListener != null)
			{
				tcpListener.Stop();
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			SendFile();
		}
		//string[] TEST;
		private void button2_DragDrop(object sender, DragEventArgs e)
		{
			List<string> PathsDirectories = new List<string>();
			List<string> PathsFiles = new List<string>();
			foreach (string item in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				if (Directory.Exists(item))
				{
					PathsDirectories.AddRange(Directory.GetFiles(item, "*.*", SearchOption.AllDirectories));
				}
				else if(File.Exists(item))
				{
					PathsFiles.Add(item);
				}
			}
			//TEST = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			button5.Enabled = true;
			if(PathsFiles.Count > 0)
			{
				FileSend = string.Join("\r\n", PathsFiles);
			}
			if (PathsDirectories.Count > 0)
			{
				FileSend = string.Join("\r\n", PathsDirectories);
			}

			richTextBox3.Text = FileSend;
			richTextBox3.SelectionStart = richTextBox3.TextLength;
			ActiveControl = richTextBox3;
		}

		private void button2_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void button2_DragLeave(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked == true)
			{
				textBox3.Visible = true;
				comboBox1.Visible = false;
			}
			else if (((CheckBox)sender).Checked == false)
			{
				comboBox1.Visible = true;
				textBox3.Visible = false;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.IPComboBoxIndex = comboBox1.SelectedIndex;
		}

		//public static T Clone<T>(T controlToClone) where T : Control
		//{
		//	T instance = Activator.CreateInstance<T>();

		//	Type control = controlToClone.GetType();
		//	PropertyInfo[] info = control.GetProperties();
		//	object p = control.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, controlToClone, null);
		//	foreach (PropertyInfo pi in info)
		//	{
		//		if ((pi.CanWrite) && !(pi.Name == "WindowTarget") && !(pi.Name == "Capture"))
		//		{
		//			pi.SetValue(instance, pi.GetValue(controlToClone, null), null);
		//		}
		//	}
		//	return instance;
		//}
		//      public static T Clone<T>(this T controlToClone) 
		//	where T : Control
		//{
		//	PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

		//	T instance = Activator.CreateInstance<T>();

		//	foreach (PropertyInfo propInfo in controlProperties)
		//	{
		//		if (propInfo.CanWrite)
		//		{
		//			if (propInfo.Name != "WindowTarget")
		//				propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
		//		}
		//	}

		//	return instance;
		//}


		public UserControl1[] UsersMessages = new UserControl1[340];
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="panelToAddIn"></param>
		/// <param name="Text"></param>
		/// <param name="iP"></param>
		/// <param name="DateTimeText"></param>
		/// <param name="color">Сделайте значение <see langword="null"/>, чтобы получился случайный цвет</param>
		/// <returns></returns>
		int CreateNewText(Panel panelToAddIn, string Text, string iP, string DateTimeText, Color? color)
		{
			for (int i = 0; i < UsersMessages.Length; i++)
			{
				if (UsersMessages[i] == null)
				{
					UsersMessages[i] = new UserControl1();
					UsersMessages[i].DateUp = UsersMessages[0].DateUp;
					UsersMessages[i].GetGeneralText = Text;

					if (color == null)
					{
						if (AllColors.Length <= IndexForColor)
						{
							IndexForColor = 0;
						}
						while (CheckColorNoBlack())
						{
							if (AllColors.Length <= IndexForColor)
							{
								IndexForColor = 0;
							}
							else
							{
								IndexForColor++;
								ColorsSkips++;
							}
						}
						if (AllColors.Length <= IndexForColor)
						{
							IndexForColor = 0;
						}
						if (AllColors[IndexForColor] == "Transparent")
							IndexForColor++;
						UsersMessages[i].GetGeneralColor = Color.FromName(AllColors[IndexForColor]);
						IndexForColor++;
					}
					else
					{
						UsersMessages[i].GetGeneralColor = color.Value;
					}

					UsersMessages[i].Anchor = UserFromMessage.Anchor;
					UsersMessages[i].Dock = UserFromMessage.Dock;
					if (UsersMessages[i].DateUp)
					{
						UsersMessages[i].GetIPTextUp = iP;
						UsersMessages[i].GetDateTimeTextUp = DateTimeText;
					}
					else if (!UsersMessages[i].DateUp)
					{
						UsersMessages[i].GetIPTextDown = iP;
						UsersMessages[i].GetDateTimeTextDown = DateTimeText;
					}

					panelToAddIn.Controls.Add(UsersMessages[i]);
					UsersMessages[i].Location = new Point(UsersMessages[i - 1].Location.X, UsersMessages[i - 1].Location.Y + UsersMessages[i - 1].Size.Height + 1);
					panelToAddIn.ScrollControlIntoView(UsersMessages[i]);
					return i;
				}
			}
			return -1;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="panelToAddIn"></param>
		/// <param name="Text"></param>
		/// <param name="iP"></param>
		/// <param name="DateTime"></param>
		/// <param name="color">Сделайте значение <see langword="null"/>, чтобы получился случайный цвет</param>
		/// <returns></returns>
		int CreateNewText(Panel panelToAddIn, string Text, string iP, DateTime DateTime, Color? color)
		{
			for (int i = 0; i < UsersMessages.Length; i++)
			{
				if (UsersMessages[i] == null)
				{
					UsersMessages[i] = new UserControl1();
					UsersMessages[i].DateUp = UsersMessages[0].DateUp;
					UsersMessages[i].GetGeneralText = Text;

					if (color == null)
					{
						if (AllColors.Length <= IndexForColor)
						{
							IndexForColor = 0;
						}
						while (CheckColorNoBlack())
						{
							if (AllColors.Length <= IndexForColor)
							{
								IndexForColor = 0;
							}
							else
							{
								IndexForColor++;
								ColorsSkips++;
							}
						}
						if (AllColors.Length <= IndexForColor)
						{
							IndexForColor = 0;
						}
						if (AllColors[IndexForColor] == "Transparent")
							IndexForColor++;

						UsersMessages[i].GetGeneralColor = Color.FromName(AllColors[IndexForColor]);
						IndexForColor++;
					}
					else
					{
						UsersMessages[i].GetGeneralColor = color.Value;
					}

					UsersMessages[i].Anchor = UserFromMessage.Anchor;
					UsersMessages[i].Dock = UserFromMessage.Dock;
					if (UsersMessages[i].DateUp)
					{
						UsersMessages[i].GetIPTextUp = iP;
						UsersMessages[i].GetDateTimeTextUp = DateTime.ToString();
					}
					else if (!UsersMessages[i].DateUp)
					{
						UsersMessages[i].GetIPTextDown = iP;
						UsersMessages[i].GetDateTimeTextDown = DateTime.ToString();
					}

					panelToAddIn.Controls.Add(UsersMessages[i]);
					UsersMessages[i].Location = new Point(UsersMessages[i - 1].Location.X, UsersMessages[i - 1].Location.Y + UsersMessages[i - 1].Size.Height + 1);
					panelToAddIn.ScrollControlIntoView(UsersMessages[i]);
					return i;
				}
			}
			return -1;
		}
		

		string[] AllColors;
		int IndexForColor = 0;
		int ColorsSkips = 0;
		System.Drawing.Color c;
		private void button6_Click(object sender, EventArgs e)
		{
			//tableFromMessage.RowCount++;
			//SystemColors.
			if (AllColors.Length <= IndexForColor)
			{
				IndexForColor = 0;
			}
			while (CheckColorNoBlack())
			{
				if (AllColors.Length <= IndexForColor)
				{
					IndexForColor = 0;
				}
				else
				{
					IndexForColor++;
					ColorsSkips++;
				}
			}
			if (AllColors.Length <= IndexForColor)
			{
				IndexForColor = 0;
			}
			if (AllColors[IndexForColor] == "Transparent")
				IndexForColor++;

			c = Color.FromName(AllColors[IndexForColor]);
			IndexForColor++;

			CreateNewText(flowLayoutPanel1, string.Concat(Enumerable.Repeat("S", 255)), "Для: " + "255.255.255.255", DateTime.Now.ToString(), c);
			CreateNewText(flowLayoutPanel2, string.Concat(Enumerable.Repeat("S", 255)), "От:  " + "255.255.255.255", DateTime.Now.ToString(), c);

			//panel2.VerticalScroll.Value = w[0].Size.Height * d;
		}
		// Проверяет цвета на черный. Чтобы текст не сливался с фоном
		bool CheckColorNoBlack()
		{
			Color color1 = Color.FromName(AllColors[IndexForColor]);
			Color color2 = Color.FromArgb(255, 0, 0, 0);
			return color1.ToArgb() == color2.ToArgb();
		}
		Point StartpointPanel2;
		//async private void flowLayoutPanel1_LocationChanged(object sender, EventArgs e)
		//{
		//	//if (this.IsHandleCreated)
		//	//{
		//	//	await Task.Run(() =>
		//	//	{
		//	//		Invoke(new Action(() =>
		//	//		{
		//	//			if (flowLayoutPanel1.Location.X >= StartpointFlow1.X * 2 && !StartpointFlow1.IsEmpty)
		//	//			{
		//	//				flowLayoutPanel1.Location = new Point(StartpointFlow1.X * 2, flowLayoutPanel2.Location.Y);
		//	//			}
		//	//			else if (flowLayoutPanel1.Location.X <= StartpointFlow1.X && !StartpointFlow1.IsEmpty)
		//	//			{
		//	//				flowLayoutPanel1.Location = new Point(StartpointFlow1.X, flowLayoutPanel2.Location.Y);
		//	//			}
		//	//		}));
		//	//	});
		//	//};
		//}

		private void flowLayoutPanel2_ControlAdded(object sender, ControlEventArgs e)
		{
			if ((panel2.Location.Y < StartpointPanel2.Y + UserFromMessage.Size.Height) && !StartpointPanel2.IsEmpty)
			{
				panel2.Location = new Point(panel2.Location.X, panel2.Location.Y + UserFromMessage.Size.Height);
			}
			//tableLayoutPanel3.Size = new Size(tableLayoutPanel3.Size.Width, tableLayoutPanel3.Size.Height + UserFromMessage.Height);
		}

		//async private void label12_LocationChanged(object sender, EventArgs e)
		//{
			//if (this.IsHandleCreated && this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
			//{
			//	await Task.Run(() =>
			//	{
			//		Invoke(new Action(() =>
			//		{
			//			if(label12.Location.X >= StartpointLabel12.X * 2 && !StartpointLabel12.IsEmpty)
			//			{
			//				label12.Location = new Point(StartpointLabel12.X * 2, label3.Location.Y);
			//			}
			//			else if(label12.Location.X <= StartpointLabel12.X && !StartpointLabel12.IsEmpty)
			//			{
			//				label12.Location = new Point(StartpointLabel12.X, label3.Location.Y);
			//			}
			//		}));
			//	});
   //         }
   //         else if(WindowState == FormWindowState.Minimized)
   //         {
			//	PosBeforeResize = flowLayoutPanel1.Location;
   //         }
		//}

		//private void Form1_Resize(object sender, EventArgs e)
		//{
			
		//}

		//     private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
		//     {
		//int i = w.Count(x => x != null);
		//int s = i;

		//         //vScrollBar1.SmallChange;
		//         //vScrollBar1.LargeChange;
		//     }
		//void Copy(Control source, Control destination)
		//{
		//	var pdc = TypeDescriptor.GetProperties(source);

		//	// Копируем значения всех свойств
		//	foreach (PropertyDescriptor pd in pdc)
		//	{
		//		if (!pd.IsReadOnly)
		//			pd.SetValue(destination, pd.GetValue(source));
		//	}

		//	// Создаём копии вложенных контролов и копируем содержания
		//	foreach (Control ctrl in source.Controls)
		//	{
		//		var ctrl2 = (Control)Activator.CreateInstance(ctrl.GetType());
		//		Copy(ctrl, ctrl2);
		//		ctrl2.Visible = true;
		//		destination.Controls.Add(ctrl2);
		//	}
		//}
		
		///// <summary>
		///// 
		///// </summary>
		///// <param name="panelToAddIn"></param>
		///// <param name="Text"></param>
		///// <param name="IP"></param>
		///// <param name="DateTime"></param>
		///// <param name="color">Сделайте значение <see langword="null"/>, чтобы получился случайный цвет</param>
		///// <returns></returns>
		//int CreateNewText(Panel panelToAddIn, string Text, IPAddress IP, DateTime DateTime, Color? color)
		//{
		//	for (int i = 0; i < UsersMessages.Length; i++)
		//	{
		//		if (UsersMessages[i] == null)
		//		{
		//			UsersMessages[i] = new UserControl1();
		//			UsersMessages[i].DateUp = UsersMessages[0].DateUp;
		//			UsersMessages[i].GetGeneralText = Text;
		//			if (color == null)
		//			{
		//				if (AllColors.Length <= IndexForColor)
		//				{
		//					IndexForColor = 0;
		//				}
		//				while (CheckColorNoBlack())
		//				{
		//					if (AllColors.Length <= IndexForColor)
		//					{
		//						IndexForColor = 0;
		//					}
		//					else
		//					{
		//						IndexForColor++;
		//						ColorsSkips++;
		//					}
		//				}
		//				if (AllColors.Length <= IndexForColor)
		//				{
		//					IndexForColor = 0;
		//				}
		//				if (AllColors[IndexForColor] == "Transparent")
		//					IndexForColor++;
		//				UsersMessages[i].GetGeneralColor = Color.FromName(AllColors[IndexForColor]);
		//				IndexForColor++;
		//			}
		//			else
		//			{
		//				UsersMessages[i].GetGeneralColor = color.Value;
		//			}

		//			UsersMessages[i].Anchor = UserFromMessage.Anchor;
		//			UsersMessages[i].Dock = UserFromMessage.Dock;
		//			if (UsersMessages[i].DateUp)
		//			{
		//				UsersMessages[i].GetIPTextUp = IP.ToString();
		//				UsersMessages[i].GetDateTimeTextUp = DateTime.ToString();
		//			}
		//			else if (!UsersMessages[i].DateUp)
		//			{
		//				UsersMessages[i].GetIPTextDown = IP.ToString();
		//				UsersMessages[i].GetDateTimeTextDown = DateTime.ToString();
		//			}

		//			panelToAddIn.Controls.Add(UsersMessages[i]);
		//			UsersMessages[i].Location = new Point(UsersMessages[i - 1].Location.X, UsersMessages[i - 1].Location.Y + UsersMessages[i - 1].Size.Height);
		//			return i;
		//		}
		//	}
		//	return -1;
		//}
		///// <summary>
		///// 
		///// </summary>
		///// <param name="panelToAddIn"></param>
		///// <param name="Text"></param>
		///// <param name="iP"></param>
		///// <param name="DateTimeText"></param>
		///// <param name="color">Сделайте значение <see langword="null"/>, чтобы получился случайный цвет</param>
		///// <returns></returns>
		//void CreateNewText(Panel panelToAddIn, string Text, IPAddress iP, string DateTimeText, Color? color)
		//{
		//	for (int i = 0; i < UsersMessages.Length; i++)
		//	{
		//		if (UsersMessages[i] == null)
		//		{
		//			UsersMessages[i] = new UserControl1();
		//			UsersMessages[i].DateUp = UsersMessages[0].DateUp;
		//			UsersMessages[i].GetGeneralText = Text;
		//			if (color == null)
		//			{
		//				if (AllColors.Length <= IndexForColor)
		//				{
		//					IndexForColor = 0;
		//				}
		//				while (CheckColorNoBlack())
		//				{
		//					if (AllColors.Length <= IndexForColor)
		//					{
		//						IndexForColor = 0;
		//					}
		//					else
		//					{
		//						IndexForColor++;
		//						ColorsSkips++;
		//					}
		//				}
		//				if (AllColors.Length <= IndexForColor)
		//				{
		//					IndexForColor = 0;
		//				}
		//				if (AllColors[IndexForColor] == "Transparent")
		//					IndexForColor++;
		//				UsersMessages[i].GetGeneralColor = Color.FromName(AllColors[IndexForColor]);
		//				IndexForColor++;
		//			}
		//			else
		//			{
		//				UsersMessages[i].GetGeneralColor = color.Value;
		//			}

		//			UsersMessages[i].Anchor = UserFromMessage.Anchor;
		//			UsersMessages[i].Dock = UserFromMessage.Dock;
		//			if (UsersMessages[i].DateUp)
		//			{
		//				UsersMessages[i].GetIPTextUp = iP.ToString();
		//				UsersMessages[i].GetDateTimeTextUp = DateTimeText;
		//			}
		//			else if (!UsersMessages[i].DateUp)
		//			{
		//				UsersMessages[i].GetIPTextDown = iP.ToString();
		//				UsersMessages[i].GetDateTimeTextDown = DateTimeText;
		//			}

		//			panelToAddIn.Controls.Add(UsersMessages[i]);
		//			UsersMessages[i].Location = new Point(UsersMessages[i - 1].Location.X, UsersMessages[i - 1].Location.Y + UsersMessages[i - 1].Size.Height + 1);
		//			break;
		//		}
		//	}
		//}

	}
}