using Attendance.Common;
using Attendance.Models;
using Common;
using Contract.Common;
using Contract.Models;
using Newtonsoft.Json;
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
        int code = 0;
        private static Dictionary<string, string> tokenCache = new Dictionary<string, string>();
        public static string MakeToken(string uid, string psw, string type, string department_id)
        {
            TokenObj obj = new TokenObj();
            obj.uid = uid;
            obj.psw = psw;
            obj.type = type;
            obj.department_id = department_id;
            obj.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            obj.randomstr = Guid.NewGuid().ToString().Replace("-", "");
            string strToken = JsonConvert.SerializeObject(obj);
            string Token = DES.EncryptDES(strToken);
            return Token;
        }
        public static TokenObj CheckToken(string Token, out int code)
        {
            code = MessageCode.UNKONWN;
            int token_last_day = 99999;
            TokenObj tokenObj = null;
            try
            {
                string token = DES.DecryptDES(Token);
                tokenObj = JsonConvert.DeserializeObject<TokenObj>(token);
            }
            catch (Exception ex)
            {
                code = MessageCode.ERROR_TOKEN_VALIDATE;
                return null;

            }
            //判断Token是否过期
            string date = tokenObj.timestamp;
            DateTime token_date = DateTime.Parse(date);
            TimeSpan sp = DateTime.Now - token_date;
            if (sp.Days > token_last_day)
            {
                code = MessageCode.ERROR_TOKEN_TIMEOUT;

            }
            //判断Token是否合法


            if (tokenCache.Keys.Contains(tokenObj.uid))
            {
                string oldToken = tokenCache[tokenObj.uid];
                if (oldToken == Token)
                {
                    code = MessageCode.SUCCESS;
                    return tokenObj;
                }
            }

            DBOA dboa = new DBOA();
            DataSet ds = dboa.ExeQuery("select LOGINID,PASSWORD,LASTNAME,MOBILE,DEPARTMENTID from HRMRESOURCE where LOGINID=:LOGINID",
                new OracleParameter("LOGINID", tokenObj.uid));
            dboa.Close();
            if (ds == null)
            {

                code = MessageCode.ERROR_EXECUTE_SQL;
                return null;
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                code = MessageCode.ERROR_TOKEN_VALIDATE;
                return null;

            }
            string md5PSW = MD5.GetMD5Hash(tokenObj.psw);
            if (md5PSW != ds.Tables[0].Rows[0]["PASSWORD"].ToString())
            {
                code = MessageCode.ERROR_TOKEN_VALIDATE;
                return null;
            }

            tokenCache[tokenObj.uid] = Token;

            code = MessageCode.SUCCESS;
            return tokenObj;
        }
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
        [HttpPost]
        [ActionName("QueryTrip")]
        public DataResult QueryTrip([FromBody]Trip obj)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            DBOA db = new DBOA();
            DataSet ds = db.ExeQuery(@"select b.LASTNAME,c.NOWNODETYPE,a.* from  
                (formtable_main_45 a left join  HRMRESOURCE b on (a.JBR=b.id) ) 
                left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
                where CC4>=:StartDate and CC4<=:EndDate and 
                b.LASTNAME like :LASTNAME and c.NOWNODETYPE=3 order by b.LASTNAME ASC",
                new OracleParameter("StartDate", obj.StartDate),
                new OracleParameter("EndDate", obj.EndDate),
                new OracleParameter("LASTNAME", "%" + obj.LASTNAME + "%"));
            
            int n = ds.Tables[0].Rows.Count;
            List<Trip> list = new List<Trip>();
            for (int i = 0; i < n; i++)
            {
                Trip trip = new Trip();
                trip.UID = ds.Tables[0].Rows[i]["JBR"].ToString();
                trip.LASTNAME = ds.Tables[0].Rows[i]["LASTNAME"].ToString();
                trip.Title = ds.Tables[0].Rows[i]["CC3"].ToString();
                trip.Path = ds.Tables[0].Rows[i]["CC6"].ToString();
                trip.REQUESTID = ds.Tables[0].Rows[i]["REQUESTID"].ToString();
                trip.StartDate = ds.Tables[0].Rows[i]["CC4"].ToString();
                trip.EndDate = ds.Tables[0].Rows[i]["CC5"].ToString();
                DateTime s = DateTime.Parse(trip.StartDate);
                DateTime e = DateTime.Parse(trip.EndDate);
                TimeSpan timeSpan = e - s;
                trip.Days = timeSpan.Days+1;
                list.Add(trip);
                string strIDS = ds.Tables[0].Rows[i]["CC8"].ToString();
                string[] arrIDs = strIDS.Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < arrIDs.Length; j++)
                {
                    if (arrIDs[j] == trip.UID) continue;
                    Trip trip1 = new Trip();
                    trip1.UID = arrIDs[j];
                    
                    DataSet ds1= db.ExeQuery($@"select LASTNAME from HRMRESOURCE where id={trip1.UID}");
                    trip1.LASTNAME = ds1.Tables[0].Rows[0][0].ToString() ;
                    trip1.Title = trip.Title;
                    trip1.Path = trip.Path;
                    trip1.REQUESTID = trip.REQUESTID;
                    trip1.StartDate = trip.StartDate;
                    trip1.EndDate = trip.EndDate;
                    trip1.Days = trip.Days;
                    list.Add(trip1);
                }

            }
            db.Close();
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
