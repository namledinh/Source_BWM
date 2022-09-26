using BWM.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BWM.Workers
{
    class Worker_Covid : Processor
    {
        Random random = new Random();
        public Worker_Covid(Worker Worker) : base(Worker)
        {
            Init();
        }
        override public void InitSpeedAdapter()
        {
            SpeedAdapter.AddMilestone(1, 10);
        }

        override public string DoProcess()
        {
            //GetDataAPIHelper.APICall("http://ncov.moh.gov.vn/", out string html);
            //GetDataAPIHelper.APICall("https://ncov.vncdc.gov.vn/", out string html);
            GetDataAPIHelper.APICall("https://datastudio.google.com/embed/reporting/a5b8c6df-8e8c-4d09-bab3-3b211a22be18/page/71oIB", out string html);

            WriteLog(html);

            return "";
        }
    }
}
