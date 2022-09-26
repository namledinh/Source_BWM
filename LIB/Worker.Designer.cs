namespace BWM.LIB
{
    partial class Worker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.txt3 = new System.Windows.Forms.TextBox();
            this.tb = new System.Windows.Forms.TrackBar();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.txt2 = new System.Windows.Forms.TextBox();
            this.txt1 = new System.Windows.Forms.TextBox();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.pb);
            this.groupBox.Controls.Add(this.lblStatus);
            this.groupBox.Controls.Add(this.btnStartStop);
            this.groupBox.Controls.Add(this.txt3);
            this.groupBox.Controls.Add(this.tb);
            this.groupBox.Controls.Add(this.txt2);
            this.groupBox.Controls.Add(this.txt1);
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(200, 90);
            this.groupBox.TabIndex = 26;
            this.groupBox.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(6, 66);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(52, 13);
            this.lblStatus.TabIndex = 26;
            this.lblStatus.Text = "Waiting...";
            this.lblStatus.Visible = false;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Image = global::BWM.Properties.Resources.start_icon;
            this.btnStartStop.Location = new System.Drawing.Point(6, 19);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(46, 30);
            this.btnStartStop.TabIndex = 23;
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.BtnStartStop_Click);
            // 
            // txt3
            // 
            this.txt3.Location = new System.Drawing.Point(167, 63);
            this.txt3.Name = "txt3";
            this.txt3.ReadOnly = true;
            this.txt3.Size = new System.Drawing.Size(27, 20);
            this.txt3.TabIndex = 33;
            this.txt3.Text = "0";
            this.txt3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt3.Visible = false;
            // 
            // tb
            // 
            this.tb.Location = new System.Drawing.Point(58, 19);
            this.tb.Name = "tb";
            this.tb.Size = new System.Drawing.Size(103, 45);
            this.tb.TabIndex = 31;
            this.tb.ValueChanged += new System.EventHandler(this.tb_ValueChanged);
            this.tb.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tb_MouseUp);
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(64, 64);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(100, 18);
            this.pb.TabIndex = 32;
            this.pb.Visible = false;
            // 
            // txt2
            // 
            this.txt2.Location = new System.Drawing.Point(167, 41);
            this.txt2.Name = "txt2";
            this.txt2.ReadOnly = true;
            this.txt2.Size = new System.Drawing.Size(27, 20);
            this.txt2.TabIndex = 21;
            this.txt2.Text = "0";
            this.txt2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt1
            // 
            this.txt1.Location = new System.Drawing.Point(167, 19);
            this.txt1.Name = "txt1";
            this.txt1.ReadOnly = true;
            this.txt1.Size = new System.Drawing.Size(27, 20);
            this.txt1.TabIndex = 25;
            this.txt1.Text = "0";
            this.txt1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Worker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Name = "Worker";
            this.Size = new System.Drawing.Size(200, 90);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.TextBox txt3;
        private System.Windows.Forms.TrackBar tb;
        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.TextBox txt2;
        private System.Windows.Forms.TextBox txt1;
    }
}
