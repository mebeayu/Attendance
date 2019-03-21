using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class TokenObj
    {
        public string randomstr { get; set; }
        public string uid { get; set; }
        public string psw { get; set; }
        public string type { get; set; }
        public string department_id { get; set; }
        public string timestamp { get; set; }
    }
}