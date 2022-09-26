using BWM.BSS;
using BWM.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWM.Workers
{
    class Worker_Sample_ProcessWorks : Processor
    {
        static public int TotalWorks = 0;
        public Worker_Sample_ProcessWorks(Worker Worker) : base(Worker)
        {
            Init();
        }
        override public void InitSpeedAdapter()
        {
            SpeedAdapter.AddMilestone(1, 30);
            SpeedAdapter.AddMilestone(1, 20);
            SpeedAdapter.AddMilestone(1, 10);
            SpeedAdapter.AddMilestone(2, 10);
            SpeedAdapter.AddMilestone(2, 8);
            SpeedAdapter.AddMilestone(3, 4);
            SpeedAdapter.AddMilestone(4, 2);
            SpeedAdapter.AddMilestone(5, 1);
        }

        static bool firstRun = true;
        override public string DoProcess()
        {
            if (TotalWorks == 0) // tổng số việc còn phải làm == 0 (hết việc)
            {
                if (SpeedAdapter.DecreaseSpeed()) WriteLog("<< SA speed: " + SpeedAdapter.GetCurrentSpeedString("works"));
                return "";
            }

            var numOfWorks = SpeedAdapter.GetNumOfWorks();
            if (numOfWorks > TotalWorks) numOfWorks = TotalWorks;

            WriteLog($"SA works/to process/in total]: {SpeedAdapter.GetNumOfWorks()} / {numOfWorks} / {TotalWorks}");

            // do work
            WriteLog($"Do {numOfWorks} works");
            TotalWorks -= numOfWorks;

            // Cảnh báo BNI
            if (firstRun)
            {
                firstRun = false;
                var msg = BNIHelper.SendWarning(1228, "Worker_Sample_ProcessWorks", "Worker_Sample_ProcessWorks", "Worker_Sample_ProcessWorks", "detail Worker_Sample_ProcessWorks", 1);
                if (msg.Length > 0) WriteErrorLog($"Error: BNIHelper.SendWarning: {msg}");
            }

            if (SpeedAdapter.IncreaseSpeed()) WriteLog(">> SA speed: " + SpeedAdapter.GetCurrentSpeedString("works"));

            return "";
        }
    }
}
