using Attendance.Common;
using Attendance.Models;
using Common;
using Attendance.Common;
using Attendance.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using WorkflowService.Models;
using WorkflowService;

namespace Attendance.API
{
    public class APIAttController : ApiController
    {
        
        int code = 0;
        private static Dictionary<string, string> tokenCache = new Dictionary<string, string>();
        public static string MakeToken(string oa_logid, string psw, string type, string department_id)
        {
            TokenObj obj = new TokenObj();
            obj.uid = oa_logid;
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
                if (tokenObj.psw!= ds.Tables[0].Rows[0]["PASSWORD"].ToString())
                {
                    code = MessageCode.ERROR_TOKEN_VALIDATE;
                    return null;
                }
               
            }

            tokenCache[tokenObj.uid] = Token;

            code = MessageCode.SUCCESS;
            return tokenObj;
        }
        [HttpPost]
        [ActionName("TakePointToOA")]
        public DataResult TakePointToOA([FromBody]LoginRequest obj)
        {
            DBOA dboa = new DBOA();
            DataSet ds = dboa.ExeQuery("select LOGINID,PASSWORD,LASTNAME,MOBILE,DEPARTMENTID from HRMRESOURCE where LOGINID=:LOGINID", new OracleParameter("LOGINID", obj.oa_login_id));
            dboa.Close();
            if (ds.Tables[0].Rows.Count==0)
            {
                DataResult r = DataResult.InitFromMessageCode(MessageCode.UNKONWN);
                r.message = "用户不存在";
                return r;
            }
            string LASTNAME = ds.Tables[0].Rows[0]["LASTNAME"].ToString();
            string MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
            string DEPARTMENTID = ds.Tables[0].Rows[0]["DEPARTMENTID"].ToString();
            string PASSWORD = ds.Tables[0].Rows[0]["PASSWORD"].ToString();
            DBHR dbhr = new DBHR();
            ds = dbhr.ExeQuery("select oa_dept_path from oa_dept where oa_dept_id=@oa_dept_id", new SqlParameter("oa_dept_id", DEPARTMENTID));
            dbhr.Close();
            string department_path = "";
            if (ds.Tables[0].Rows.Count > 0)
            {
                department_path = ds.Tables[0].Rows[0][0].ToString();
            }
            DB db = new DB();
            ds = db.ExeQuery("select * from login_user where uid=@uid", new SqlParameter("uid", obj.oa_login_id));
            string type = "0";
            if (ds.Tables[0].Rows.Count == 0)
            {
                int res = db.ExeCMD(@"insert into login_user(uid,name,mobile,department_id,department_path) 
                                    values(@uid,@name,@mobile,@department_id,@department_path)",
                                    new SqlParameter("uid", obj.oa_login_id),
                                    new SqlParameter("name", LASTNAME),
                                    new SqlParameter("mobile", MOBILE),
                                    new SqlParameter("department_id", DEPARTMENTID),
                                    new SqlParameter("department_path", department_path));
            }
            else
            {
                type = ds.Tables[0].Rows[0]["type"].ToString();
                int res = db.ExeCMD("update login_user set name=@name,mobile=@mobile,department_id=@department_id,department_path=@department_path where uid=@uid",
                                    new SqlParameter("uid", obj.oa_login_id),
                                    new SqlParameter("name", LASTNAME),
                                    new SqlParameter("mobile", MOBILE),
                                    new SqlParameter("department_id", DEPARTMENTID),
                                    new SqlParameter("department_path", department_path));
            }
            db.Close();
            DataResult return_data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            string Token = MakeToken(obj.oa_login_id, PASSWORD, type, DEPARTMENTID);
            UserInfo u = new UserInfo();
            u.Token = Token;
            u.name = LASTNAME;
            u.type = type;
            u.department_id = DEPARTMENTID;
            u.department_path = department_path;
            u.oa_login_id = obj.oa_login_id;
            tokenCache[obj.oa_login_id] = Token;
            return_data.data = u;
            return return_data;

        }
        [HttpPost]
        [ActionName("Login")]
        public DataResult Login([FromBody]LoginRequest obj)
        {

            DBOA dboa = new DBOA();
            DataSet ds = dboa.ExeQuery("select LOGINID,PASSWORD,LASTNAME,MOBILE,DEPARTMENTID from HRMRESOURCE where LOGINID=:LOGINID", new OracleParameter("LOGINID", obj.oa_login_id));
            dboa.Close();
            if (ds == null) return DataResult.InitFromMessageCode(MessageCode.ERROR_EXECUTE_SQL);
            if (ds.Tables[0].Rows.Count == 0) return DataResult.InitFromMessageCode(MessageCode.ERROR_NO_DATA);
            string md5PSW = MD5.GetMD5Hash(obj.psw);
            if (md5PSW != ds.Tables[0].Rows[0]["PASSWORD"].ToString()) return DataResult.InitFromMessageCode(MessageCode.ERROR_PASSWORD);
            string LASTNAME = ds.Tables[0].Rows[0]["LASTNAME"].ToString();
            string MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
            string DEPARTMENTID = ds.Tables[0].Rows[0]["DEPARTMENTID"].ToString();
            DBHR dbhr = new DBHR();
            ds = dbhr.ExeQuery("select oa_dept_path from oa_dept where oa_dept_id=@oa_dept_id", new SqlParameter("oa_dept_id", DEPARTMENTID));
            dbhr.Close();
            string department_path = "";
            if (ds.Tables[0].Rows.Count > 0)
            {
                department_path = ds.Tables[0].Rows[0][0].ToString();
            }
            DB db = new DB();
            ds = db.ExeQuery("select * from login_user where uid=@uid", new SqlParameter("uid", obj.oa_login_id));
            string type = "0";
            if (ds.Tables[0].Rows.Count == 0)
            {
                int res = db.ExeCMD(@"insert into login_user(uid,name,mobile,department_id,department_path) 
                                    values(@uid,@name,@mobile,@department_id,@department_path)",
                                    new SqlParameter("uid", obj.oa_login_id),
                                    new SqlParameter("name", LASTNAME),
                                    new SqlParameter("mobile", MOBILE),
                                    new SqlParameter("department_id", DEPARTMENTID),
                                    new SqlParameter("department_path", department_path));
            }
            else
            {
                type = ds.Tables[0].Rows[0]["type"].ToString();
                int res = db.ExeCMD("update login_user set name=@name,mobile=@mobile,department_id=@department_id,department_path=@department_path where uid=@uid",
                                    new SqlParameter("uid", obj.oa_login_id),
                                    new SqlParameter("name", LASTNAME),
                                    new SqlParameter("mobile", MOBILE),
                                    new SqlParameter("department_id", DEPARTMENTID),
                                    new SqlParameter("department_path", department_path));
            }
            db.Close();
            DataResult return_data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            string Token = MakeToken(obj.oa_login_id, obj.psw, type, DEPARTMENTID);
            UserInfo u = new UserInfo();
            u.Token = Token;
            u.name = LASTNAME;
            u.type = type;
            u.department_id = DEPARTMENTID;
            u.department_path = department_path;
            tokenCache[obj.oa_login_id] = Token;
            return_data.data = u;
            return return_data;
        }
        [HttpPost]
        [ActionName("QueryLeave")]
        public DataResult QueryLeave([FromBody]LeaveQuest obj)
        {
            TokenObj tokenObj = CheckToken(obj.Token, out code);
            if (code != MessageCode.SUCCESS)
            {
                return DataResult.InitFromMessageCode(code);
            }
            
            List<LeaveQuest> list = AttBiz.QueryLeave(obj, tokenObj);
           
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                list[i].xjsq9 = TranLeaveType(int.Parse(list[i].xjsq9));
            }
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = list;
            return data;
        }
        [HttpPost]
        [ActionName("QueryTrip")]
        public DataResult QueryTrip([FromBody]Trip obj)
        {
            TokenObj tokenObj = CheckToken(obj.Token, out code);
            if (code != MessageCode.SUCCESS)
            {
                return DataResult.InitFromMessageCode(code);
            }
            //AttBiz attbiz = new AttBiz();
            List<Trip> list = AttBiz.QueryTrip(obj, tokenObj);
            //attbiz.Close();
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = list;
            return data;
        }
        [HttpPost]
        [ActionName("QueryAttList")]
        public DataResult QueryAttList([FromBody]Person obj)
        {
            //TokenObj tokenObj = CheckToken(obj.Token, out code);
            //if (code != MessageCode.SUCCESS)
            //{
            //    return DataResult.InitFromMessageCode(code);
            //}
            //AttBiz attbiz = new AttBiz();
            //Trip t = new Trip();
            //t.StartDate = obj.StartDate;
            //t.EndDate = obj.EndDate;
            //t.UID = obj.UID;
            //t.LASTNAME = "";
            //List<Trip> list_trip = AttBiz.QueryTrip(t);
            //List<string> arrUID = new List<string>();
            //if (tokenObj.type == "0")
            //{
            //    string oa_id = attbiz.GetOAUIDByLoginID(tokenObj.uid);
            //    if (oa_id != null) arrUID.Add(oa_id);
            //}
            //else
            //{
            //   arrUID = attbiz.GetUIDinDate(obj.StartDate, obj.EndDate, list_trip);
            //}
            //List<Person> list = attbiz.QueryAttList(arrUID, obj.StartDate, obj.EndDate, list_trip);
            //attbiz.AddLog(tokenObj.uid, "获取出差休假报表");
            //attbiz.Close();
            //DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            //data.data = list;
            //return data;
            return null;
        }

        [HttpPost]
        [ActionName("QueryPerson")]
        public DataResult QueryPerson([FromBody]Person obj)
        {
            //AttBiz attbiz = new AttBiz();
            Trip t = new Trip();
            t.StartDate = obj.StartDate;
            t.EndDate = obj.EndDate;
            t.UID = obj.UID;
            t.LASTNAME = "";
            List<Trip> list = AttBiz.QueryTrip(t);
            Person p = AttBiz.QueryPersonAtt(obj.LOGINID, obj.StartDate, obj.EndDate, list,null,null);
           
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = p;
            return data;

        }
        [HttpPost]
        [ActionName("StcAttReport")]
        public DataResult StcAttReport([FromBody]DateRange obj)
        {
            TokenObj tokenObj = CheckToken(obj.Token, out code);
            if (code != MessageCode.SUCCESS)
            {
                return DataResult.InitFromMessageCode(code);
            }
            AttBiz attbiz = new AttBiz();
            List<Person> list = attbiz.StcAttReport(obj.start_date, obj.end_date, tokenObj);
            attbiz.Close();
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = list;
            return data;
        }
        [HttpPost]
        [ActionName("StcAttOld")]
        public DataResult StcAttOld()
        {
            string uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Upload\\";
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection fileCollection = request.Files;
            var formData = request.Form;
            string start_date = formData["start_time"];
            string end_date = formData["end_time"];
            if (fileCollection.Count > 0)
            {
                // 获取文件
                HttpPostedFile httpPostedFile = fileCollection[0];
                // 文件扩展名
                string fileExtension = Path.GetExtension(httpPostedFile.FileName);
                // 名称
                string fileName = httpPostedFile.FileName;
                // 上传路径
                string filePath = uploadPath + fileName;
                httpPostedFile.SaveAs(filePath);

                AttBiz att = new AttBiz();
                List<Person> list =  att.StcAttFromLoaclExcel(filePath, start_date, end_date);
                att.Close();
                DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
                data.data = list;
                return data;

            }
            return null;
        }
        [HttpPost]
        [ActionName("GetPersonAtt")]
        public DataResult GetPersonAtt([FromBody]Person obj)
        {
            TokenObj tokenObj = CheckToken(obj.Token, out code);
            if (code != MessageCode.SUCCESS)
            {
                return DataResult.InitFromMessageCode(code);
            }
            DBOA dboa = new DBOA();
            DataSet ds = dboa.ExeQuery($"select id,MOBILE,LOGINID from HRMRESOURCE where LOGINID='{tokenObj.uid}'");
            string mobile = ds.Tables[0].Rows[0]["MOBILE"].ToString();
            dboa.Close();
            DBAtt130 db = new DBAtt130();
            ds = db.ExeQuery($"select USERID from USERINFO where PAGER='{mobile}'");
            db.Close();
            string att_uid = ds.Tables[0].Rows[0][0].ToString();
            List<Att> list_att = AttBiz.GetPersonAtt(att_uid, obj.Month, tokenObj.uid);
            if (list_att == null)
            {
                return DataResult.InitFromMessageCode(MessageCode.ERROR_NO_DATA);
            }
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = list_att;
            return data;
        }
        /// <summary>
        /// Token,UID,LASTNAME,StartDate,EndDate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPersonTripandLeaveDetail")]
        public DataResult GetPersonTripandLeaveDetail([FromBody]Person obj)
        {
            TokenObj tokenObj = CheckToken(obj.Token, out code);
            if (code != MessageCode.SUCCESS)
            {
                return DataResult.InitFromMessageCode(code);
            }
            
            Trip t = new Trip();
            t.StartDate = obj.StartDate;
            t.EndDate = obj.EndDate;
            t.UID = obj.UID;
            t.LASTNAME = "";
            List<Trip> list_trip = AttBiz.list_trip_cache;
            if (list_trip == null)
            {
                list_trip = AttBiz.QueryTrip(t);
            }

            LeaveQuest leaveObj = new LeaveQuest();
            leaveObj.LASTNAME = obj.LASTNAME;
            leaveObj.StartDate = obj.StartDate;
            leaveObj.EndDate = obj.EndDate;
            List<LeaveQuest> list_leave = AttBiz.QueryLeave(leaveObj);
            List<Trip> list_trip_one = new List<Trip>();

            List<Att> list_att = new List<Att>();
            if (obj.att_userid!=null&& obj.att_userid != "") list_att = AttBiz.GetPersonAtt(obj.att_userid, obj.StartDate.Substring(0,7),obj.LOGINID);
           
            for (int i = 0; i < list_trip.Count; i++)
            {
                if (list_trip[i].UID == obj.UID) list_trip_one.Add(list_trip[i]);
            }
            PersonTripandLeaveDetail d = new PersonTripandLeaveDetail();
            for (int i = 0; i < list_leave.Count; i++)
            {
                list_leave[i].xjsq9 = TranLeaveType(int.Parse(list_leave[i].xjsq9));
            }
            d.list_leave = list_leave;
            d.list_trip = list_trip_one;
            d.list_att = list_att;
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = d;
            return data;

        }
        [HttpPost]
        [ActionName("PushGongchuForm")]
        public DataResult PushGongchuForm([FromBody]GongchuForm obj)
        {
            TokenObj tokenObj = CheckToken(obj.Token, out code);
            if (code != MessageCode.SUCCESS)
            {
                return DataResult.InitFromMessageCode(code);
            }
            DBOA db = new DBOA();
            DataSet ds = db.ExeQuery($@"select * from HRMRESOURCE where LOGINID='{tokenObj.uid}'");
            db.Close();
            if (ds.Tables[0].Rows.Count==0)
            {
                return DataResult.InitFromMessageCode(MessageCode.ERROR_NO_DATA);
            }
            obj.owner = ds.Tables[0].Rows[0]["ID"].ToString();
            obj.name = ds.Tables[0].Rows[0]["LASTNAME"].ToString();
            obj.com = ds.Tables[0].Rows[0]["SUBCOMPANYID1"].ToString();
            obj.dpt = ds.Tables[0].Rows[0]["DEPARTMENTID"].ToString();
            int res = OAService.PushGongchu(obj);
            DataResult data = null;
            if (res>0)
            {
                data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
                data.data = res;
            }
            else
            {
                data = DataResult.InitFromMessageCode(MessageCode.UNKONWN);
                data.data = res;
                data.message = "推送OA失败";
            }
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
