namespace ComercialSF_APILinker
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tmrRun = new System.Windows.Forms.Timer(this.components);
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.btnRun = new System.Windows.Forms.Button();
            this.lvSource = new System.Windows.Forms.ListView();
            this.lblMessage = new System.Windows.Forms.Label();
            this.tmrBar = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tmrTray = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrRun
            // 
            this.tmrRun.Interval = 5000;
            this.tmrRun.Tick += new System.EventHandler(this.tmrRun_Tick);
            // 
            // pBar
            // 
            this.pBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pBar.Location = new System.Drawing.Point(0, 614);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(916, 13);
            this.pBar.TabIndex = 0;
            // 
            // btnRun
            // 
            this.btnRun.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnRun.Location = new System.Drawing.Point(0, 585);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(916, 29);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Activar proceso";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Visible = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lvSource
            // 
            this.lvSource.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvSource.BackColor = System.Drawing.SystemColors.Control;
            this.lvSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvSource.FullRowSelect = true;
            this.lvSource.GridLines = true;
            this.lvSource.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSource.HideSelection = false;
            this.lvSource.Location = new System.Drawing.Point(0, 0);
            this.lvSource.Name = "lvSource";
            this.lvSource.Size = new System.Drawing.Size(916, 525);
            this.lvSource.TabIndex = 2;
            this.lvSource.UseCompatibleStateImageBehavior = false;
            this.lvSource.View = System.Windows.Forms.View.Details;
            // 
            // lblMessage
            // 
            this.lblMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblMessage.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblMessage.Location = new System.Drawing.Point(0, 528);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(916, 57);
            this.lblMessage.TabIndex = 7;
            // 
            // tmrBar
            // 
            this.tmrBar.Tick += new System.EventHandler(this.tmrBar_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // tmrTray
            // 
            this.tmrTray.Interval = 5000;
            this.tmrTray.Tick += new System.EventHandler(this.tmrTray_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 627);
            this.ControlBox = false;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lvSource);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.pBar);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cepech-APILinker";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrRun;
        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ListView lvSource;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Timer tmrBar;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer tmrTray;
    }
}