using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class Gongchu
    {
        public string oa_uid { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string date { get; set; }
        public double day_count { get; set; }
        public string memo { get; set; }
        public string range { get; set; }
    }
    public class GongchuRequest
    {
        public string Token { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}