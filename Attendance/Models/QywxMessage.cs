using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class text
    {
        public string content { get; set; }
    }
    public class QywxMessage
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        public string totag { get; set; }
        public string msgtype { get; set; }
        public int agentid { get; set; }
        public text text { get; set; }
        public int safe { get; set; }
        public int enable_id_trans { get; set; }
    }
}

