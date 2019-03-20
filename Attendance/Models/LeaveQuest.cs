using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contract.Models
{
    public class LeaveQuest
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string LASTNAME { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string xjsq5 { get; set; }
        /// <summary>
        /// 天数
        /// </summary>
        public string xjsq19 { get; set; }
        /// <summary>
        /// 0事假 1病假 2婚假 3产假 4丧假 5年休假 6其他 7陪产假
        /// </summary>
        public string xjsq9 { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string xjsq10 { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string xjsq17 { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NOWNODETYPE { get; set; }
    }
}