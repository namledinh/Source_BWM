using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BWM.LIB
{
    public delegate void UpdateUIDelegate();
    public delegate void UpdatebtnStartAll();
    public enum WorkerStatus
    {
        IsStopped = 0,
        IsRunning = 1,
        IsStopping = 2,
        IsCancelWaiting = 3
    }
    public partial class Worker : UserControl
    {
        public WorkerStatus WorkerStatus = WorkerStatus.IsStopped;
        public bool IsStopped { get { return WorkerStatus == WorkerStatus.IsStopped; } }
        public bool IsRunning { get { return WorkerStatus == WorkerStatus.IsRunning; } }
        public bool IsStopping { get { return WorkerStatus == WorkerStatus.IsStopping; } }
        public bool IsCancelWaiting { get { return WorkerStatus == WorkerStatus.IsCancelWaiting; } }
        public bool IsCancelling { get { return WorkerStatus == WorkerStatus.IsStopped || WorkerStatus == WorkerStatus.IsStopping; } }

        public string WorkerName { get; set; } = "WorkerName";
        UpdateUIDelegate ParentUpdateUI = null;
        Processor Processor;
        public WriteLogWithListenerDelegate BaseWriteLog { get; set; } = null;
        public Worker SetWorkerName(string WorkerName)
        {
            this.WorkerName = WorkerName;
            groupBox.Text = this.WorkerName;
            return this;
        }
        public Worker SetProcessor(Processor Processor)
        {
            this.Processor = Processor;

            UpdateUI_TB_Txt1_Txt2();

            return this;
        }
        public Worker SetWriteLog(WriteLogWithListenerDelegate WriteLog)
        {
            BaseWriteLog = WriteLog;
            return this;
        }
        public Worker SetUpdateUI(UpdateUIDelegate UpdateUI)
        {
            ParentUpdateUI = UpdateUI;
            return this;
        }

        public Worker()
        {
            InitializeComponent();
        }

        void BtnStartStop_Click(object sender, EventArgs e)
        {
            StartStop();
        }
        public void StartStop()
        {
            try
            {
                DoStartStop();
            }
            catch (Exception ex)
            {
                WriteLog(Utilities.LOG_ERROR + "StartStop Exception: " + ex.ToString());
            }
        }
        public async void DoStartStop()
        {
            if (IsRunning)
            {
                WorkerStatus = WorkerStatus.IsStopping;
            }
            else if (IsStopped)
            {
                WriteLog(string.Format("***** [{0}] Started ! *****", WorkerName));

                WorkerStatus = WorkerStatus.IsRunning;
                while (IsRunning)
                {
                    UpdateUI();

                    try
                    {
                        if (Processor != null) await Task.Run(() => Processor.Process());
                        else WriteLog($"ERROR: Processor == null");
                    }
                    catch (Exception ex)
                    {
                        WriteLog("DoStartStop Processor.Processor error: " + ex.ToString());
                    }

                    await Task.Run(() => Waiting());
                }
                WorkerStatus = WorkerStatus.IsStopped;

                WriteLog(string.Format("***** [{0}] Stopped ! *****", WorkerName));
            }
            UpdateUI();
        }

        public int SleepInterval = 100;
        void Waiting()
        {
            UpdateLabelStatus("Waiting...");

            UpdateUI_TB_Txt1_Txt2();

            var totalWaitingTime = Processor.SpeedAdapter.GetWaitingTime() * 1000;
            var NumOfIntervalUpdates = totalWaitingTime / SleepInterval;
            UpdatePBMinimum(0);
            UpdatePBMaximum(NumOfIntervalUpdates);
            for (int i = 0; i < NumOfIntervalUpdates; i++)
            {
                if (IsCancelling) { WriteLog("[Waiting] CancellationPending is true: return"); break; }
                if (IsCancelWaiting) { WorkerStatus = WorkerStatus.IsRunning; break; }

                UpdatePBCurrent(i);
                UpdateTxt3(((totalWaitingTime - i * SleepInterval) / 1000.0).ToString("0.0"));

                Thread.Sleep(SleepInterval);
            }
        }

        public void WriteLog(string log)
        {
            BaseWriteLog(log, WorkerName);
        }

        void UpdateUI_TB_Txt1_Txt2()
        {
            UpdateTBMinimum(0);
            UpdateTBMaximum(Processor.SpeedAdapter.GetMaxSpeed());
            UpdateTBCurrent(Processor.SpeedAdapter.currentSpeed);

            UpdateTxt1(Processor.SpeedAdapter.GetNumOfWorks());
            UpdateTxt2(Processor.SpeedAdapter.GetWaitingTime());
        }

        public void UpdateUI()
        {
            if (IsStopping)
            {
                lblStatus.Text = "Stopping...";
                btnStartStop.Image = global::BWM.Properties.Resources.start_icon;
                btnStartStop.Enabled = false;
                lblStatus.Visible = pb.Visible = txt3.Visible = true;
            }
            else if (IsRunning)
            {
                lblStatus.Text = "Running...";
                btnStartStop.Image = global::BWM.Properties.Resources.stop_icon;
                btnStartStop.Enabled = true;
                lblStatus.Visible = pb.Visible = txt3.Visible = true;
            }
            else
            {
                lblStatus.Text = "Not running";
                btnStartStop.Image = global::BWM.Properties.Resources.start_icon;
                btnStartStop.Enabled = true;
                lblStatus.Visible = pb.Visible = txt3.Visible = false;
            }

            Application.DoEvents();
        }

        public void UpdateTBCurrent(int i)
        {
            UpdateControl(tb, "Value" + i);
        }
        public void UpdateTBMinimum(int i)
        {
            UpdateControl(tb, "Minimum" + i);
        }
        public void UpdateTBMaximum(int i)
        {
            UpdateControl(tb, "Maximum" + i);
        }
        public void UpdateTxt1(object value)
        {
            UpdateTxt(txt1, value);
        }
        public void UpdateTxt2(object value)
        {
            UpdateTxt(txt2, value);
        }
        public void UpdateTxt3(object value)
        {
            UpdateTxt(txt3, value);
        }
        public void UpdateTxt(Control control, object value)
        {
            UpdateControl(control, value?.ToString());
        }

        public void UpdatePBCurrent(int i)
        {
            if (i > pb.Maximum) i = pb.Maximum;
            UpdateControl(pb, "Value" + i);
        }
        public void UpdatePBMinimum(int i)
        {
            UpdateControl(pb, "Minimum" + i);
        }
        public void UpdatePBMaximum(int i)
        {
            UpdateControl(pb, "Maximum" + i);
        }
        public void UpdateLabelStatus(string text)
        {
            UpdateControl(lblStatus, text);
        }

        private delegate void ControlDelegate(Control c, string text);
        void UpdateControl(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                var d = new ControlDelegate(UpdateControl);
                Invoke(d, new object[] { control, text });
                return;
            }
            if (control as TextBox != null)
            {
                var tb = control as TextBox;
                tb.Text = text;
            }
            else if (control as ProgressBar != null)
            {
                var pb = control as ProgressBar;
                int value = 0;
                if (ToInt(text, "Value", out value)) pb.Value = value;
                if (ToInt(text, "Minimum", out value)) pb.Minimum = value;
                if (ToInt(text, "Maximum", out value)) pb.Maximum = value;
            }
            else if (control as TrackBar != null)
            {
                var tb = control as TrackBar;
                int value = 0;
                if (ToInt(text, "Value", out value)) tb.Value = value;
                if (ToInt(text, "Minimum", out value)) tb.Minimum = value;
                if (ToInt(text, "Maximum", out value)) tb.Maximum = value;
            }
            else if (control as Label != null)
            {
                var lbl = control as Label;
                lbl.Text = text;
            }
        }
        bool ToInt(string text, string op, out int value)
        {
            value = 0;

            if (!text.StartsWith(op)) return false;

            if (!int.TryParse(text.Substring(op.Length), out value)) return false;

            return true;
        }

        private void tb_ValueChanged(object sender, EventArgs e)
        {
            Processor.SetSpeedAdapter(tb.Value);

            txt1.Text = Processor.SpeedAdapter.GetNumOfWorks().ToString();
            txt2.Text = Processor.SpeedAdapter.GetWaitingTime().ToString();
        }
        private void tb_MouseUp(object sender, MouseEventArgs e)
        {
            WorkerStatus = WorkerStatus.IsCancelWaiting;
            WriteLog(string.Format("Changed SA speed to {0} max-threads and wait {1} second(s)", Processor.SpeedAdapter.GetNumOfWorks(), Processor.SpeedAdapter.GetWaitingTime()));
        }
    }
}