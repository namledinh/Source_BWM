using BWM.LIB;

namespace BWM
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStripMenuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemStopAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnCheckForUpdate = new System.Windows.Forms.Button();
            this.timerCheckUpdate = new System.Windows.Forms.Timer(this.components);
            this.lblAbout = new System.Windows.Forms.Label();
            this.tbStatusAll = new System.Windows.Forms.TextBox();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnStartAll = new System.Windows.Forms.Button();
            this.tabControlStatus = new System.Windows.Forms.TabControl();
            this.tabPageAll = new System.Windows.Forms.TabPage();
            this.tabPageErrors = new System.Windows.Forms.TabPage();
            this.tbStatusErrors = new System.Windows.Forms.TextBox();
            this.gbBkav = new System.Windows.Forms.GroupBox();
            this.wJiraToBEC = new BWM.LIB.Worker();
            this.pbDemo = new System.Windows.Forms.PictureBox();
            this.toolTipRestart = new System.Windows.Forms.ToolTip(this.components);
            this.cbAutoScroll = new System.Windows.Forms.CheckBox();
            this.btnStopAll = new System.Windows.Forms.Button();
            this.cbxInfoMode = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip.SuspendLayout();
            this.tabControlStatus.SuspendLayout();
            this.tabPageAll.SuspendLayout();
            this.tabPageErrors.SuspendLayout();
            this.gbBkav.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDemo)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenuItemAbout
            // 
            this.toolStripMenuItemAbout.Name = "toolStripMenuItemAbout";
            this.toolStripMenuItemAbout.Size = new System.Drawing.Size(115, 22);
            this.toolStripMenuItemAbout.Text = "&About";
            this.toolStripMenuItemAbout.Click += new System.EventHandler(this.ToolStripMenuItemAbout_Click);
            // 
            // toolStripMenuItemStatus
            // 
            this.toolStripMenuItemStatus.Name = "toolStripMenuItemStatus";
            this.toolStripMenuItemStatus.Size = new System.Drawing.Size(115, 22);
            this.toolStripMenuItemStatus.Text = "&Status";
            this.toolStripMenuItemStatus.Click += new System.EventHandler(this.ToolStripMenuItemStatus_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAbout,
            this.toolStripMenuItemStatus,
            this.toolStripMenuItemStopAll,
            this.toolStripSeparator1,
            this.toolStripMenuItemRestart,
            this.toolStripMenuItemExit});
            this.contextMenuStrip.Name = "contextMenuStripBAB";
            this.contextMenuStrip.Size = new System.Drawing.Size(116, 120);
            // 
            // toolStripMenuItemStopAll
            // 
            this.toolStripMenuItemStopAll.Name = "toolStripMenuItemStopAll";
            this.toolStripMenuItemStopAll.Size = new System.Drawing.Size(115, 22);
            this.toolStripMenuItemStopAll.Text = "Stop All";
            this.toolStripMenuItemStopAll.Click += new System.EventHandler(this.ToolStripMenuItemStopAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(112, 6);
            // 
            // toolStripMenuItemRestart
            // 
            this.toolStripMenuItemRestart.Name = "toolStripMenuItemRestart";
            this.toolStripMenuItemRestart.Size = new System.Drawing.Size(115, 22);
            this.toolStripMenuItemRestart.Text = "Restart";
            this.toolStripMenuItemRestart.Click += new System.EventHandler(this.ToolStripMenuItemRestart_Click);
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(115, 22);
            this.toolStripMenuItemExit.Text = "&Exit";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "eHDSM";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseUp);
            // 
            // btnCheckForUpdate
            // 
            this.btnCheckForUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCheckForUpdate.Location = new System.Drawing.Point(999, 646);
            this.btnCheckForUpdate.Margin = new System.Windows.Forms.Padding(0);
            this.btnCheckForUpdate.Name = "btnCheckForUpdate";
            this.btnCheckForUpdate.Size = new System.Drawing.Size(113, 30);
            this.btnCheckForUpdate.TabIndex = 1;
            this.btnCheckForUpdate.Text = "Check for Update";
            this.toolTipRestart.SetToolTip(this.btnCheckForUpdate, "Cannot restart. Please stop all workers");
            this.btnCheckForUpdate.UseVisualStyleBackColor = true;
            this.btnCheckForUpdate.Click += new System.EventHandler(this.btnCheckForUpdate_Click);
            // 
            // timerCheckUpdate
            // 
            this.timerCheckUpdate.Tick += new System.EventHandler(this.timerCheckUpdate_Tick);
            // 
            // lblAbout
            // 
            this.lblAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAbout.AutoSize = true;
            this.lblAbout.ForeColor = System.Drawing.Color.Blue;
            this.lblAbout.Location = new System.Drawing.Point(2155, 2);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(35, 13);
            this.lblAbout.TabIndex = 123;
            this.lblAbout.Text = "About";
            this.lblAbout.Click += new System.EventHandler(this.lblAbout_Click);
            // 
            // tbStatusAll
            // 
            this.tbStatusAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStatusAll.Location = new System.Drawing.Point(2, 2);
            this.tbStatusAll.Multiline = true;
            this.tbStatusAll.Name = "tbStatusAll";
            this.tbStatusAll.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbStatusAll.Size = new System.Drawing.Size(966, 594);
            this.tbStatusAll.TabIndex = 122;
            this.tbStatusAll.WordWrap = false;
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(1115, 646);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(90, 30);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnStartAll
            // 
            this.btnStartAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartAll.Image = global::BWM.Properties.Resources.start_icon;
            this.btnStartAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartAll.Location = new System.Drawing.Point(8, 646);
            this.btnStartAll.Margin = new System.Windows.Forms.Padding(0);
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(80, 30);
            this.btnStartAll.TabIndex = 0;
            this.btnStartAll.Text = "Start All";
            this.btnStartAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStartAll.UseVisualStyleBackColor = true;
            this.btnStartAll.Click += new System.EventHandler(this.btnStartAll_Click);
            // 
            // tabControlStatus
            // 
            this.tabControlStatus.Controls.Add(this.tabPageAll);
            this.tabControlStatus.Controls.Add(this.tabPageErrors);
            this.tabControlStatus.Location = new System.Drawing.Point(8, 16);
            this.tabControlStatus.Multiline = true;
            this.tabControlStatus.Name = "tabControlStatus";
            this.tabControlStatus.SelectedIndex = 0;
            this.tabControlStatus.Size = new System.Drawing.Size(978, 624);
            this.tabControlStatus.TabIndex = 149;
            // 
            // tabPageAll
            // 
            this.tabPageAll.Controls.Add(this.tbStatusAll);
            this.tabPageAll.Location = new System.Drawing.Point(4, 22);
            this.tabPageAll.Name = "tabPageAll";
            this.tabPageAll.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageAll.Size = new System.Drawing.Size(970, 598);
            this.tabPageAll.TabIndex = 0;
            this.tabPageAll.Text = "All";
            this.tabPageAll.UseVisualStyleBackColor = true;
            // 
            // tabPageErrors
            // 
            this.tabPageErrors.Controls.Add(this.tbStatusErrors);
            this.tabPageErrors.Location = new System.Drawing.Point(4, 22);
            this.tabPageErrors.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageErrors.Name = "tabPageErrors";
            this.tabPageErrors.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageErrors.Size = new System.Drawing.Size(970, 598);
            this.tabPageErrors.TabIndex = 3;
            this.tabPageErrors.Text = "Errors";
            this.tabPageErrors.UseVisualStyleBackColor = true;
            // 
            // tbStatusErrors
            // 
            this.tbStatusErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStatusErrors.Location = new System.Drawing.Point(2, 2);
            this.tbStatusErrors.Multiline = true;
            this.tbStatusErrors.Name = "tbStatusErrors";
            this.tbStatusErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbStatusErrors.Size = new System.Drawing.Size(966, 594);
            this.tbStatusErrors.TabIndex = 123;
            this.tbStatusErrors.WordWrap = false;
            // 
            // gbBkav
            // 
            this.gbBkav.Controls.Add(this.wJiraToBEC);
            this.gbBkav.Location = new System.Drawing.Point(991, 16);
            this.gbBkav.Name = "gbBkav";
            this.gbBkav.Size = new System.Drawing.Size(214, 620);
            this.gbBkav.TabIndex = 150;
            this.gbBkav.TabStop = false;
            this.gbBkav.Text = "BWM";
            // 
            // wJiraToBEC
            // 
            this.wJiraToBEC.BaseWriteLog = null;
            this.wJiraToBEC.Location = new System.Drawing.Point(6, 24);
            this.wJiraToBEC.Name = "wJiraToBEC";
            this.wJiraToBEC.Size = new System.Drawing.Size(200, 90);
            this.wJiraToBEC.TabIndex = 155;
            this.wJiraToBEC.WorkerName = "WorkerName";
            // 
            // pbDemo
            // 
            this.pbDemo.ErrorImage = null;
            this.pbDemo.Image = ((System.Drawing.Image)(resources.GetObject("pbDemo.Image")));
            this.pbDemo.InitialImage = null;
            this.pbDemo.Location = new System.Drawing.Point(171, 646);
            this.pbDemo.Name = "pbDemo";
            this.pbDemo.Size = new System.Drawing.Size(65, 30);
            this.pbDemo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbDemo.TabIndex = 152;
            this.pbDemo.TabStop = false;
            // 
            // toolTipRestart
            // 
            this.toolTipRestart.AutoPopDelay = 5000;
            this.toolTipRestart.InitialDelay = 50;
            this.toolTipRestart.ReshowDelay = 100;
            // 
            // cbAutoScroll
            // 
            this.cbAutoScroll.AutoSize = true;
            this.cbAutoScroll.Checked = true;
            this.cbAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoScroll.Location = new System.Drawing.Point(912, 646);
            this.cbAutoScroll.Name = "cbAutoScroll";
            this.cbAutoScroll.Size = new System.Drawing.Size(74, 17);
            this.cbAutoScroll.TabIndex = 153;
            this.cbAutoScroll.Text = "AutoScroll";
            this.cbAutoScroll.UseVisualStyleBackColor = true;
            // 
            // btnStopAll
            // 
            this.btnStopAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopAll.Image = global::BWM.Properties.Resources.stop_icon;
            this.btnStopAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStopAll.Location = new System.Drawing.Point(88, 646);
            this.btnStopAll.Margin = new System.Windows.Forms.Padding(0);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(80, 30);
            this.btnStopAll.TabIndex = 154;
            this.btnStopAll.Text = "Stop All";
            this.btnStopAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStopAll.UseVisualStyleBackColor = true;
            this.btnStopAll.Click += new System.EventHandler(this.BtnStopAll_Click);
            // 
            // cbxInfoMode
            // 
            this.cbxInfoMode.AutoSize = true;
            this.cbxInfoMode.Checked = true;
            this.cbxInfoMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxInfoMode.Location = new System.Drawing.Point(792, 646);
            this.cbxInfoMode.Name = "cbxInfoMode";
            this.cbxInfoMode.Size = new System.Drawing.Size(89, 17);
            this.cbxInfoMode.TabIndex = 155;
            this.cbxInfoMode.Text = "Info full mode";
            this.cbxInfoMode.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnStartAll;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1217, 687);
            this.Controls.Add(this.cbxInfoMode);
            this.Controls.Add(this.btnStopAll);
            this.Controls.Add(this.cbAutoScroll);
            this.Controls.Add(this.pbDemo);
            this.Controls.Add(this.gbBkav);
            this.Controls.Add(this.tabControlStatus);
            this.Controls.Add(this.btnStartAll);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.lblAbout);
            this.Controls.Add(this.btnCheckForUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BWM - Bkav Workers Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.contextMenuStrip.ResumeLayout(false);
            this.tabControlStatus.ResumeLayout(false);
            this.tabPageAll.ResumeLayout(false);
            this.tabPageAll.PerformLayout();
            this.tabPageErrors.ResumeLayout(false);
            this.tabPageErrors.PerformLayout();
            this.gbBkav.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDemo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAbout;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStatus;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Button btnCheckForUpdate;
        private System.Windows.Forms.Timer timerCheckUpdate;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.TextBox tbStatusAll;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnStartAll;
        private System.Windows.Forms.TabControl tabControlStatus;
        private System.Windows.Forms.TabPage tabPageAll;
        private System.Windows.Forms.GroupBox gbBkav;
        private System.Windows.Forms.PictureBox pbDemo;
        private System.Windows.Forms.ToolTip toolTipRestart;
        private System.Windows.Forms.CheckBox cbAutoScroll;
        private System.Windows.Forms.TabPage tabPageErrors;
        private System.Windows.Forms.TextBox tbStatusErrors;
        private System.Windows.Forms.Button btnStopAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStopAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRestart;
        private Worker wJiraToBEC;
        private System.Windows.Forms.CheckBox cbxInfoMode;
    }
}

