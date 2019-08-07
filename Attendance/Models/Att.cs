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
    }
}