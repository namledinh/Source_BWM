using System;
using System.IO;

namespace BWM.LIB
{
    public class Settings
    {
        public static Settings CurrentSettings = null;

        public int DemoMode { get; set; } = 1;
        public int AutoStartMode { get; set; } = 1;
        public string ConnectionString_Main { get; set; } = "";
        public string ConnectionString_Demo { get; set; } = "";

        public string SettingsPassword { get; set; } = "Khongcopass";

        public string EmailsRootPath { get; set; } = @"C:\Email";
        public string MailHost { get; set; } = "bmail.bkav.com";
        public string MailUsernameEhd { get; set; } = "xxx@bkav.com";
        public string MailPasswordEhd { get; set; } = "xxx";
        public string MailCode { get; set; } = "xxx";

        public string RunningTimes { get; set; } = "08:00-10:59"; //tg chạy, from-to cách nhau bởi dấu , để rỗng nếu chạy cả ngày
        public int MaxRetryNum { get; set; } = 3; //Số lần retry tối đa nếu fail, tính cả lần chạy đầu
        public int MinutesBeforeRetrying { get; set; } = 5; //Thời gian (phút) nghỉ chờ retry lại sau mỗi lần fail 

        public string DomainApi_Get_Session { get; set; } = "https://sdss.bkav.com/rest/auth/1/session";
        public string DomainApi_Search_Project { get; set; } = "https://sdss.bkav.com/rest/api/2/search";
        public string DomainApi_Issue_Key { get; set; } = "https://sdss.bkav.com/rest/api/2/issue/";
        public string UserName { get; set; } = "admin_bcn";
        public string PassUser { get; set; } = "";
        public int MaxResults { get; set; } = 1000;
        public string Token_BEC { get; set; } = "QAVGWA5pofiUozte1SwW";
        public string Search_Issue_To_WEC { get; set; } = "project IN(TKIOC,BP,WEC,BMM) AND status not in (Closed, Cancelled, Done) AND (duedate < NOW() OR cf[10301] < NOW()) AND assignee IS NOT NULL";
        public string Link_Jira_SDSS { get; set; } = "https://sdss.bkav.com/browse/";
        public int Day_Add_Explain { get; set; } = 2;
        public string WEC_API { get; set; } = "https://devio.bkav.com:8903/api/Wec/AddOrUpdateWec";
        public string WEC_TOKEN { get; set; } = "C43DC3D4-5A72-467D-B58C-16B0B9CB9D1B";
        public string WEC_SYSTEM { get; set; } = "SDSS";
        public string WEC_HOLIDAY { get; set; } = "01/01;04/30;05/01;09/02";
        static public void InitCurrentSettings()
        {
            CurrentSettings = new Settings();
        }

        static public string ReadSettingsFromFile(string SettingsFilePath)
        {
            string msg = "";

            try
            {
                var encryptedData = File.ReadAllText(SettingsFilePath);
                var data = Utilities.Decrypt(encryptedData);

                msg = Utilities.JsonToObject(data, out CurrentSettings);
                if (msg.Length > 0) return msg;
            }
            catch (Exception ex)
            {
                msg = "ReadSettingsFromFile error: " + ex.ToString();
            }
            return msg;
        }
        static public string SaveSettingsToFile(string SettingsFilePath)
        {
            string msg = "";
            try
            {
                if (File.Exists(SettingsFilePath))
                    File.Move(SettingsFilePath, SettingsFilePath.Replace(".dat", DateTime.Now.ToString(".yyMMdd-HHmmss") + ".dat"));

                msg = Utilities.ObjectToJson(CurrentSettings, out string json);
                if (msg.Length > 0) return msg;

                var encryptedData = Utilities.Encrypt(json);
                File.WriteAllText(SettingsFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                msg = "SaveSettingsToFile error: " + ex.ToString();
            }
            return msg;
        }
    }
}
