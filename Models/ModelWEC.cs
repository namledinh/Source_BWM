using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWM.Models
{
    public class ModelWEC
    {
        public class WecInfoInput
        {
            public WecInfo WecInfo { get; set; }
            public string Token { get; set; }
        }
        public class WecInfo
        {
            public string ModuleName { get; set; }

            public string TypeName { get; set; }

            public int SourceId { get; set; }

            public int StatusId { get; set; }

            public int HandleType { get; set; }

            public string Account { get; set; }

            public string Content { get; set; }

            public DateTime DueDate { get; set; }

            public bool IsApproved { get; set; }
        }
        public class ApiResponse
        {
            public int Status { get; set; }
            public object Object { get; set; }
            public bool IsOk { get; set; }
            public bool IsError { get; set; }
        }
    }
}
