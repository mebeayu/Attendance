using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class UserRel
    {
        public string oa_userid { get; set; }
        public string att_userid { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }
        public string oa_login_id { get; set; }
    }
}