using Attendance.Common;
using Common;
using Contract.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Attendance.API
{
    public class APIAttController : ApiController
    {
        [HttpPost]
        [ActionName("QueryLeave")]
        public DataResult QueryLeave([FromBody]LeaveQuest obj)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            DBOA db = new DBOA();
            DataSet ds = db.ExeQuery(@"SELECT b.LASTNAME,xjsq5,xjsq19,xjsq9,xjsq10,xjsq17,c.NOWNODETYPE from 
(formtable_main_242 a left join HRMRESOURCE b on b.ID=a.xjsq5) 
left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
where xjsq10>=:StartDate and xjsq10<=:EndDate and b.LASTNAME like :LASTNAME and c.NOWNODETYPE=3 order by b.LASTNAME ASC",
                new OracleParameter("StartDate", obj.StartDate),
                new OracleParameter("EndDate", obj.EndDate),
                new OracleParameter("LASTNAME", "%" + obj.LASTNAME + "%"));
            db.Close();
            int n = ds.Tables[0].Rows.Count;
            List<LeaveQuest> list = new List<LeaveQuest>();
            for (int i = 0; i < n; i++)
            {
                LeaveQuest row = new LeaveQuest();
                row.LASTNAME = ds.Tables[0].Rows[i]["LASTNAME"].ToString();
                row.xjsq5 = ds.Tables[0].Rows[i]["xjsq5"].ToString();
                row.xjsq19 = ds.Tables[0].Rows[i]["xjsq19"].ToString();
                int type = -1;
                try
                {
                    type = int.Parse(ds.Tables[0].Rows[i]["xjsq9"].ToString());
                }
                catch (Exception)
                {
                    type = -1;
                }
                row.xjsq9 = TranLeaveType(type);
                row.xjsq10 = ds.Tables[0].Rows[i]["xjsq10"].ToString();
                row.xjsq17 = ds.Tables[0].Rows[i]["xjsq17"].ToString();
                row.NOWNODETYPE = int.Parse(ds.Tables[0].Rows[i]["NOWNODETYPE"].ToString());
                list.Add(row);

            }
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = list;
            return data;
        }
        public string TranLeaveType(int type)
        {
            if (type < 0)
            {
                return "";
            }
            string[] str = new string[] { "事假", "病假", "婚假", "产假", "丧假", "年休假", "其他", "陪产假" };
            return str[type];
        }
    }
}
