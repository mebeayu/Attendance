using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowService.Models
{
    public class GongchuForm
    {
        public string com { get; set; }
        public string owner { get; set; }
        /// <summary>
        /// 0=普通员工；1=部门经理
        /// </summary>
        public string person_type { get; set; }
        public string dpt { get; set; }
        /// <summary>
        /// 0=上午；1=下午；2=全天
        /// </summary>
        public string range { get; set; }
        public string date_time_gc { get; set; }
        public string reason { get; set; }
        public string addrss { get; set; }
        public string name { get; set; }
        public string Token { get; set; }
    }
}
