using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class Trip
    {
        public string Token { get; set; }
        public string UID { get; set; }
        public string LOGINID { get; set; }
        public string MOBILE { get; set; }
        public string REQUESTID { get; set; }
        public string LASTNAME { get; set; }
        public string Title { get; set; }//CC3
        public string Path { get; set; }//CC7
        public string StartDate { get; set; }//CC4
        public string EndDate { get; set; }//CC5
        public DateTime _StartDate { get; set; }//CC4
        public DateTime _EndDate { get; set; }//CC5
        public int Days { get; set; }
    }
}