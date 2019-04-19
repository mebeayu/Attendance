using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class PersonTripandLeaveDetail
    {
        public List<LeaveQuest> list_leave { get; set; }
        public List<Trip> list_trip { get; set; }
        public Person person { get; set; }
    }
}