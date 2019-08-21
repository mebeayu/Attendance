using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class Att
    {
        public string att_userid { get; set; }
        public string date_day { get; set; }  
        public DateTime first { get; set; }
        public DateTime last { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string first_str { get; set; }
        public string last_str { get; set; }
        public string week { get; set; }
        public string gongchu { get; set; }
        public bool late_tag { get; set; } 
        public bool early_tag { get; set; }
        public bool is_holiday { get; set; }
        public string range { get; set; }
        public string memo { get; set; }
    }
}