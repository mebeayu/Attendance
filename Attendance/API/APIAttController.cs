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
        [ActionName("Login")]
        public DataResult Login([FromBody]LoginRequest obj)
        {

            DBOA dboa = new DBOA();
            DataSet ds = dboa.ExeQuery("select LOGINID,PASSWORD,LASTNAME,MOBILE,DEPARTMENTID from HRMRESOURCE where LOGINID=:LOGINID", new OracleParameter("LOGINID", obj.uid));
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
            ds = db.ExeQuery("select * from login_user where uid=@uid", new SqlParameter("uid", obj.uid));
            string type = "0";
            if (ds.Tables[0].Rows.Count == 0)
            {
                int res = db.ExeCMD(@"insert into login_user(uid,name,mobile,department_id,department_path) 
                                    values(@uid,@name,@mobile,@department_id,@department_path)",
                                    new SqlParameter("uid", obj.uid),
                                    new SqlParameter("name", LASTNAME),
                                    new SqlParameter("mobile", MOBILE),
                                    new SqlParameter("department_id", DEPARTMENTID),
                                    new SqlParameter("department_path", department_path));
            }
            else
            {
                type = ds.Tables[0].Rows[0]["type"].ToString();
                int res = db.ExeCMD("update login_user set name=@name,mobile=@mobile,department_id=@department_id,department_path=@department_path where uid=@uid",
                                    new SqlParameter("uid", obj.uid),
                                    new SqlParameter("name", LASTNAME),
                                    new SqlParameter("mobile", MOBILE),
                                    new SqlParameter("department_id", DEPARTMENTID),
                                    new SqlParameter("department_path", department_path));
            }
            db.Close();
            DataResult return_data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            string Token = MakeToken(obj.uid, obj.psw, type, DEPARTMENTID);
            UserInfo u = new UserInfo();
            u.Token = Token;
            u.name = LASTNAME;
            u.type = type;
            u.department_id = DEPARTMENTID;
            u.department_path = department_path;
            tokenCache[obj.uid] = Token;
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
            AttBiz attbiz = new AttBiz();
            List<LeaveQuest> list = attbiz.QueryLeave(obj, tokenObj);
            attbiz.Close();
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
            AttBiz attbiz = new AttBiz();
            List<Trip> list = attbiz.QueryTrip(obj, tokenObj);
            attbiz.Close();
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = list;
            return data;
        }
        [HttpPost]
        [ActionName("QueryAttList")]
        public DataResult QueryAttList([FromBody]Person obj)
        {
            TokenObj tokenObj = CheckToken(obj.Token, out code);
            if (code != MessageCode.SUCCESS)
            {
                return DataResult.InitFromMessageCode(code);
            }
            AttBiz attbiz = new AttBiz();
            Trip t = new Trip();
            t.StartDate = obj.StartDate;
            t.EndDate = obj.EndDate;
            t.UID = obj.UID;
            t.LASTNAME = "";
            List<Trip> list_trip = attbiz.QueryTrip(t);
            List<string> arrUID = new List<string>();
            if (tokenObj.type == "0")
            {
                string oa_id = attbiz.GetOAUIDByLoginID(tokenObj.uid);
                if (oa_id != null) arrUID.Add(oa_id);
            }
            else
            {
               arrUID = attbiz.GetUIDinDate(obj.StartDate, obj.EndDate, list_trip);
            }
            List<Person> list = attbiz.QueryAttList(arrUID, obj.StartDate, obj.EndDate, list_trip);
            attbiz.Close();
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = list;
            return data;
        }
        [HttpPost]
        [ActionName("QueryPerson")]
        public DataResult QueryPerson([FromBody]Person obj)
        {
            AttBiz attbiz = new AttBiz();
            Trip t = new Trip();
            t.StartDate = obj.StartDate;
            t.EndDate = obj.EndDate;
            t.UID = obj.UID;
            t.LASTNAME = "";
            List<Trip> list = attbiz.QueryTrip(t);
            Person p = attbiz.QueryPersonAtt(obj.UID, obj.StartDate, obj.EndDate, list);
            attbiz.Close();
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = p;
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
            Person p = AttBiz.GetPersonAtt(tokenObj.uid, obj.Month);
            if (p==null)
            {
                return DataResult.InitFromMessageCode(MessageCode.ERROR_NO_DATA);
            }
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = p;
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
            AttBiz attbiz = new AttBiz();
            Trip t = new Trip();
            t.StartDate = obj.StartDate;
            t.EndDate = obj.EndDate;
            t.UID = obj.UID;
            t.LASTNAME = "";
            List<Trip> list_trip = attbiz.QueryTrip(t);
            LeaveQuest leaveObj = new LeaveQuest();
            leaveObj.LASTNAME = obj.LASTNAME;
            leaveObj.StartDate = obj.StartDate;
            leaveObj.EndDate = obj.EndDate;
            List<LeaveQuest> list_leave = attbiz.QueryLeave(leaveObj);
            List<Trip> list_trip_one = new List<Trip>();
            attbiz.Close();
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
            DataResult data = DataResult.InitFromMessageCode(MessageCode.SUCCESS);
            data.data = d;
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
