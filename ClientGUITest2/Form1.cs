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

namespace ClientGUITest2
{
	public partial class ClientGUITest2 : Form
	{
		public ClientGUITest2()
		{
			InitializeComponent();
			MycLIENT = new MyClient(this);
		}

		MyClient MycLIENT;
		private void button1_Click_1(object sender, EventArgs e)
		{
			//richTextBox1.Text = this.ToString();

			MyClient.ParseAll();
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (!MycLIENT.CurreTcpclient.Connected)
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

		byte[] BufferForSend = new byte[4092];
		async private void button2_Click(object sender, EventArgs e)
		{
			await Task.Run(() =>
			{
				Invoke(new Action(() => pictureBox3.Visible = true));
				Invoke(new Action(() => BufferForSend = Encoding.UTF8.GetBytes(richTextBox2.Text)));
				MycLIENT.CurreTcpclient.GetStream().Write(BufferForSend, 0, BufferForSend.Length);
				Invoke(new Action(() => pictureBox3.Visible = false));
				Invoke(new Action(() =>
				{
					if (richTextBox2.Text.Length > 0)
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
					MycLIENT.CurreTcpclient.GetStream().Write(CheckNameCountFileByte, 0, CheckNameCountFileByte.Length);
					//Task.Delay(1000);
	                MycLIENT.CurreTcpclient.Client.SendFile(FileSend, null, null, TransmitFileOptions.UseDefaultWorkerThread);
					ReadLog("Файл отправлен: " + CurretNameFile);
                }
                catch (SocketException)
                {
					ReadLog("Сбой при отправке файла: " + CurretNameFile);
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
			if(Properties.Settings.Default.SavedIP.Length > 0)
            {
				textBox1.Text = Properties.Settings.Default.SavedIP;
				textBox2.Text = Properties.Settings.Default.SavedPORT;
            }
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
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
    }
    class MyClient
	{
		public static string error = "Не удается прочитать данные из транспортного соединения: Попытка установить соединение была безуспешной," +
			" т.к. от другого компьютера за требуемое время не получен нужный отклик," +
			" или было разорвано уже установленное соединение из-за неверного отклика уже подключенного компьютера.";
		static ClientGUITest2 formmm1;
		static TcpClient tcpClients;
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
		public ClientGUITest2 FORM1
		{
			get
			{
				return formmm1;
			}
			protected set
			{
				formmm1 = value;
			}
		}
		public TcpClient CurreTcpclient
		{
			get
			{
				return tcpClients;
			}
		}
		public MyClient(ClientGUITest2 _form1)
		{
			FORM1 = _form1;
		}
		~MyClient()
		{
			if (tcpClients != null)
			{
				tcpClients.Close();
			}
		}
		public static void ParseAll()
		{
			if (IPAddress.TryParse(formmm1.textBox1.Text, out iP))
			{
				try
				{
					port = Convert.ToInt32(formmm1.textBox2.Text);
					if (port > 0 && port < 65535)
					{
						ConnectToServer(iP, port, formmm1);
					}
					else
					{
						Console.Beep(40, 300);
						formmm1.timer1.Enabled = false;
					}
				}
				catch (FormatException)
				{
					Console.Beep(40, 300);
					formmm1.timer1.Enabled = false;
				}
			}
			else
			{
				Console.Beep(40, 300);
				formmm1.timer1.Enabled = false;
			}
		}
		async static public void ConnectToServer(IPAddress iP, int port, ClientGUITest2 Form1)
		{
			TcpClient tcpClient = new TcpClient();

			await Task.Run(() =>
			{
				try
				{
					formmm1.Invoke(new Action(() => formmm1.label4.Visible = false));
					formmm1.Invoke(new Action(() => formmm1.pictureBox4.Visible = true));
					tcpClient.Connect(iP, port);
					ThreadAcceptSend(tcpClient);
					Console.Beep(4000, 150);

					Properties.Settings.Default.SavedIP = formmm1.textBox1.Text;
					Properties.Settings.Default.SavedPORT = formmm1.textBox2.Text;
					formmm1.Invoke(new Action(() => formmm1.button3.Enabled = true));
					formmm1.Invoke(new Action(() => formmm1.textBox1.ReadOnly = true));
					formmm1.Invoke(new Action(() => formmm1.textBox2.ReadOnly = true));
					//formmm1.Invoke(new Action(() => formmm1.pictureBox1.Visible = true));
					formmm1.Invoke(new Action(() => formmm1.button2.Visible = true));
					formmm1.Invoke(new Action(() => formmm1.pictureBox4.Visible = false));
					formmm1.Invoke(new Action(() => formmm1.button1.Visible = false));
					formmm1.Invoke(new Action(() => formmm1.label4.Visible = true));
					formmm1.Invoke(new Action(() => formmm1.timer1.Enabled = true));
					formmm1.Invoke(new Action(() => Form1.label4.Text = "Connected"));
					formmm1.Invoke(new Action(() => Form1.label4.ForeColor = Color.Lime));
					formmm1.Invoke(new Action(() => Form1.label4.Refresh()));
					tcpClients = tcpClient;
				}
				catch (SocketException)
				{
					formmm1.Invoke(new Action(() => formmm1.button3.Enabled = false));
					formmm1.Invoke(new Action(() => formmm1.pictureBox4.Visible = false));
					formmm1.Invoke(new Action(() => formmm1.button1.Visible = true));
					formmm1.Invoke(new Action(() => Form1.label4.Text = "ERROR"));
					formmm1.Invoke(new Action(() => formmm1.label4.Visible = true));
					formmm1.Invoke(new Action(() => Form1.label4.ForeColor = Color.Red));
					formmm1.Invoke(new Action(() => Form1.label4.Refresh()));
					formmm1.Invoke(new Action(() => formmm1.timer1.Enabled = false));
					Console.Beep(40, 300);
					return;
				}
			});

			//Thread threadForAcceptSend = new Thread(ThreadAcceptSend);
			//threadForAcceptSend.IsBackground = true;
			//threadForAcceptSend.Start(tcpClient);
		}

		async public static void ThreadAcceptSend(object StateInfo)
		{
			TcpClient tcpClient = (TcpClient)StateInfo;
			byte[] Buffer = new byte[256];
			string[] CheckNameCountFileStr;
			string StrBuffer;

			int CountByte;
			await Task.Run(() =>
			{
				try
				{
					while (true)
					{
						StrBuffer = null;
						CountByte = tcpClient.GetStream().Read(Buffer, 0, Buffer.Length);
						StrBuffer = Encoding.UTF8.GetString(Buffer, 0, CountByte);
						CheckNameCountFileStr = StrBuffer.Split(' ');

						if (CountByte == 0 && StrBuffer.Length == 0 && Buffer[0] == 0)
						{
							break;
						}
						string NameFileStr;
						int CountInt;

						if (Buffer.Length > 0 && Equals(CheckNameCountFileStr[0], "▲FILE▲"))
						{
							NameFileStr = ParseNameFile(CheckNameCountFileStr);
							CountInt = Convert.ToInt32(CheckNameCountFileStr[CheckNameCountFileStr.Length - 1]);

							string AcceptedFiles = @"Accepted files\";
							if (!Directory.Exists(AcceptedFiles))
							{
								Directory.CreateDirectory(AcceptedFiles);
							}
							FileStream file1 = File.Create(AcceptedFiles + NameFileStr);
							file1.Close();
							file1 = File.OpenWrite(AcceptedFiles + NameFileStr);

							int CountIntWrite;
							Stopwatch stopwatch = new Stopwatch();
							while (file1.Length < CountInt)
							{
								byte[] BufferForFile = new byte[CountInt];
								CountIntWrite = tcpClient.GetStream().Read(BufferForFile, 0, CountInt);
								stopwatch.Start();
								formmm1.Invoke(new Action(() => formmm1.textBox3.Text += " " + CountIntWrite));
								formmm1.Invoke(new Action(() => formmm1.textBox3.SelectionStart = formmm1.textBox3.Text.Length));
								file1.Write(BufferForFile, 0, CountIntWrite);

								if (stopwatch.Elapsed.TotalMinutes > 60)
								{
									MessageBox.Show("Отправка файла затянулась > 60 минут");
									break;
								}
							}
							stopwatch.Stop();
							formmm1.ReadLog("Файл принят: " + NameFileStr);
							MessageBox.Show("Файл принят за " + stopwatch.Elapsed.Minutes.ToString() + " минут " + stopwatch.Elapsed.Seconds.ToString() + " секунд " + stopwatch.Elapsed.Milliseconds.ToString() + " миллисекунд");
							file1.Close();
						}
						else
						{
							formmm1.ReadLog(StrBuffer);
							formmm1.Invoke(new Action(() => formmm1.richTextBox1.Text = StrBuffer));
						}
						Buffer = new byte[4092];
					}
					//MessageBox.Show("Сервер закрылся КОД:2");
					Application.Restart();
				}
				catch (System.IO.IOException ex)
				{
					Application.Restart();
					MessageBox.Show("Сервер закрылся КОД:1. сообщение:\n" + ex.Message);
				}

			});
			string ParseNameFile(string[] File)
			{
				List<string> FileList = File.ToList();
				FileList.RemoveAt(0);
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
		}
	}
}