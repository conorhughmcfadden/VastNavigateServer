using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace VastNavigateServer
{
    partial class VastNavigateServer
    {
        private System.ComponentModel.IContainer components = null;
        private Timer lpsTimer;
        private Timer vastTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lpsTimer = new System.Windows.Forms.Timer(this.components);
            this.vastTimer = new System.Windows.Forms.Timer(this.components);
            this.buttonLaunch = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lpsTimer
            // 
            this.lpsTimer.Tick += new System.EventHandler(this.LPSTimer_Tick);
            // 
            // vastTimer
            // 
            this.vastTimer.Tick += new System.EventHandler(this.VASTTimer_Tick);
            // 
            // buttonLaunch
            // 
            this.buttonLaunch.Location = new System.Drawing.Point(12, 200);
            this.buttonLaunch.Name = "buttonLaunch";
            this.buttonLaunch.Size = new System.Drawing.Size(260, 23);
            this.buttonLaunch.TabIndex = 0;
            this.buttonLaunch.Text = "Launch LPS+VAST";
            this.buttonLaunch.UseVisualStyleBackColor = true;
            this.buttonLaunch.Click += new System.EventHandler(this.LaunchButton_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(12, 229);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(260, 23);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect to Navigate";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(13, 13);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(259, 181);
            this.textBox.TabIndex = 2;
            this.textBox.ScrollBars = ScrollBars.Vertical;
            this.textBox.WordWrap = true;
            this.textBox.ReadOnly = true;
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // VastNavigateServer
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.buttonLaunch);
            this.Name = "VastNavigateServer";
            this.Text = this.Name;
            this.Load += new System.EventHandler(this.VastNavigateServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Button buttonLaunch;
        private Button buttonConnect;
        private TextBox textBox;
    }
}

