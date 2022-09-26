using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BWM.LIB
{
    public delegate string DoWorkFunc(Work work);

    public class Processor
    {
        public Worker Worker;
        public SpeedAdapter SpeedAdapter { get; set; } = new SpeedAdapter(30);

        public int SleepInterval = 100;

        public WriteLogWithoutListenerDelegate WriteLog;
        public bool isWorking = false;

        public List<Work> Works = new List<Work>();

        public Processor(Worker Worker)
        {
            this.Worker = Worker;
            WriteLog = Worker.WriteLog;
        }

        public virtual void InitSpeedAdapter()
        {
            SpeedAdapter.AddMilestone(1, 600);
            SpeedAdapter.AddMilestone(1, 300);
            SpeedAdapter.AddMilestone(1, 120);
            SpeedAdapter.AddMilestone(1, 60);
            SpeedAdapter.AddMilestone(1, 30);
            SpeedAdapter.AddMilestone(1, 10);
            SpeedAdapter.AddMilestone(1, 5);
            SpeedAdapter.AddMilestone(1, 1);
        }
        public void SetSpeedAdapter(int speed)
        {
            SpeedAdapter.SetSpeed(speed);
        }

        public virtual void Init()
        {
            InitSpeedAdapter();
        }

        public virtual void Process()
        {
            var msg = DoProcess();
            if (msg.Length > 0) WriteErrorLog("Process error: " + msg);
        }
        public virtual string DoProcess()
        {
            return "";
        }

        public void DoWorks(DoWorkFunc DoWork, bool RunParallelly)
        {
            if (!RunParallelly)
            {
                foreach (Work work in Works) // chạy tuần tự
                {
                    var msg = DoWork(work);
                    if (msg.Length > 0) WriteErrorLog($"Error toDBWork: {work.WorkName}: {msg}"); // lỗi của toDBWork không ảnh hưởng các toDBWork khác, nên chỉ ghi log lỗi và vẫn tiếp tục
                }
            }
            else
            {
                var task = DoWorksAsync(DoWork, Works);
                task.Wait();
                if (task.Result.Length > 0) WriteErrorLog("Process error: " + task.Result);
            }
        }

        public async Task<string> DoWorksAsync(DoWorkFunc DoWork, IEnumerable works)
        {
            if (DoWork == null) return "DoWork == null";

            var cdWorkers = new ConcurrentDictionary<Work, Task<string>>();
            foreach (Work work in works)
            {
                var task = Task.Run(() => DoWork(work));
                cdWorkers.TryAdd(work, task);

                Worker.UpdateTxt1(cdWorkers.Count);
            }

            while (cdWorkers.Any())
                await WaitAndRemoveAWorker(cdWorkers);

            return "";
        }
        async Task<string> WaitAndRemoveAWorker(ConcurrentDictionary<Work, Task<string>> cdWorkers)
        {
            if (!cdWorkers.Any()) return "";

            var tasks = cdWorkers.Values.ToArray();
            var finishedTask = await Task.WhenAny(tasks);

            if (cdWorkers.Values.Any(x => x == finishedTask))
            {
                var result = await finishedTask;

                var work = cdWorkers.FirstOrDefault(x => x.Value == finishedTask).Key;
                if (result.Length > 0) WriteErrorLog($"Error at WorkName {work.WorkName}: {result}");

                cdWorkers.TryRemove(work, out var t);

                Worker.UpdateTxt1(cdWorkers.Count);
            }
            return "";
        }

        protected void WriteErrorLog(string log)
        {
            WriteLog(Utilities.LOG_ERROR + log);
        }
        protected string WriteLogAndReturnOk(string log)
        {
            WriteLog(log);
            return "";
        }
        protected string WriteLogAndReturnLog(string log)
        {
            WriteLog(log);
            return log;
        }
    }
    public class Work
    {
        public string WorkName;
    }
}
