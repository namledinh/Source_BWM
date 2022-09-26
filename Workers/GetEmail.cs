using BWM.LIB;
using Chilkat;
using System;
using System.ComponentModel;

namespace BWM.Workers
{
    class GetEmail : Processor
    {
        private MailMan mailman;
        private string MailHost;
        private string Username;
        private string Password;

        private int NumberErrorEmail = 0;
        private int NumOfTotalEmailZero = 0;
        public GetEmail(Worker Worker, string mailHost, string username, string password) : base(Worker)
        {
            Init();

            MailHost = mailHost;
            Username = username;
            Password = password;

            InitMailMan();
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

        private void InitMailMan()
        {
            if (mailman != null)
            {
                mailman.Pop3EndSession();
                mailman.Dispose();
            }

            mailman = new MailMan();

            if (!mailman.UnlockComponent(Settings.CurrentSettings.MailCode))
            {
                WriteErrorLog("Cannot unlock Chilkat Component: " + mailman.LastErrorText);
                return;
            }
            mailman.MailHost = MailHost;
            mailman.PopUsername = Username;
            mailman.PopPassword = Password;
            mailman.MailPort = 110;// 7995;
            mailman.PopSsl = true;

            NumberErrorEmail = 0;
            NumOfTotalEmailZero = 0;

            WriteLog(string.Format("InitMailMan: MailHost {0}; PopUsername {1}", MailHost, Username));
        }

        override public string DoProcess()
        {
            if (mailman == null) return "mailman is null";
            mailman.ImmediateDelete = true;

            if (!mailman.Pop3BeginSession()) return "mailman.Pop3BeginSession error: " + mailman.LastErrorText;

            int totalEmails = mailman.GetMailboxCount();
            if (totalEmails < 0) return "mailman.GetMailboxCount error: " + mailman.LastErrorText;
            if (totalEmails == 0)
            {
                NumOfTotalEmailZero++;
                if (NumOfTotalEmailZero > 10)
                {
                    WriteLog("TotalEmails = 0 in " + NumOfTotalEmailZero + " times ==> Re-init new MailMan");
                    InitMailMan();
                }
                if (SpeedAdapter.DecreaseSpeed()) WriteLog("<< SA speed: " + SpeedAdapter.GetCurrentSpeedString("email"));
                return "";
            }

            var maxEmails = SpeedAdapter.GetNumOfWorks();
            var numEmails = totalEmails > maxEmails ? maxEmails : totalEmails;
            var mailBundles = mailman.GetHeaders(numEmails, NumberErrorEmail, NumberErrorEmail + numEmails);
            if (mailBundles == null)
            {
                if (mailman.LastErrorText.Contains("Socket connection closed.") || mailman.LastErrorText.Contains("No socket exists for sending") || mailman.LastErrorText.Contains("ConnectFailReason: Connection rejected"))
                {
                    WriteErrorLog("Socket connection closed ==> Re-init new MailMan. Error: " + mailman.LastErrorText);
                    InitMailMan();
                    return "";
                }
                return "mailBundles == null: " + mailman.LastErrorText;
            }

            WriteLog(string.Format("Total emails [to process/in mailbox]: {0}/{1}; Number of error emails: {2}", mailBundles.MessageCount, totalEmails, NumberErrorEmail));

            if (mailBundles.MessageCount == 0)
            {
                if (totalEmails <= NumberErrorEmail)
                {
                    if (SpeedAdapter.DecreaseSpeed()) WriteLog("<< SA speed: " + SpeedAdapter.GetCurrentSpeedString("email"));
                }
                else
                {
                    NumberErrorEmail++;
                    WriteErrorLog("Still has error emails. NumberErrorEmail = " + NumberErrorEmail);
                }
                return "";
            }

            try
            {
                for (int i = 1; i <= mailBundles.MessageCount; i++)
                {
                    var partialEmail = mailBundles.GetEmail(i - 1);
                    if (partialEmail == null)
                    {
                        WriteErrorLog(string.Format("mailBundles #{0}/{1}: mailBundles.GetEmail == null error: {2}", i, mailBundles.MessageCount, mailman.LastErrorText));
                        continue;
                    }
                    var email = mailman.GetFullEmail(partialEmail);
                    if (email == null)
                    {
                        WriteErrorLog(string.Format("mailBundles #{0}/{1}: mailman.GetFullEmail == null error: {2}", i, mailBundles.MessageCount, mailman.LastErrorText));
                        continue;
                    }

                    //string filename = ccsEmail.BuildFilename();
                    //bool saveOk = email.SaveEml(filename);
                    //if (!saveOk)
                    //{
                    //    WriteErrorLog(string.Format("mailBundles #{0}/{1}: email.SaveEml ToFile {2} FromAddress {3}; Subject {4}; GetEmailTypeString {5} error: {6}",
                    //        i, mailBundles.MessageCount, filename, ccsEmail.FromAddress, email.Subject, ccsEmail.GetEmailTypeString(), mailman.LastErrorText));
                    //    continue;
                    //}

                    //WriteLog(string.Format("mailBundles #{0}/{1}: Saved OK ToFile {2} FromAddress {3}; Subject {4}; GetEmailTypeString {5}",
                    //    i, mailBundles.MessageCount, filename, ccsEmail.FromAddress, email.Subject, ccsEmail.GetEmailTypeString()));

                    //if (!mailman.DeleteEmail(partialEmail))
                    //    WriteErrorLog(string.Format("mailBundles #{0}/{1}: mailman.DeleteEmail ToFile {2} FromAddress {3}; Subject {4}; GetEmailTypeString {5} error: {6}",
                    //        i, mailBundles.MessageCount, filename, ccsEmail.FromAddress, email.Subject, ccsEmail.GetEmailTypeString(), mailman.LastErrorText));
                }
                if (!mailman.Pop3EndSession()) return "Close POP3 Connection error: " + mailman.LastErrorText;

                if (SpeedAdapter.IncreaseSpeed()) WriteLog(">> SA speed: " + SpeedAdapter.GetCurrentSpeedString("email"));
            }
            catch (Exception ex)
            {
                return "Error when save to eml file: " + ex.ToString();
            }

            return "";
        }
    }
}