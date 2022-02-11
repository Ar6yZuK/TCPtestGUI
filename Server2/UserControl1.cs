using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace Server2
{
	public partial class UserControl1 : UserControl
	{

		[Description("Текст, связанный с элементом управления"), Category("Внешний вид")]
		public string GetGeneralText
		{
			get
			{
				return this.GeneralText.Text;
			}

			set
			{
				this.GeneralText.Text = value;
			}
		}
		[Description("Текст, связанный с элементом управления"), Category("Внешний вид")]
		public string GetIPTextUp
		{
			get
			{
				return this.IpTextUp.Text;
			}

			set
			{
				this.IpTextUp.Text = value;
			}
		}
		[Description("Текст, связанный с элементом управления"), Category("Внешний вид")]
		public string GetIPTextDown
		{
			get
			{
				return this.IpTextDown.Text;
			}

			set
			{
				this.IpTextDown.Text = value;
			}
		}
		[Description("Текст, связанный с элементом управления"), Category("Внешний вид")]
		public string GetDateTimeTextUp
		{
			get
			{
				return this.DateTimeTextUp.Text;
			}
			set
			{
				this.DateTimeTextUp.Text = value;
			}
		}
		[Description("Текст, связанный с элементом управления"), Category("Внешний вид")]
		public string GetDateTimeTextDown
		{
			get
			{
				return this.DateTimeTextDown.Text;
			}
			set
			{
				this.DateTimeTextDown.Text = value;
			}
		}
		void AddEventTest_Click(Delegate d)
		{
			//GeneralText.Click += d;
		}

		bool LastDateUp = false;
		[Description("Управляет местоположением Даты и Времени. Если true то сверху, если false то внизу"), Category("Внешний вид")]
		public bool DateUp
		{
			get
			{
				return LastDateUp;
			}
			set
			{
				if (value)
				{
					//S(value);
					
					LastDateUp = true;
					panel2.Visible = true;
					panel1.Visible = false;
					GeneralText.Location = new Point(GeneralText.Location.X, DateTimeTextDown.Size.Height);
				}
				else if (!value)
				{
					LastDateUp = false;
					panel2.Visible = false;
					panel1.Visible = true;
					GeneralText.Location = new Point(GeneralText.Location.X, 0);
				}
			}
		}
		[Description("Меняет цвет BackColor"), Category("Внешний вид")]
		public Color GetGeneralColor
		{
			get
			{
				return GeneralPanel.BackColor;
			}
			set
			{
				GeneralPanel.BackColor = value;
			}
		}

        public UserControl1()
		{
			InitializeComponent();
		}
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
			GeneralText.BackColor = GeneralPanel.BackColor;
			panel1.BackColor = GeneralPanel.BackColor;
			panel2.BackColor = GeneralPanel.BackColor;
        }
    }
}
