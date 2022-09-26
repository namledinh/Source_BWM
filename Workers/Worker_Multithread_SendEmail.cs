using BSS;
using BWM.LIB;
using Chilkat;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BWM.Workers
{
    /// <summary>
    /// Code chạy đa luồng, vd ở bài toán sau:
    /// Mỗi khi DoProcess, sẽ lấy ra N việc cần làm (vd 100).
    /// Nếu N việc này đều có thể thực hiện song song, tức là N việc có thể thực hiện độc lập với nhau (vd N email cần gửi; N hóa đơn cần ký số; N fcmid cần verify ở Google Firebase...), thì có thể dùng mô hình multithread trong ví dụ Worker_Multithread_SendEmail.
    /// Cơ bản mô hình này là spawn (sinh ra) M task (M < N, ví dụ M = 10). Đây chính là M thread chạy song song. Mỗi thread chịu trách nhiệm làm 1 task trong M task.
    /// Các thread này có thời gian thực hiện khác nhau: thread nào xong trước thì lại quay lại kho (kho có N việc) để lấy việc mới và làm.
    /// Như vậy thay vì tuần tự thực hiện từ 1 đến N việc; thì nay có M thread chạy song song, xử lý N việc. Cứ thread nào hoàn thành việc của mình thì lại lấy việc từ kho N việc ra làm tiếp; và như vậy thời gian xử lý tổng thể giảm xuống.
    /// Mô hình này đã áp dụng cho bài toán send email eHoadon rất hiệu quả. Có thể áp dụng cho các bài toán tương tự (vd cho DA BZ: verify fcmid trên GG Firebase)
    /// </summary>
    public class Worker_Multithread_SendEmail : Processor
    {
        private const int MaxTaskRunning = 10;

        public Worker_Multithread_SendEmail(Worker Worker) : base(Worker)
        {
            Init();
        }

        override public void InitSpeedAdapter()
        {
            SpeedAdapter.AddMilestone(1, 30);
            SpeedAdapter.AddMilestone(1, 20);
            SpeedAdapter.AddMilestone(5, 10);
            SpeedAdapter.AddMilestone(50, 9);
            SpeedAdapter.AddMilestone(50, 8);
            SpeedAdapter.AddMilestone(50, 7);
            SpeedAdapter.AddMilestone(50, 6);
            SpeedAdapter.AddMilestone(100, 5);
            SpeedAdapter.AddMilestone(100, 4);
            SpeedAdapter.AddMilestone(100, 3);
            SpeedAdapter.AddMilestone(100, 2);
            SpeedAdapter.AddMilestone(100, 1);
        }

        override public void Process()
        {
            Task<string> task = DoProcessAsync();
            task.Wait();
            if (task.Result.Length > 0) WriteErrorLog("Process error: " + task.Result);
        }
        static ConcurrentDictionary<long, Task<string>> cdWorkers = new ConcurrentDictionary<long, Task<string>>();
        private async Task<string> DoProcessAsync()
        {
            string msg;
            try
            {
                msg = DBM.ExecStore("sp_tblTempMailsSend_NumberOfMailNeedToSend", new { }, out int totalEmails);
                if (msg.Length > 0) return msg;

                var Max = SpeedAdapter.GetNumOfWorks();
                WriteLog("===== Number of emails [need to send/max to send]: " + totalEmails + "/" + Max);
                if (totalEmails <= 0)
                {
                    if (SpeedAdapter.DecreaseSpeed()) WriteLog("<< SA speed: " + SpeedAdapter.GetCurrentSpeedString("email"));
                    return "";
                }

                msg = DBM.ExecStore("sp_tblTempMailsSend_GetByTop", new { NumberOfMailToProcess = Max }, out DataTable dtListMailNeedToSend);
                if (msg.Length > 0) return msg;

                int CurrIndex = 0;
                string minEmailId = dtListMailNeedToSend.Rows[0]["Id"].ToString();
                string maxEmailId = dtListMailNeedToSend.Rows[dtListMailNeedToSend.Rows.Count - 1]["Id"].ToString();
                foreach (DataRow dr in dtListMailNeedToSend.Rows)
                {
                    long currId = long.Parse(dr["Id"].ToString());

                    if (Worker.IsCancelling) { WriteLog("[DoProcess] CancellationPending is true: return"); break; }

                    if (cdWorkers.ContainsKey(currId)) continue; // Da ton tai Id trong DS worker thi khong xu ly nua (continue)

                    var task = Task.Run(() => SendAnEmail(dr, dtListMailNeedToSend.Rows.Count, currId, minEmailId, maxEmailId, ++CurrIndex)); // Run Send an Email task ký //, currIndex
                    cdWorkers.TryAdd(currId, task);

                    if (cdWorkers.Count >= MaxTaskRunning)
                    { // Đã spawn tối đa số Worker, phải chờ 1 worker kết thúc thì mới tiếp tục
                        await WaitAndRemoveAWorker(cdWorkers);
                    }
                }

                while (cdWorkers.Any())
                    await WaitAndRemoveAWorker(cdWorkers);

                if (SpeedAdapter.IncreaseSpeed()) WriteLog(">> SA speed: " + SpeedAdapter.GetCurrentSpeedString("email"));
            }
            catch (Exception ex)
            {
                return "DoProcess Exception: " + ex.ToString();
            }
            return "";
        }

        async Task<string> WaitAndRemoveAWorker(ConcurrentDictionary<long, Task<string>> cdWorkers)
        {
            if (!cdWorkers.Any()) return "";

            var ts = cdWorkers.Values.ToArray();
            var finishedTask = await Task.WhenAny(ts);

            if (cdWorkers.Values.Any(x => x == finishedTask))
            {
                await finishedTask;
                var accountID = cdWorkers.FirstOrDefault(x => x.Value == finishedTask).Key;
                cdWorkers.TryRemove(accountID, out var t);

                Worker.UpdateTxt1(cdWorkers.Count);
            }
            return "";
        }

        private string SendAnEmail(DataRow dr, int numberOfEmail, long currId, string minEmailId, string maxEmailId, int CurrIndex)
        {
            var sendOk = true; // mMan.SendEmail(email);
            if (!sendOk)
            {
                var lastErrorText = "error"; // mMan.LastErrorText;
                WriteErrorLog("DoProcess SendEmail error: " + lastErrorText);
            }
            int SendTypeID = 0;
            int mailId = 0;

            if (sendOk)
            {
                WriteLog($"#{CurrIndex}/{numberOfEmail}: Sent OK: SendTypeID {SendTypeID}; mailId: {mailId}; To: {dr["Email"].ToString().Trim()}");
            }
            return "";
        }
    }
}
