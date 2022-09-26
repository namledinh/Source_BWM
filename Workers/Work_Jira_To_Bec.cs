using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BSS;
using BWM.com.bkav.bwss;
using BWM.Commons;
using BWM.LIB;
using BWM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BWM.Workers
{
    internal sealed class Work_Jira_To_Bec : Processor
    {
        LIB.Monitor monitor = new LIB.Monitor();
        public Work_Jira_To_Bec(Worker Worker) : base(Worker) { Init(); }
        public override void InitSpeedAdapter()
        {
            SpeedAdapter.AddMilestone(1, SpeedAdapter.WT_20Minute);
        }
        string vSession = "";
        public override string DoProcess()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return WriteLogAndReturnOk("Ngày chủ nhật không sinh cảnh báo");

            var msg = CheckHoliday();
            if (msg.Length > 0) return WriteLogAndReturnOk(msg);

            msg = monitor.CheckCanStart(Settings.CurrentSettings.RunningTimes, Settings.CurrentSettings.MaxRetryNum, Settings.CurrentSettings.MinutesBeforeRetrying, out string reason);
            if (msg.Length > 0) return WriteLogAndReturnLog(msg);

            if (!string.IsNullOrEmpty(reason)) return WriteLogAndReturnOk(reason);

            //DBM.ExecStore("ConnectionString", "SP trong DB",{Tham so của SP, }, out var data);
            
            WriteLog("DoProcess...");

            msg = DoWork();

            monitor.UpdateResult(msg);
            if (msg.Length > 0) return msg;

            WriteLog("DoProcess done!");

            return msg;
        }

        private string CheckHoliday()
        {
            var msg = "";
            foreach (var item in Settings.CurrentSettings.WEC_HOLIDAY.Split(';'))
            {
                msg = Convertor.StringToDatetime(DateTime.Now.Year + "/" + item, out DateTime holiday);
                if (DateTime.Now.Date == holiday.Date)
                {
                    msg = "Ngày nghỉ không sinh cảnh báo";
                    break;
                }
            }

            return msg;
        }

        private string DoWork()
        {
            var msg = GetValueSession();
            if (msg.Length > 0) return msg;

            msg = GetAllIssueProject();
            if (msg.Length > 0) return msg;

            //msg = GetIssuesKey();
            //if (msg.Length > 0) return msg;

            return "";
        }

        public string GetValueSession()
        {
            var url = Settings.CurrentSettings.DomainApi_Get_Session;
            var body = "{ 'username': '" + Settings.CurrentSettings.UserName + "', 'password': '" + Settings.CurrentSettings.PassUser + "'}";
            body = body.Replace("'", "\"");

            var msg = GetDataAPIHelper.PostCookieRequest(url, body, "", out string data);
            if (msg.Length > 0) return $"Get ValueSession error {msg}";

            msg = Convertor.JsonToObject(data, out APIResponse details_json);
            if (msg.Length > 0) return $"Lỗi convert session error {msg}";

            vSession = details_json.session.value.ToString();

            return msg;
        }

        public string GetAllIssueProject()
        {
            var url = Settings.CurrentSettings.DomainApi_Search_Project;
            var body = "{ 'jql':'" + Settings.CurrentSettings.Search_Issue_To_WEC + "','maxResults':'" + Settings.CurrentSettings.MaxResults + "'}";
            body = body.Replace("'", "\"");

            var msg = GetDataAPIHelper.PostCookieRequest(url, body, vSession, out string data);
            if (msg.Length > 0) return $"Get data issue error {msg}";

            msg = Convertor.JsonToObject(data, out APIResponse details_json);
            if (msg.Length > 0) return $"Lỗi convert session error {msg}";

            if (details_json == null || details_json.total == 0) return $"Dữ liệu api null";
            WriteLog($"Tổng số task cần giải trình {details_json.total}");

            msg = PostToBec(details_json);
            if (msg.Length > 0) return msg;

            return msg;
        }

        public string GetIssuesKey()
        {
            string key = "BWM-82";
            var url = Settings.CurrentSettings.DomainApi_Issue_Key + key;
            url = url.Replace("'", "");

            var msg = GetDataAPIHelper.GetCookieRequest(url, "", vSession, out string data);
            if (msg.Length > 0) return $"Get data issue error {msg}";

            msg = Convertor.JsonToObject(data, out APIResponseIssuesKey details_json);
            if (msg.Length > 0) return $"Lỗi convert session error {msg}";

            return msg;
        }

        public string PostToBec(APIResponse dataJira)
        {
            WebserviceBWSS webserviceBWSS = new WebserviceBWSS();
            var msg = "";

            foreach (var item in dataJira.issues)
            {
                if (Worker.IsCancelling) return "";

                string token = Settings.CurrentSettings.WEC_TOKEN;
                ModelWEC.WecInfoInput body = item.GetBodyWec(item.key, token);

                /*if (item.ToBEC(item.key) == null) { WriteErrorLog("Khởi tạo BEC không thành công"); continue; }
                var wBEC = item.ToBEC(item.key);*/

                //WriteLog($"Gửi cảnh báo dataKey {item.key} của {item.fields.assignee.name} sang WEC");

                //msg = InsertWec(body);
                //if (msg.Length > 0) { WriteLog($"InsertWec Error: {msg}"); continue; }
                //else 
                    WriteLog($"Gửi cảnh báo dataKey {item.key} của {item.fields.assignee.name} sang WEC thành công");

                /*WriteLog($"Gửi cảnh báo dataKey {item.key} của {wBEC.Account} sang BEC");
                ObjectResult obj = webserviceBWSS.CreateBECByAccount(wBEC);
                if (obj != null)
                {
                    msg = obj.ErrorMessage;
                    if (msg != null && msg.Length > 0) { WriteLog($"CreateBECByAccount {msg}"); continue; }
                }
                else
                {
                    WriteLog($"CreateBECByAccount không có giá trị trả về");
                }*/
                using (StreamWriter sw = new StreamWriter(@"C:\Users\NamLDC\App_BWM\NewBWM_1709\File_txt\Text.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + $"{item.key} của {item.fields.assignee.name} sang WEC thành công");
                }
            }

            return msg;
        }

        public string InsertWec(ModelWEC.WecInfoInput body)
        {
            string apiWec = Settings.CurrentSettings.WEC_API;

            var msg = GetDataAPIHelper.ApiCallPost(apiWec, "application/json", null, body, out string html);
            if (msg.Length > 0) return msg;

            msg = Convertor.JsonToObject(html, out ModelWEC.ApiResponse apiResponse);
            if (msg.Length > 0) return msg;
            msg = apiResponse.Object != null ? apiResponse.Object.ToString() : "";

            return msg;
        }

        public class APIResponse
        {
            public DataSession session { get; set; }
            public DataloginInfo loginInfo { get; set; }
            public string expand { get; set; }
            public string startAt { get; set; }
            public string maxResults { get; set; }
            public int total { get; set; }
            public List<DataIssuesResponse> issues { get; set; }
        }

        public class DataSession
        {
            public string name { get; set; }
            public string value { get; set; }
        }
        public class DataloginInfo
        {
            public string loginCount { get; set; }
            public string previousLoginTime { get; set; }
        }
        public class DataIssuesResponse
        {
            public string expand { get; set; }
            public int id { get; set; }
            public string self { get; set; }
            public string key { get; set; }
            public Datafields fields { get; set; }

            public ModelWEC.WecInfoInput GetBodyWec(string ID, string token)
            {
                string link_sdss = $"<a href='{Settings.CurrentSettings.Link_Jira_SDSS}{ID}' target='_blank'>Xem chi tiết trên {Settings.CurrentSettings.WEC_SYSTEM}</a>";
                var body = new ModelWEC.WecInfoInput
                {
                    WecInfo = new ModelWEC.WecInfo
                    {
                        Account = fields.assignee == null ? fields.reporter.name : fields.assignee.name,
                        Content = $"Task {fields.summary} quá hạn xử lý {link_sdss}",
                        DueDate = DateTime.Now.AddDays(Settings.CurrentSettings.Day_Add_Explain),
                        HandleType = CommonWEC.GetHandleType(Settings.CurrentSettings.WEC_SYSTEM, $"Cảnh báo từ hệ thống {Settings.CurrentSettings.WEC_SYSTEM}", ""),
                        IsApproved = true,
                        ModuleName = Settings.CurrentSettings.WEC_SYSTEM,
                        SourceId = 0,
                        StatusId = 1,
                        TypeName = $"Cảnh báo từ hệ thống {Settings.CurrentSettings.WEC_SYSTEM}"
                    },
                    Token = token
                };

                return body;
            }

            public BEC ToBEC(string ID)
            {
                try
                {
                    string link_sdss = $"<a href='{Settings.CurrentSettings.Link_Jira_SDSS}{ID}' target='_blank'>Xem chi tiết trên SDSS</a>";
                    BEC wBEC = new BEC()
                    {
                        Account = fields.assignee == null ? fields.reporter.name : fields.assignee.name,
                        LastTime = DateTime.Now.AddDays(Settings.CurrentSettings.Day_Add_Explain),
                        System = "SDSS",
                        SystemSource = fields.project.key,
                        Category = "Cảnh báo từ hệ thống SDSS",
                        TokenSystem = Settings.CurrentSettings.Token_BEC,
                        ContentViolateDetail = fields.description,
                        ContentViolate = $"Task {fields.summary} quá hạn xử lý {link_sdss}"
                    };
                    return wBEC;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }

        public class Datafields
        {
            public DataAssigneeReporter assignee { get; set; }
            public DataAssigneeReporter reporter { get; set; }
            public DataProject project { get; set; }
            public string description { get; set; }
            public string summary { get; set; }
            public DataIssuesKey IssueKey { get; set; }
        }
        public class DataProject
        {
            public string self { get; set; }
            public int id { get; set; }
            public string key { get; set; }
            public string name { get; set; }
            public string projectTypeKey { get; set; }
            public DataAvatarUrls avatarUrls { get; set; }
        }
        public class DataAssigneeReporter
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public string displayName { get; set; }
            public string active { get; set; }
            public string timeZone { get; set; }
            public DataAvatarUrls avatarUrls { get; set; }
        }

        public class DataAvatarUrls
        {
            [JsonProperty("48x48")]
            public string p48x48 { get; set; }
            [JsonProperty("24x24")]
            public string p24x24 { get; set; }
            [JsonProperty("16x16")]
            public string p16x16 { get; set; }
            [JsonProperty("32x32")]
            public string p32x32 { get; set; }
        }
        public class APIResponseIssuesKey
        {
            public string expand { get; set; }
            public int id { get; set; }
            public string self { get; set; }
            public DataIssuesKey fields { get; set; }
        }
        public class DataIssuesKey
        {
            public DataIssuetype issuetype { get; set; }
            public string timespent { get; set; }
            public string customfield_10200 { get; set; }
            public string customfield_10201 { get; set; }
            public string customfield_10202 { get; set; }
            public string customfield_10400 { get; set; }
            public string duedate { get; set; }
        }

        public class DataIssuetype
        {
            public string self { get; set; }
            public int id { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string subtask { get; set; }
        }

    }
}

