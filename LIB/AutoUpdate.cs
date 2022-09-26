using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BWM.LIB
{
    public partial class AutoUpdate : Form
    {
        public AutoUpdate()
        {
            InitializeComponent();
        }

        private void AutoUpdate_Load(object sender, EventArgs e)
        {
            CenterToParent();
            this.Top -= 100;
        }

        public void SetStatus(string s, int curr, int max)
        {
            downloadStatus.Text = s;
            pbDownloadStatus.Maximum = max;
            pbDownloadStatus.Value = curr;
        }
    }
}
