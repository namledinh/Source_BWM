using BWM.com.bkav.bni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWM.BSS
{
    static public class BNIHelper
    {
        static public string SendWarning(int packetWarningID, string smsContent, string emailContent, string bwcContent, string bwcContentDetail, int repeat)
        {
            string msg = "";
            try
            {
                AgentWS agentWS = new AgentWS();
                msg = agentWS.SendWarning(packetWarningID, smsContent, emailContent, bwcContent, bwcContentDetail, repeat);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return msg;
        }
    }
}
