using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class LoginRequest
    {
        public string uid { get; set; }
        public string psw { get; set; }
    }
}