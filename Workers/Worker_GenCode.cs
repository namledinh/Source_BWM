using BWM.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BWM.Workers
{
    class Worker_GenCode : Processor
    {
        Random random = new Random();
        public Worker_GenCode(Worker Worker) : base(Worker)
        {
            Init();
        }
        override public void InitSpeedAdapter()
        {
            SpeedAdapter.AddMilestone(1, 100000);
        }

        override public string DoProcess()
        {
            FormGenCode fgc = new FormGenCode();
            fgc.ShowDialog();

            return "";
        }
    }
}
