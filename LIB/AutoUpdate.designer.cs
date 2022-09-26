namespace BWM.LIB
{
    partial class AutoUpdate
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
            downloadStatus = new System.Windows.Forms.Label();
            pbDownloadStatus = new System.Windows.Forms.ProgressBar();
            SuspendLayout();
            // 
            // downloadStatus
            // 
            downloadStatus.AutoSize = true;
            downloadStatus.Location = new System.Drawing.Point(12, 56);
            downloadStatus.Name = "downloadStatus";
            downloadStatus.Size = new System.Drawing.Size(43, 13);
            downloadStatus.TabIndex = 0;
            downloadStatus.Text = "Status: ";
            // 
            // pbDownloadStatus
            // 
            pbDownloadStatus.Location = new System.Drawing.Point(12, 30);
            pbDownloadStatus.Name = "pbDownloadStatus";
            pbDownloadStatus.Size = new System.Drawing.Size(471, 23);
            pbDownloadStatus.TabIndex = 1;
            // 
            // AutoUpdate
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(495, 93);
            Controls.Add(pbDownloadStatus);
            Controls.Add(downloadStatus);
            Icon = global::BWM.Properties.Resources.BWM;
            Name = "AutoUpdate";
            Text = "AutoUpdate";
            TopMost = true;
            Load += new System.EventHandler(AutoUpdate_Load);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label downloadStatus;
        private System.Windows.Forms.ProgressBar pbDownloadStatus;
    }
}