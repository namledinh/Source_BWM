using BWM.LIB;
using BSS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace BWM
{
    public partial class MainForm : Form
    {
        const int UPDATECHECKINGINTERVAL = 3600000;   //=1000 * 60 * 60 * 1;  // Check new update for every 1 hour
        const string AppRootPath = @"C:\BWM";

        WorkerLogManagement WorkerLogManagement = new WorkerLogManagement();
        List<Worker> workers = new List<Worker>();

        string InitMsg = "";

        public MainForm()
        {
            InitializeComponent();

            InitMsg = Init();
            if (InitMsg.Length > 0) return;

            InitMsg = InitWorkers();
        }

        private string Init()
        {
            string msg = "";
            try
            {
                msg = Utilities.Init(WriteLog, AppRootPath);
                if (msg.Length > 0) return msg;

                if (!File.Exists(Utilities.SettingsFilePath))
                {
                    Settings.InitCurrentSettings();
                    WriteLog("Setting File [" + Utilities.SettingsFilePath + "] not found. Use default settings.");
                }
                else
                {
                    msg = Settings.ReadSettingsFromFile(Utilities.SettingsFilePath);
                    if (msg.Length > 0) return msg;
                    WriteLog("Read from Setting File [" + Utilities.SettingsFilePath + "] Ok.");
                }

                //msg = Utilities.InitDirectory(Settings.CurrentSettings.EmailsRootPath);
                //if (msg.Length > 0) return msg;

                timerCheckUpdate.Interval = 1000;
                timerCheckUpdate.Start();

                Text = Utilities.AssemblyTitle + " (v" + Assembly.GetExecutingAssembly().GetName().Version + ")";
                notifyIcon.Text = this.Text;

                bool isDemoMode = Settings.CurrentSettings.DemoMode != 0;
                DBM.ConnectionString = isDemoMode ? Settings.CurrentSettings.ConnectionString_Demo : Settings.CurrentSettings.ConnectionString_Main;

                WorkerLogManagement.tabControlStatus = tabControlStatus;
            }
            catch (Exception ex)
            {
                return $"Init error: {ex}";
            }
            return msg;
        }
        private Worker InitWorker(string workerName, Worker w, Type processorType = null)
        {
            Utilities.AddLoggingSettings(workerName);

            WorkerLogManagement.AddWorker(workerName);

            w.SetWorkerName(workerName).SetWriteLog(WriteLog).SetUpdateUI(UpdateUI);

            if (processorType != null)
            {
                var p = Activator.CreateInstance(processorType, w) as Processor;
                w.SetProcessor(p);
            }

            workers.Add(w);

            return w;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (InitMsg.Length > 0)
            {
                // có lỗi, thoát chương trình
                MessageBox.Show("MainForm error: " + InitMsg + "\r\n\r\nPress Ok to quit !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else if (Settings.CurrentSettings.AutoStartMode == 1) DoStartStopAll(true);
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.WindowsShutDown && e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                Hide();
            }
        }

        private delegate void UpdateUIDelegate();
        void UpdateUI()
        {
            if (InvokeRequired) { Invoke(new UpdateUIDelegate(UpdateUI), new object[] { }); return; }

            btnCheckForUpdate.Enabled = IsAllWorkerStopped();

            var tooltip = "";
            if (!btnCheckForUpdate.Enabled) tooltip = "Cannot Check for Update. Please stop all workers";
            toolTipRestart.SetToolTip(this.btnCheckForUpdate, tooltip);

            btnStartAll.Enabled = HasAWorkerStopped();
            btnStopAll.Enabled = toolStripMenuItemStopAll.Enabled = !IsAllWorkerStopped();
            toolStripMenuItemRestart.Enabled = IsAllWorkerStopped();

            tabPageErrors.Text = string.Format("Errors [{0}]", Utilities.NumOfErrorLogs);
        }
        bool HasAWorkerStopped()
        {
            foreach (var worker in workers) if (!worker.IsRunning) return true;
            return false;
        }
        bool IsAllWorkerStopped()
        {
            foreach (var worker in workers) if (!worker.IsStopped && worker.Enabled) return false;
            return true;
        }

        void WriteLog(string s)
        {
            WriteLog(s, "ALL");
        }
        void WriteLog(string s, string workerName)
        {
            if (!cbxInfoMode.Checked && s.IsInfoMessage()) return; // không write INFO log nếu cờ Full Info Mode không được bật

            TextBox tb = WorkerLogManagement?.GetWorkerTextBoxStatus(workerName);
            if (tb == null) tb = tbStatusAll;

            Utilities.WriteLogToTextBox(tb, tbStatusErrors, s, cbAutoScroll.Checked, workerName);
            UpdateUI();
        }

        private void btnStartAll_Click(object sender, EventArgs e)
        {
            DoStartStopAll(true);
        }
        private void BtnStopAll_Click(object sender, EventArgs e)
        {
            DoStartStopAll(false);
        }
        void DoStartStopAll(bool IsStartAll)
        {
            if (IsStartAll)
            {
                WriteLog("***** Start All *****");
                foreach (var worker in workers) if (!worker.IsRunning) worker.StartStop();
            }
            else
            {
                WriteLog("***** Stop All *****");
                foreach (var worker in workers) if (worker.IsRunning) worker.StartStop();
            }
            UpdateUI();
        }

        private void timerCheckUpdate_Tick(object sender, EventArgs e)
        {
            timerCheckUpdate.Interval = UPDATECHECKINGINTERVAL;
            timerCheckUpdate.Start();

            CheckForUpdate();
        }
        private void CheckForUpdate()
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, UPDATECHECKINGINTERVAL);
            WriteLog("Checking for update (checking interval: " + ts.ToString(@"dd\.hh\:mm\:ss") + ")...");

            Utilities.UpdateApplication();
        }
        private void btnCheckForUpdate_Click(object sender, EventArgs e)
        {
            CheckForUpdate();
        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            string value = "";
            if (InputBox.BoxTwoButton("Settings", "Password:", "OK", "Cancel", ref value) != DialogResult.OK) return;

            if (!value.Equals(Settings.CurrentSettings.SettingsPassword)) MessageBox.Show("Incorrect password !", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                SettingsForm sf = new SettingsForm(Utilities.SettingsFilePath);
                sf.ShowDialog();
            }
        }

        private void lblAbout_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }
        private void ToolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;

            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }
        private void ToolStripMenuItemStatus_Click(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }
        private void ToolStripMenuItemStopAll_Click(object sender, EventArgs e)
        {
            DoStartStopAll(false);
        }
        private void ToolStripMenuItemRestart_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            if (!IsAllWorkerStopped())
            {
                WriteLog("Cannot quit ! Please stop all worker before exit eHDSM.");
                return;
            }

            notifyIcon.Visible = false;
            Environment.Exit(0);
        }
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (WindowState == FormWindowState.Maximized || WindowState == FormWindowState.Normal)
            {
                Hide();
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        Control _currentToolTipControl = null;
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            Control control = GetChildAtPoint(e.Location);
            if (control != null)
            {
                if (!control.Enabled && _currentToolTipControl == null)
                {
                    string toolTipString = toolTipRestart.GetToolTip(control);
                    // trigger the tooltip with no delay and some basic positioning just to give you an idea
                    toolTipRestart.Show(toolTipString, control, control.Width / 2, control.Height / 2);
                    _currentToolTipControl = control;
                }
            }
            else
            {
                if (_currentToolTipControl != null) toolTipRestart.Hide(_currentToolTipControl);
                _currentToolTipControl = null;
            }
        }
    }
    class WorkerLogManagement
    {
        static ConcurrentDictionary<string, WorkerLog> WorkerLogs = new ConcurrentDictionary<string, WorkerLog>();
        public TabControl tabControlStatus;

        public void AddWorker(string workerName)
        {
            if (WorkerLogs.ContainsKey(workerName)) return;

            WorkerLogs.TryAdd(workerName, new WorkerLog(workerName, tabControlStatus));
        }
        public TextBox GetWorkerTextBoxStatus(string workerName)
        {
            if (!WorkerLogs.TryGetValue(workerName, out WorkerLog workerLog)) return null;

            return workerLog?.tbStatus;
        }
    }
    public class WorkerLog
    {
        public TabPage tabPage = new TabPage();
        public TextBox tbStatus = new TextBox();

        public WorkerLog(string workerName, TabControl tabControlStatus)
        {
            tabPage.Controls.Add(tbStatus);
            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = "tabPage" + workerName;
            tabPage.Padding = new System.Windows.Forms.Padding(2);
            tabPage.Size = new System.Drawing.Size(613, 623);
            //tabPage.TabIndex = 4;
            tabPage.Text = workerName;
            tabPage.UseVisualStyleBackColor = true;

            tbStatus.Dock = DockStyle.Fill;
            tbStatus.Location = new System.Drawing.Point(2, 2);
            tbStatus.Multiline = true;
            tbStatus.Name = "tbStatus" + workerName;
            tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            tbStatus.Size = new System.Drawing.Size(609, 619);
            //tbStatus.TabIndex = 124;
            tbStatus.WordWrap = false;

            tabControlStatus.Controls.Add(this.tabPage);
        }
    }
}

