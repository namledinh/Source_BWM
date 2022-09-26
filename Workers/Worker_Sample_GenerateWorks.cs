using BWM.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWM.Workers
{
    class Worker_Sample_GenerateWorks : Processor
    {
        Random random = new Random();
        public Worker_Sample_GenerateWorks(Worker Worker) : base(Worker)
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

        override public string DoProcess()
        {
            if (Worker_Sample_ProcessWorks.TotalWorks >= 100) // còn nhiều hơn 100 việc thì giảm tốc
            {
                if (SpeedAdapter.DecreaseSpeed()) WriteLog("<< SA speed: " + SpeedAdapter.GetCurrentSpeedString("works"));
                return "";
            }

            var numOfWorks = random.Next(30);
            Worker_Sample_ProcessWorks.TotalWorks += numOfWorks;

            WriteLog($"Generate more {numOfWorks} works. Total works now: {Worker_Sample_ProcessWorks.TotalWorks}");

            if (SpeedAdapter.IncreaseSpeed()) WriteLog(">> SA speed: " + SpeedAdapter.GetCurrentSpeedString("works"));

            return "";
        }
    }
}
