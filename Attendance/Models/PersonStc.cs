using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Models
{
    public class Person
    {
        public string UID { get; set; }
        public string MOBILE { get; set; }
        public string LASTNAME { get; set; }
        public float LateCount { get; set; }
        public float EarlyCount { get; set; }
        public int Trip { get; set; }
        //0事假 1病假 2婚假 3产假 4丧假 5年休假 6其他 7陪产假
        public double Leave0 { get; set; }
        public double Leave1 { get; set; }
        public double Leave2 { get; set; }
        public double Leave3 { get; set; }
        public double Leave4 { get; set; }
        public double Leave5 { get; set; }
        public double Leave6 { get; set; }
        public double Leave7 { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}