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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ClientGUITest2
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			Startpointpanel4 = panel4.Location;

			AllColors = Enum.GetNames(typeof(KnownColor));
			UsersMessages[0] = UserFromMessage;
			////panel2.AutoScrollMinSize = new Size(0, w[0].Size.Height);
			//panel2.VerticalScroll.Enabled = false;
			flowLayoutPanel1.VerticalScroll.LargeChange = UserFromMessage.Height;
			flowLayoutPanel2.VerticalScroll.LargeChange = UserFromMessage.Height;
			flowLayoutPanel1.VerticalScroll.SmallChange = UserFromMessage.Height;
			flowLayoutPanel2.VerticalScroll.SmallChange = UserFromMessage.Height;
			
			//MycLIENT = new MyClient(this);
		}

		//MyClient MycLIENT;
		private void button1_Click_1(object sender, EventArgs e)
		{
			//richTextBox1.Text = this.ToString();

			ParseAll();
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (!tcpClients.Connected)
			{
				label4.Text = "No connected";
				label4.ForeColor = Color.DarkRed;
			}
			else
			{
				label4.Text = "Connected";
				label4.ForeColor = Color.Lime;
			}
		}

		async private void button2_Click(object sender, EventArgs e)
		{
			byte[] BufferForSend = new byte[256];
			if (!string.IsNullOrWhiteSpace(richTextBox2.Text))
			{
				await Task.Run(() =>
				{
					Invoke(new Action(() => pictureBox3.Visible = true));
					Invoke(new Action(() => BufferForSend = Encoding.UTF8.GetBytes("▲TEXT▲" + richTextBox2.Text + "▲TEXT▲")));
					Invoke(new Action(() =>
					{
						if (UserToMessage.GetGeneralText.Length == 0)
						{
							UserToMessage.GetGeneralText = richTextBox2.Text;
							if (UserToMessage.DateUp)
							{
								UserToMessage.GetIPTextUp = "Для: " + ((IPEndPoint)tcpClients.Client.RemoteEndPoint).Address;
								UserToMessage.GetDateTimeTextUp = DateTime.Now.ToString();
							}
							else if (!UserToMessage.DateUp)
							{
								UserToMessage.GetIPTextDown = "Для: " + ((IPEndPoint)tcpClients.Client.RemoteEndPoint).Address;
								UserToMessage.GetDateTimeTextDown = DateTime.Now.ToString();
							}
						}
						else
						{
							CreateNewText(flowLayoutPanel1, richTextBox2.Text, "Для: " + ((IPEndPoint)tcpClients.Client.RemoteEndPoint).Address.ToString(), DateTime.Now, null);
						}
					}));

					tcpClients.GetStream().Write(BufferForSend, 0, BufferForSend.Length);
					Invoke(new Action(() => pictureBox3.Visible = false));
					Invoke(new Action(() =>
					{
						if (!string.IsNullOrWhiteSpace(richTextBox2.Text))
						{
							pictureBox2.Visible = true;
						}
						if (!timer2.Enabled)
						{
							timer2.Enabled = true;
						}
					}));
				});
			}
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			pictureBox2.Visible = false;
			pictureBox3.Visible = false;
			timer2.Enabled = false;
		}

		FileStream fileLog;
		string nameFile = "file.log";
		public void ReadLog(string StrToLog)
		{
			while (true)
			{
				try
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
					break;
				}
				catch (System.IO.IOException ex)
				{
					MessageBox.Show("Ошибки не будет, но лучше не запускай два и более клиента в одной директории");
				}
			}
		}
		string FileSend;
		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
		}

		string CurretNameFile;
		async void SendFile()
		{
			await Task.Run(() =>
			{
				FileStream filestream = File.OpenRead(FileSend);
				string LENGHTfileStr = filestream.Length.ToString();
				filestream.Dispose();
				filestream.Close();

				CurretNameFile = ParseNameFile();
				byte[] CheckNameCountFileByte = Encoding.UTF8.GetBytes(("▲FILE▲\"" + CurretNameFile + "\" " + LENGHTfileStr + "▲FILE▲"));

				string TestStr = Encoding.UTF8.GetString(CheckNameCountFileByte);
				Invoke(new Action(() => richTextBox3.Text = CurretNameFile));
				Invoke(new Action(() => richTextBox3.SelectionStart = richTextBox3.Text.Length));

				//MycLIENT.CurreTcpclient.GetStream().Write(CheckNameCountFileByte, 0, CheckNameCountFileByte.Length);
				Thread.Sleep(1);

				//byte[] FILEbyte = File.ReadAllBytes(FileSend);
				//for (int i = 0; i < FILEbyte.Length;)
				//{
				//MycLIENT.CurreTcpclient.GetStream().Write(FILEbyte, 0, 8192);
				//i += 8192;
				//}

				try
				{
					tcpClients.GetStream().Write(CheckNameCountFileByte, 0, CheckNameCountFileByte.Length);
					//Task.Delay(1000);
					tcpClients.Client.SendFile(FileSend, null, null, TransmitFileOptions.UseDefaultWorkerThread);
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
					Application.Restart();
				}
				//Thread.Sleep(500);
				//MycLIENT.CurreTcpclient.GetStream().Write(Encoding.UTF8.GetBytes("▼"), 0, 3);

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
		//private void pictureBox1_Click(object sender, EventArgs e)
		//{
		//	openFileDialog1.ShowDialog();
		//}
		private void richTextBox3_TextChanged(object sender, EventArgs e)
		{
			((RichTextBox)sender).Size = new Size(CurretNameFile.Length, ((RichTextBox)sender).Size.Height);
		}

		private void ClientGUITest2_Load(object sender, EventArgs e)
		{
			if (Directory.GetCurrentDirectory() == Environment.GetFolderPath(Environment.SpecialFolder.Desktop) && Directory.GetCurrentDirectory() == Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
			{
				this.Text += " — Рабочий стол";
			}
			else
			{
				this.Text += " — " + Directory.GetCurrentDirectory();
			}
			if (Properties.Settings.Default.SavedIP.Length > 0)
			{
				textBox1.Text = Properties.Settings.Default.SavedIP;
				textBox2.Text = Properties.Settings.Default.SavedPORT;
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				button4.Enabled = true;
				FileSend = openFileDialog1.FileName;

				richTextBox3.Text = FileSend;
				richTextBox3.SelectionStart = richTextBox3.TextLength;
				ActiveControl = richTextBox3;
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			SendFile();
			//button3.Visible = true;
		}

		string[] TEST;
		private void button3_DragDrop(object sender, DragEventArgs e)
		{
			TEST = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			button4.Enabled = true;
			FileSend = TEST[0];
			richTextBox3.Text = FileSend;
			richTextBox3.SelectionStart = richTextBox3.TextLength;
			ActiveControl = richTextBox3;
		}

		private void button3_DragEnter(object sender, DragEventArgs e)
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

		private void button3_DragLeave(object sender, EventArgs e)
		{
		}

		private void ClientGUITest2_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.Save();
		}

		public   string error = "Не удается прочитать данные из транспортного соединения: Попытка установить соединение была безуспешной," +
			" т.к. от другого компьютера за требуемое время не получен нужный отклик," +
			" или было разорвано уже установленное соединение из-за неверного отклика уже подключенного компьютера.";
		// ClientGUITest2 formmm1;
		 TcpClient tcpClients;
		 IPAddress iP;
		 int port;
		//public int Port
		//{
		//	get { return port; }
		//}
		//public IPAddress IP
		//{
		//	get { return iP; }
		//}
		//public TcpClient CurreTcpclient
		//{
		//	get
		//	{
		//		return tcpClients;
		//	}
		//}
		public  void ParseAll()
		{
			if (IPAddress.TryParse(textBox1.Text, out iP))
			{
				try
				{
					port = Convert.ToInt32(textBox2.Text);
					if (port > 0 && port < 65535)
					{
						ConnectToServer(iP, port);
					}
					else
					{
						Console.Beep(40, 300);
						timer1.Enabled = false;
					}
				}
				catch (FormatException)
				{
					Console.Beep(40, 300);
					timer1.Enabled = false;
				}
			}
			else
			{
				Console.Beep(40, 300);
				timer1.Enabled = false;
			}
		}
		async  public void ConnectToServer(IPAddress iP, int port)
		{
			TcpClient tcpClient = new TcpClient();

			await Task.Run(() =>
			{
				try
				{
					Invoke(new Action(() => label4.Visible = false));
					Invoke(new Action(() => pictureBox4.Visible = true));
					tcpClient.Connect(iP, port);

					ThreadAcceptMessage(tcpClient);
					Console.Beep(4000, 150);

					Properties.Settings.Default.SavedIP = textBox1.Text;
					Properties.Settings.Default.SavedPORT = textBox2.Text;
					Invoke(new Action(() => button3.Enabled = true));
					Invoke(new Action(() => textBox1.ReadOnly = true));
					Invoke(new Action(() => textBox2.ReadOnly = true));
					//Invoke(new Action(() => pictureBox1.Visible = true));
					Invoke(new Action(() => button2.Visible = true));
					Invoke(new Action(() => pictureBox4.Visible = false));
					Invoke(new Action(() => button1.Visible = false));
					Invoke(new Action(() => label4.Visible = true));
					Invoke(new Action(() => timer1.Enabled = true));
					Invoke(new Action(() => label4.Text = "Connected"));
					Invoke(new Action(() => label4.ForeColor = Color.Lime));
					Invoke(new Action(() => label4.Refresh()));
					tcpClients = tcpClient;
				}
				catch (SocketException)
				{
					Invoke(new Action(() => button3.Enabled = false));
					Invoke(new Action(() => pictureBox4.Visible = false));
					Invoke(new Action(() => button1.Visible = true));
					Invoke(new Action(() => label4.Text = "ERROR"));
					Invoke(new Action(() => label4.Visible = true));
					Invoke(new Action(() => label4.ForeColor = Color.Red));
					Invoke(new Action(() => label4.Refresh()));
					Invoke(new Action(() => timer1.Enabled = false));
					Console.Beep(40, 300);
					return;
				}
			});

			//Thread threadForAcceptSend = new Thread(ThreadAcceptSend);
			//threadForAcceptSend.IsBackground = true;
			//threadForAcceptSend.Start(tcpClient);
		}
		async public  void ThreadAcceptMessage(object StateInfo)
		{
			TcpClient tcpClient = (TcpClient)StateInfo;
			byte[] Buffer = new byte[256];
			string[] CheckNameCountFileStr;
			string CheckNameCountFileStr2;
			string StrBuffer;
			FileStream file1 = null;

			int CountByte;
			await Task.Run(() =>
			{
				try
				{
					while (true)
					{
						CheckNameCountFileStr = null;
						CheckNameCountFileStr2 = null;

						StrBuffer = null;
						CountByte = tcpClient.Client.Receive(Buffer, 0, Buffer.Length, SocketFlags.None);
						StrBuffer = Encoding.UTF8.GetString(Buffer, 0, CountByte);
						//CheckNameCountFileStr = StrBuffer.Split(' ');

						if (CountByte == 0 && StrBuffer.Length == 0 && Buffer[0] == 0)
						{
							break;
						}
						MatchCollection FileReceived = Regex.Matches(StrBuffer, "▲FILE▲");
						MatchCollection TextReceived = Regex.Matches(StrBuffer, "▲TEXT▲");

						string NameFileStr;
						int CountInt;
						string newBufferStr;
						const string AcceptedFiles = @"Accepted files\";

						if (StrBuffer.Length > 0 && Buffer.Length > 0 && FileReceived.Count == 2)
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
							byte[] newBuffer = Encoding.UTF8.GetBytes(newBufferStr);

							FileStream file2 = File.Create(AcceptedFiles + NameFileStr);
							//if (newBuffer.Length > FileReceived[1].Index + FileReceived[1].Length)
							if (newBuffer.Length > 0)
							{
								file2.Write(newBuffer, 0, newBuffer.Length);
								Invoke(new Action(() => textBox3.Text += " " + newBuffer.Length));
								Invoke(new Action(() => textBox3.SelectionStart = textBox3.Text.Length));
							}
							file2.Close();
							file1 = File.OpenWrite(AcceptedFiles + NameFileStr);

							bool bo = true;

							int CountIntWrite;
							int CountInTheFileInt = 0;
							Stopwatch stopwatch = new Stopwatch();
							byte[] BufferForFile = new byte[5242880];
							double Propgress1 = 0;
							while (file1.Length < CountInt)
							{
								CountIntWrite = tcpClient.Client.Receive(BufferForFile, 0, 5242880, SocketFlags.None);
								stopwatch.Start();
								Invoke(new Action(() => textBox3.Text += " " + CountIntWrite));
								Invoke(new Action(() => textBox3.SelectionStart = textBox3.Text.Length));

								file1.Write(BufferForFile, 0, CountIntWrite);

								CountInTheFileInt += CountIntWrite;
								file1.Close();
								Invoke(new Action(() =>
								{
									if (bo)
									{
										double aa = GetLastSeconds(CountInt, CountInTheFileInt, stopwatch.Elapsed.TotalSeconds);
										label7.Text = aa.ToString("0.0");
										bo = false;
									}
									if (GetKBPerSecond(CountInTheFileInt, stopwatch.Elapsed.TotalSeconds) >= 1000)
									{
										label5.Text = ((int)GetMBPerSecond(CountInTheFileInt, stopwatch.Elapsed.TotalSeconds)).ToString() + " МБ/с ꟷ " + ((GetMBInTheFile(CountInTheFileInt))).ToString("0.0") + "/" + ((CountInt / 1024.0 / 1024.0)).ToString("0.0") + " МБ, осталось " + (GetLastSeconds(CountInt, CountInTheFileInt, stopwatch.Elapsed.TotalSeconds)).ToString("0.0") + " сек.";
									}
									else
									{
										label5.Text = ((int)GetKBPerSecond(CountInTheFileInt, stopwatch.Elapsed.TotalSeconds)).ToString() + " КБ/с ꟷ " + ((GetMBInTheFile(CountInTheFileInt))).ToString("0.0") + "/" + ((CountInt / 1024.0 / 1024.0)).ToString("0.0") + " МБ, осталось " + (GetLastSeconds(CountInt, CountInTheFileInt, stopwatch.Elapsed.TotalSeconds)).ToString("0.0") + " сек.";
									}
									if (stopwatch.Elapsed.TotalMinutes >= 1)
									{
										label6.Text = "Минут прошло:" + stopwatch.Elapsed.TotalMinutes.ToString("0.0");
									}
									else if (stopwatch.Elapsed.TotalMinutes < 1)
									{
										label6.Text = "Секунд прошло:" + stopwatch.Elapsed.TotalSeconds.ToString("0.0");
									}
									//textBox6.Text = stopwatch.Elapsed.TotalSeconds.ToString("0.0"); // Секунд прошло
									//label6.Text = (CountInTheFileInt / 1024.0/* / 1024.0*/ / stopwatch.Elapsed.TotalSeconds).ToString("0000") + " КБ/с"; // Скорость в секунду MB/s
									//label5.Font = Control.DefaultFont;
									//label7.Text = " ꟷ " + ((/*CountInt - */CountInTheFileInt) / 1024.0 / 1024.0).ToString("0.000") + "/МБ,"; // Сколько осталось MB
									//label8.Text = "осталось " + ((int)(((CountInt - CountInTheFileInt) / 1024.0 / 1024.0) / (CountInTheFileInt / 1024.0 / 1024.0 / stopwatch.Elapsed.TotalSeconds))).ToString() + "сек."; // Сколько времени осталось
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
										tcpClient.GetStream().Write(BufferForError, 0, BufferForError.Length);
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
									MessageBox.Show("Отправка файла затянулась > 60 минут");
									file1.Close();
									break;
								}
								file1 = File.Open(AcceptedFiles + NameFileStr, FileMode.Append);
							}
							stopwatch.Stop();
							ReadLog("Файл принят: " + NameFileStr);
							//MessageBox.Show("Файл принят за " + stopwatch.Elapsed.Minutes.ToString() + " минут " + stopwatch.Elapsed.Seconds.ToString() + " секунд " + stopwatch.Elapsed.Milliseconds.ToString() + " миллисекунд");
							file1.Close();
						}
						else if (StrBuffer.Length > 0 && TextReceived.Count == 2)
						{
							CheckNameCountFileStr2 = StrBuffer.Remove(0, TextReceived[0].Index + TextReceived[0].Length);
							newBufferStr = CheckNameCountFileStr2.Remove(TextReceived[1].Index - TextReceived[0].Length, TextReceived[0].Length);

							Invoke(new Action(() => {
								if (!string.IsNullOrWhiteSpace(newBufferStr))
								{
									//UserFromMessage.GetGeneralText
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
										CreateNewText(flowLayoutPanel2, newBufferStr, "От " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address, DateTime.Now, null);
									}

									ReadLog(newBufferStr);
								}
							}));
						}
						else
						{
							Exception se = new Exception("KTOTO HE TTPABUJLHO UCTTOJLSYET TTPOrPAMMY");

							throw se;
						}
						Buffer = new byte[256];
					}
					MessageBox.Show("Сервер закрылся КОД:2");
					Application.Restart();
				}
				catch (Exception ex)
				{
					if (ex is IOException || ex is SocketException || ex.Message == "KTOTO HE TTPABUJLHO UCTTOJLSYET TTPOrPAMMY")
					{
						if (file1 != null)
						{
							file1.Close();
						}
						MessageBox.Show("Сервер закрылся КОД:1. сообщение:\n" + ex.Message);
						Application.Restart();
					}
					else
					{
						throw;
					}
				}

			});

			double GetMBPerSecond(int CountInTheFileInt, double TotalSeconds)
			{
				return (CountInTheFileInt / 1024.0 / 1024.0 / TotalSeconds);
			}
			double GetKBPerSecond(int CountInTheFileInt, double TotalSeconds)
			{
				return (CountInTheFileInt / 1024.0 / TotalSeconds);
			}
			double GetMBInTheFile(int CountInTheFileInt)
			{
				return ((CountInTheFileInt) / 1024.0 / 1024.0);
			}
			double GetKBInTheFile(int CountInTheFileInt)
			{
				return ((CountInTheFileInt) / 1024.0);
			}
			double GetLastSeconds(int CountInt, int CountInTheFileInt, double TotalSeconds)
			{
				return (int)(GetKBInTheFile(CountInt - CountInTheFileInt)) / GetKBPerSecond(CountInTheFileInt, TotalSeconds);
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
			//string ParseNameFile(string[] File)
			//{
			//	List<string> FileList = File.ToList();
			//	FileList.RemoveAt(0);
			//	FileList.RemoveAt(FileList.Count - 1);
			//	File = FileList.ToArray();

			//	string NameFile = "";
			//	for (int i = 0; i < File.Length; i++)
			//	{
			//		NameFile = string.Join(" ", File);
			//	}
			//	NameFile = NameFile.Remove(0, 1);
			//	NameFile = NameFile.Remove(NameFile.Length - 1, 1);
			//	return NameFile;
			//}
		}

		UserControl1[] UsersMessages = new UserControl1[341];
		int IndexForColor = 0;
		string[] AllColors;
		int ColorsSkips = 0;
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
		bool CheckColorNoBlack()
		{
			Color color1 = Color.FromName(AllColors[IndexForColor]);
			Color color2 = Color.FromArgb(255, 0, 0, 0);
			return color1.ToArgb() == color2.ToArgb();
		}
		Point Startpointpanel4;
		private void flowLayoutPanel2_ControlAdded(object sender, ControlEventArgs e)
		{
			if ((panel4.Location.Y < Startpointpanel4.Y + UserFromMessage.Size.Height) && !Startpointpanel4.IsEmpty)
			{
				panel4.Location = new Point(panel4.Location.X, panel4.Location.Y + UserFromMessage.Size.Height);
			}
			//tableLayoutPanel3.Size = new Size(tableLayoutPanel3.Size.Width, tableLayoutPanel3.Size.Height + UserFromMessage.Height);
		}

        private void button5_Click(object sender, EventArgs e)
        {
			CreateNewText(flowLayoutPanel1, string.Concat(Enumerable.Repeat("S", 255)), "Для: " + "255.255.255.255", DateTime.Now, null);
			CreateNewText(flowLayoutPanel2, string.Concat(Enumerable.Repeat("S", 255)), "От:  " + "255.255.255.255", DateTime.Now, null);
		}
    }
}