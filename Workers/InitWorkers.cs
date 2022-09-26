using BWM.LIB;
using BWM.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BWM
{
    public partial class MainForm : Form
    {
        public string InitWorkers()
        {
            var settings = Settings.CurrentSettings;

            InitWorker("JiraToWEC", wJiraToBEC, typeof(Work_Jira_To_Bec));

            return "";
        }
    }
}
