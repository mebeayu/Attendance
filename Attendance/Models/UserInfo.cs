using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class UserInfo
    {
        public string Token { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string department_id { get; set; }
        public string department_path { get; set; }
    }
}