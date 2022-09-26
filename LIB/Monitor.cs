using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWM.LIB
{
    public class Timeframe
    {
        public string From { get; set; }
        public string To { get; set; }

        public bool IsInRage(string time)
        {
            return (time.CompareTo(From) >= 0 && time.CompareTo(To) <= 0);
        }
    }
    public class Monitor
    {
        //Thời gian chạy OK gần nhất, khởi tạo bằng ngày hôm trước vì hôm nay chưa chạy lần nào
        private DateTime _lastRunOk = DateTime.Now.AddDays(-2);
        public DateTime LastRunOK
        {
            get
            {
                return _lastRunOk;
            }
            set
            {
                //reset bộ đếm retry
                CountRetrying = 0;
                _lastRunOk = value;
            }
        }

        public DateTime LastFailed { get; set; } = DateTime.Now.AddDays(-2);

        private int CountRetrying = 0;

        private List<Timeframe> timeframes = null;

        /// <summary>
        /// Hàm kiểm tra xem woker đc phép chạy hay ko?
        /// </summary>
        /// <param name="runtimes"></param>
        /// <param name="maxRetryNum"></param>
        /// <param name="minutesBeforeRetrying"></param>
        /// <param name="reason">Rỗng nếu được chạy. Khác rỗng nếu không được chạy và là lý do vì sao không được chạy</param>
        /// <returns></returns>
        public string CheckCanStart(string runtimes, int maxRetryNum, int minutesBeforeRetrying, out string reason)
        {
            reason = null;

            var msg = GetTimeframes(runtimes, out timeframes);
            if (msg.Length > 0) return $"Check start error: {msg}";

            //check giờ chạy, nếu không config khung giờ chạy nghĩa là có thể chạy bất cứ lúc nào, còn nếu có config thì cần check xem giờ hiện tại có đc chạy ko
            msg = CheckRunningTime(out var tf, out var isRunningTime);
            if (msg.Length > 0) return $"Check running time error: {msg}";

            if (!isRunningTime) { reason = $"Chưa đến giờ chạy {runtimes}".ToInfoMessage(); return ""; }

            //nếu đc phép chạy và tf null có nghĩa là không config khung giờ chạy, vậy cho chạy đúng theo chu kỳ SpeedAdapter
            if (tf == null) { reason = "Không config khung giờ chạy, vậy cho chạy đúng theo chu kỳ SpeedAdapter".ToInfoMessage(); return ""; }

            //nếu khung giờ chạy thỏa mãn thì check xem đã chạy trong khung này OK lần nào chưa? nếu OK rồi thì thôi đợi khung giờ sau
            if (LastRunOK.Date == DateTime.Now.Date && tf.IsInRage(LastRunOK.ToString("HH:mm"))) { reason = $"Đã chạy OK lúc {LastRunOK:HH:mm dd-MM-yyyy}".ToInfoMessage(); return ""; }

            //nếu chưa chạy OK khung giờ này thì check xem đã quá số lần retry hay chưa? lần đầu chạy thì hiểu CountRetrying = 0
            if (CountRetrying >= maxRetryNum && LastFailed.Date == DateTime.Now.Date) { reason = $"Đã retried tối đa {maxRetryNum} lần cho khung giờ {tf.From}-{tf.To}".ToInfoMessage(); return ""; }

            //nếu chưa vượt quá số lần retry nhưng vẫn chưa nghỉ đủ lâu (để chờ hy vọng 1 điều gì đó thay đổi) thì cũng chưa được chạy
            if ((DateTime.Now - LastFailed).TotalMinutes < minutesBeforeRetrying) { reason = $"Sau khi failed, cần đợi {minutesBeforeRetrying} phút mới có thể chạy lại".ToInfoMessage(); return ""; }

            //nếu mọi thứ OK thì cho chạy, và tăng bộ đếm retry lên            
            CountRetrying++;

            return String.Empty;
        }
        public void UpdateResult(string msg)
        {
            if (msg.Length > 0)
            {
                if (LastFailed.Date < DateTime.Now.Date) CountRetrying = 1;
                LastFailed = DateTime.Now;
            }
            else
            {
                CountRetrying = 0;
                LastRunOK = DateTime.Now;
            }
        }
        public string GetTimeframes(string runtimes, out List<Timeframe> timeframes)
        {
            timeframes = new List<Timeframe>();
            try
            {
                string[] times = runtimes.Split(',');
                foreach (var time in times)
                {
                    string[] fromTo = time.Split('-');
                    timeframes.Add(new Timeframe() { From = fromTo[0], To = fromTo[1] });
                }
            }
            catch (Exception)
            {
                return $"Config giờ chạy {runtimes} chưa đúng định dạng HH:mm-HH:mm,HH:mm-HH:mm,...";
            }
            return String.Empty;
        }
        /// <summary>
        /// Hàm check giờ hiện tại có thuộc khung giờ chạy nào không? nếu có thì return kèm theo khung giờ hiện tại
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="isRunningTime"></param>
        /// <returns></returns>
        public string CheckRunningTime(out Timeframe tf, out bool isRunningTime)
        {
            tf = null;
            isRunningTime = true;

            //rỗng nghĩa là chạy bất cứ lúc nào
            if (timeframes.Count == 0) return "";

            string hourMinute = $"{DateTime.Now:HH:mm}";
            foreach (var item in timeframes)
            {
                if (item.IsInRage(hourMinute))
                {
                    tf = item;
                    return String.Empty;
                };
            }

            isRunningTime = false;
            return String.Empty;
        }
    }
}
