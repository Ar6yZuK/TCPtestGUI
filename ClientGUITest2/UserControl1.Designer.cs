
namespace ClientGUITest2
{
    public partial class UserControl1
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.GeneralText = new System.Windows.Forms.TextBox();
            this.DateTimeTextDown = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.IpTextDown = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.IpTextUp = new System.Windows.Forms.Label();
            this.DateTimeTextUp = new System.Windows.Forms.Label();
            this.GeneralPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.GeneralPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // GeneralText
            // 
            this.GeneralText.BackColor = System.Drawing.SystemColors.Control;
            this.GeneralText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GeneralText.Location = new System.Drawing.Point(0, 17);
            this.GeneralText.MaxLength = 256;
            this.GeneralText.Multiline = true;
            this.GeneralText.Name = "GeneralText";
            this.GeneralText.ReadOnly = true;
            this.GeneralText.Size = new System.Drawing.Size(413, 81);
            this.GeneralText.TabIndex = 0;
            // 
            // DateTimeTextDown
            // 
            this.DateTimeTextDown.AutoSize = true;
            this.DateTimeTextDown.Dock = System.Windows.Forms.DockStyle.Left;
            this.DateTimeTextDown.ForeColor = System.Drawing.Color.Navy;
            this.DateTimeTextDown.Location = new System.Drawing.Point(0, 0);
            this.DateTimeTextDown.Margin = new System.Windows.Forms.Padding(0);
            this.DateTimeTextDown.Name = "DateTimeTextDown";
            this.DateTimeTextDown.Size = new System.Drawing.Size(140, 17);
            this.DateTimeTextDown.TabIndex = 54;
            this.DateTimeTextDown.Text = "00.00.0000 00:00:00";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.IpTextDown);
            this.panel1.Controls.Add(this.DateTimeTextDown);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 98);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(413, 17);
            this.panel1.TabIndex = 55;
            // 
            // IpTextDown
            // 
            this.IpTextDown.AutoSize = true;
            this.IpTextDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.IpTextDown.ForeColor = System.Drawing.Color.Navy;
            this.IpTextDown.Location = new System.Drawing.Point(295, 0);
            this.IpTextDown.Name = "IpTextDown";
            this.IpTextDown.Size = new System.Drawing.Size(116, 17);
            this.IpTextDown.TabIndex = 56;
            this.IpTextDown.Text = "255.255.255.255";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.IpTextUp);
            this.panel2.Controls.Add(this.DateTimeTextUp);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(413, 17);
            this.panel2.TabIndex = 56;
            this.panel2.Visible = false;
            // 
            // IpTextUp
            // 
            this.IpTextUp.AutoSize = true;
            this.IpTextUp.Dock = System.Windows.Forms.DockStyle.Right;
            this.IpTextUp.ForeColor = System.Drawing.Color.Navy;
            this.IpTextUp.Location = new System.Drawing.Point(295, 0);
            this.IpTextUp.Name = "IpTextUp";
            this.IpTextUp.Size = new System.Drawing.Size(116, 17);
            this.IpTextUp.TabIndex = 55;
            this.IpTextUp.Text = "255.255.255.255";
            // 
            // DateTimeTextUp
            // 
            this.DateTimeTextUp.AutoSize = true;
            this.DateTimeTextUp.Dock = System.Windows.Forms.DockStyle.Left;
            this.DateTimeTextUp.ForeColor = System.Drawing.Color.Navy;
            this.DateTimeTextUp.Location = new System.Drawing.Point(0, 0);
            this.DateTimeTextUp.Margin = new System.Windows.Forms.Padding(0);
            this.DateTimeTextUp.Name = "DateTimeTextUp";
            this.DateTimeTextUp.Size = new System.Drawing.Size(140, 17);
            this.DateTimeTextUp.TabIndex = 54;
            this.DateTimeTextUp.Text = "00.00.0000 00:00:00";
            // 
            // GeneralPanel
            // 
            this.GeneralPanel.BackColor = System.Drawing.Color.MediumAquamarine;
            this.GeneralPanel.Controls.Add(this.GeneralText);
            this.GeneralPanel.Controls.Add(this.panel2);
            this.GeneralPanel.Controls.Add(this.panel1);
            this.GeneralPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralPanel.Location = new System.Drawing.Point(0, 0);
            this.GeneralPanel.Name = "GeneralPanel";
            this.GeneralPanel.Size = new System.Drawing.Size(413, 115);
            this.GeneralPanel.TabIndex = 57;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.GeneralPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(1000, 1000);
            this.MinimumSize = new System.Drawing.Size(413, 115);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(413, 115);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.GeneralPanel.ResumeLayout(false);
            this.GeneralPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label DateTimeTextDown;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label DateTimeTextUp;
        private System.Windows.Forms.Label IpTextDown;
        private System.Windows.Forms.Label IpTextUp;
        private System.Windows.Forms.TextBox GeneralText;
        private System.Windows.Forms.Panel GeneralPanel;
    }
}
