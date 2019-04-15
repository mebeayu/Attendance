using Attendance.Models;
using Common;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Attendance.Common
{
    public class AttBiz
    {
        DBOA dboa;
        
        public AttBiz()
        {
            dboa = new DBOA();
        }
        public void Close()
        {
            dboa.Close();
        }
        public string GetOAUIDByLoginID(string LoginID)
        {
            DataSet ds = dboa.ExeQuery($@"select ID from HRMRESOURCE where LOGINID='{LoginID}'");
            if (ds.Tables[0].Rows.Count > 0) return ds.Tables[0].Rows[0][0].ToString();
            else return null;
        }
        public List<LeaveQuest> QueryLeave(LeaveQuest obj,TokenObj tokenObj=null)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            DataSet ds = null;
            if (tokenObj==null||tokenObj.type != "0")
            {
                ds = dboa.ExeQuery($@"SELECT b.LASTNAME,b.MOBILE,xjsq5,xjsq19,xjsq9,xjsq10,xjsq17,c.NOWNODETYPE from 
                (formtable_main_242 a left join HRMRESOURCE b on b.ID=a.xjsq5) 
                left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
                where ((xjsq10>='{obj.StartDate}' and xjsq10<='{obj.EndDate}') or (xjsq17>='{obj.StartDate}' and xjsq17<='{obj.EndDate}')) and 
                b.LASTNAME like :LASTNAME and c.NOWNODETYPE=3 order by b.LASTNAME ASC",
                new OracleParameter("LASTNAME", "%" + obj.LASTNAME + "%"));
            }
            else
            {
                ds = dboa.ExeQuery($@"SELECT b.LASTNAME,b.MOBILE,xjsq5,xjsq19,xjsq9,xjsq10,xjsq17,c.NOWNODETYPE from 
                (formtable_main_242 a left join HRMRESOURCE b on b.ID=a.xjsq5) 
                left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
                where ((xjsq10>='{obj.StartDate}' and xjsq10<='{obj.EndDate}') or (xjsq17>='{obj.StartDate}' and xjsq17<='{obj.EndDate}')) and 
                b.LOGINID =:LOGINID and c.NOWNODETYPE=3 order by b.LASTNAME ASC",
                new OracleParameter("LOGINID",tokenObj.uid));
            }
           
            int n = ds.Tables[0].Rows.Count;
            List<LeaveQuest> list = new List<LeaveQuest>();
            for (int i = 0; i < n; i++)
            {
                LeaveQuest row = new LeaveQuest();
                row.uid = ds.Tables[0].Rows[i]["xjsq5"].ToString();
                row.MOBILE = ds.Tables[0].Rows[i]["MOBILE"].ToString();
                row.LASTNAME = ds.Tables[0].Rows[i]["LASTNAME"].ToString();
                row.xjsq5 = ds.Tables[0].Rows[i]["xjsq5"].ToString();
                //row.xjsq19 = ds.Tables[0].Rows[i]["xjsq19"].ToString();
                int type = -1;
                try
                {
                    type = int.Parse(ds.Tables[0].Rows[i]["xjsq9"].ToString());
                }
                catch (Exception)
                {
                    type = -1;
                }
                row.xjsq9 = type.ToString();

                row.xjsq10 = ds.Tables[0].Rows[i]["xjsq10"].ToString();
                row.xjsq17 = ds.Tables[0].Rows[i]["xjsq17"].ToString();
                DateTime start = DateTime.Parse(obj.StartDate);
                DateTime end = DateTime.Parse(obj.EndDate);

                DateTime s = DateTime.Parse(row.xjsq10);
                DateTime e = DateTime.Parse(row.xjsq17);
                if (s < start) s = start;
                if (e > end) e = end;
                TimeSpan timeSpan = e - s;
                row.xjsq19 = (timeSpan.Days+1).ToString();
                row.NOWNODETYPE = int.Parse(ds.Tables[0].Rows[i]["NOWNODETYPE"].ToString());
                list.Add(row);

            }
            return list;
        }
        public List<Trip>  QueryTrip(Trip obj,TokenObj tokenObj=null)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            Dictionary<string, int> cacheDic = new Dictionary<string, int>();

            DataSet ds = null;
            if(tokenObj==null|| tokenObj.type != "0")
            {
                ds = dboa.ExeQuery($@"select b.LASTNAME,b.MOBILE,c.NOWNODETYPE,a.* from  
                (formtable_main_45 a left join  HRMRESOURCE b on (a.JBR=b.id) ) 
                left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
                where ((CC4>='{obj.StartDate}' and CC4<='{obj.EndDate}') or (CC5>='{obj.StartDate}' and CC5<='{obj.EndDate}')) and 
                b.LASTNAME like :LASTNAME and c.NOWNODETYPE=3 order by b.LASTNAME ASC",
               new OracleParameter("LASTNAME", "%" + obj.LASTNAME + "%"));
            }
            else
            {
                ds = dboa.ExeQuery($@"select b.LASTNAME,b.MOBILE,c.NOWNODETYPE,a.* from  
                (formtable_main_45 a left join  HRMRESOURCE b on (a.JBR=b.id) ) 
                left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
                where ((CC4>='{obj.StartDate}' and CC4<='{obj.EndDate}') or (CC5>='{obj.StartDate}' and CC5<='{obj.EndDate}')) and 
                b.LOGINID =:LOGINID and c.NOWNODETYPE=3 order by b.LASTNAME ASC",
               new OracleParameter("LOGINID", tokenObj.uid));
                
            }

            int n = ds.Tables[0].Rows.Count;
            List<Trip> list = new List<Trip>();
            for (int i = 0; i < n; i++)
            {
                string key = ds.Tables[0].Rows[i]["JBR"].ToString() + ds.Tables[0].Rows[i]["CC4"].ToString() + ds.Tables[0].Rows[i]["CC5"].ToString();
                
                Trip trip = new Trip();
                trip.UID = ds.Tables[0].Rows[i]["JBR"].ToString();
                trip.MOBILE = ds.Tables[0].Rows[i]["MOBILE"].ToString();
                trip.LASTNAME = ds.Tables[0].Rows[i]["LASTNAME"].ToString();
                trip.Title = ds.Tables[0].Rows[i]["CC3"].ToString();
                trip.Path = ds.Tables[0].Rows[i]["CC6"].ToString();
                trip.REQUESTID = ds.Tables[0].Rows[i]["REQUESTID"].ToString();
                trip.StartDate = ds.Tables[0].Rows[i]["CC4"].ToString();
                trip.EndDate = ds.Tables[0].Rows[i]["CC5"].ToString();

                DateTime s = DateTime.Parse(trip.StartDate);
                DateTime e = DateTime.Parse(trip.EndDate);

                DateTime start = DateTime.Parse(obj.StartDate);
                DateTime end = DateTime.Parse(obj.EndDate);
                if (s < start) s = start;
                if (e > end) e = end;
                TimeSpan timeSpan = e - s;
                trip.Days = timeSpan.Days + 1;
                if (cacheDic.Keys.Contains(key) == false)
                {
                    list.Add(trip);
                    cacheDic.Add(key, 0);
                }
                string strIDS = ds.Tables[0].Rows[i]["CC8"].ToString();
                string[] arrIDs = strIDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < arrIDs.Length; j++)
                {
                    if (arrIDs[j] == trip.UID) continue;
                    Trip trip1 = new Trip();
                    trip1.UID = arrIDs[j];

                    DataSet ds1 = dboa.ExeQuery($@"select LASTNAME,MOBILE from HRMRESOURCE where id={trip1.UID}");
                    trip1.MOBILE = ds1.Tables[0].Rows[0][1].ToString();
                    trip1.LASTNAME = ds1.Tables[0].Rows[0][0].ToString();
                    trip1.Title = trip.Title;
                    trip1.Path = trip.Path;
                    trip1.REQUESTID = trip.REQUESTID;
                    trip1.StartDate = trip.StartDate;
                    trip1.EndDate = trip.EndDate;
                    trip1.Days = trip.Days;
                    key = trip1.UID + trip1.StartDate + trip1.EndDate;
                    if (cacheDic.Keys.Contains(key) == false)
                    {
                        list.Add(trip1);
                        cacheDic.Add(key, 0);
                    }
                    
                }

            }
            
            return list;
        }
        public List<string> GetUIDinDate(string start_date, string end_date, List<Trip> list_trip)
        {
            List<string> arrUID = new List<string>();
            DataSet ds= dboa.ExeQuery($@"select distinct xjsq5 from formtable_main_242 where 
            ((xjsq10>='{start_date}' and xjsq10<='{end_date}') or (xjsq17>='{start_date}' and xjsq17<='{end_date}'))");
            int n = ds.Tables[0].Rows.Count;
            for (int i = 0; i < n; i++)
            {
                arrUID.Add(ds.Tables[0].Rows[i][0].ToString());
            }
            n = list_trip.Count;
            for (int i = 0; i < n; i++)
            {
                if (arrUID.Contains(list_trip[i].UID) == false) arrUID.Add(list_trip[i].UID);
            }
            return arrUID;
        }
        public List<Person> QueryAttList(List<string> arrUID,string start_date, string end_date, List<Trip> list_trip)
        {
            List<Person> list = new List<Person>();
            int n = arrUID.Count;
            for (int i = 0; i < n; i++)
            {
                Person p = QueryPersonAtt(arrUID[i], start_date, end_date, list_trip);
                list.Add(p);

            }
            return list;
        }
        public Person QueryPersonAtt(string  uid,string start_date,string end_date, List<Trip> list_trip)
        {
            DataSet ds = dboa.ExeQuery($@"select LASTNAME,MOBILE,DEPARTMENTNAME,LOGINID from HRMRESOURCE a 
            left join HRMDEPARTMENT b on a.DEPARTMENTID=b.ID where a.id={uid}");
            if (ds.Tables[0].Rows.Count==0)
            {
                return null;
            }
            Person p = new Person();
            p.UID = uid;
            p.LASTNAME = ds.Tables[0].Rows[0]["LASTNAME"].ToString();
            p.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
            p.Department = ds.Tables[0].Rows[0]["DEPARTMENTNAME"].ToString();
            p.LOGINID = ds.Tables[0].Rows[0]["LOGINID"].ToString();
            ds = dboa.ExeQuery($@"select * from formtable_main_242  a left join  workflow_nownode b on a.REQUESTID=b.REQUESTID
            where xjsq5={uid} and b.NOWNODETYPE=3 and 
            ((xjsq10>='{start_date}' and xjsq10<='{end_date}') or (xjsq17>='{start_date}' and xjsq17<='{end_date}'))");
            Dictionary<int, double> dic = new Dictionary<int, double>();
            dic.Add(0, 0);
            dic.Add(1, 0);
            dic.Add(2, 0);
            dic.Add(3, 0);
            dic.Add(4, 0);
            dic.Add(5, 0);
            dic.Add(6, 0);
            dic.Add(7, 0);
            int n = ds.Tables[0].Rows.Count;
            for (int i = 0; i < n; i++)
            {
                DateTime start = DateTime.Parse(start_date);
                DateTime end = DateTime.Parse(end_date);

                DateTime s = DateTime.Parse(ds.Tables[0].Rows[i]["xjsq10"].ToString());
                DateTime e = DateTime.Parse(ds.Tables[0].Rows[i]["xjsq17"].ToString());

                if (s < start) s = start;
                if (e > end) e = end;
                TimeSpan timeSpan = e - s;

                int type = StringToInt(ds.Tables[0].Rows[i]["xjsq9"].ToString());
                dic[type] = dic[type] + (timeSpan.Days+1);
            }
            //0事假 1病假 2婚假 3产假 4丧假 5年休假 6其他 7陪产假
            p.Leave0 = dic[0];
            p.Leave1 = dic[1];
            p.Leave2 = dic[2];
            p.Leave3 = dic[3];
            p.Leave4 = dic[4];
            p.Leave5 = dic[5];
            p.Leave6 = dic[6];
            p.Leave7 = dic[7];
            ///////////////////////////////////////
            p.Trip = 0;
            n = list_trip.Count;
            for (int i = 0; i < n; i++)
            {
                if(p.UID== list_trip[i].UID) p.Trip += list_trip[i].Days;
            }

            return p;
        }
        public List<Person> GetPersonBaseFromExcel(string path)
        {
            Excel excel = new Excel();
            List<Person> list = new List<Person>();
            DBOA dboa = new DBOA();
            excel.OpenExcel(path);
            int index = 4;
            while (true)
            {
                //string t = excel.GetCellValue(4, 16);
                Person p = new Person();
                p.LASTNAME = excel.GetCellValue("A", index);
                if (p.LASTNAME == "" || p.LASTNAME == null) break;
                try
                {
                    p.WorkDay = int.Parse(excel.GetCellValue("D", index));
                    //p.AttDay = int.Parse(excel.GetCellValue("E", index));
                    //p.LateCount = int.Parse(excel.GetCellValue("F", index));
                    //p.EarlyCount = int.Parse(excel.GetCellValue("H", index));
                }
               catch(Exception)
                {
                    //excel.CloseExcel();
                }
                double att = 0;
                List<DayDetail> list_daydetail = new List<DayDetail>();
                for (int i = 6; i <= 36; i++)
                {
                    att = 0;
                    DayDetail daydetail = new DayDetail();
                    daydetail.morning = excel.GetCellValue(index, i);
                    daydetail.afternoon = excel.GetCellValue(index+1, i);
                    if(daydetail.morning != "休")
                    {
                        if (daydetail.morning != "漏" && daydetail.morning != "") att += 0.5;
                        if (daydetail.afternoon != "漏" && daydetail.afternoon != "" && daydetail.afternoon != null) att += 0.5;
                        if (att >0) p.AttDay += att;
                    }
                    
                   
                    daydetail.tag = 0;
                    list_daydetail.Add(daydetail);
                }
                p.ListDetail = list_daydetail;
                DataSet ds = dboa.ExeQuery($@"select ID from HRMRESOURCE where LASTNAME='{p.LASTNAME}' and SUBCOMPANYID1=68");
                if (ds.Tables[0].Rows.Count == 0) p.UID = "";
                else p.UID = ds.Tables[0].Rows[0][0].ToString();
                list.Add(p);
                index += 2;

            }
            dboa.Close();
            excel.CloseExcel();
            return list;
        }
        public List<Person> StcAttFromLoaclExcel(string path,string start_date,string end_date)
        {
            List<Person> list = GetPersonBaseFromExcel(path);
            List<string> arrUIDs = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UID != "") arrUIDs.Add(list[i].UID);
            }
            Trip t = new Trip();
            t.StartDate = start_date;
            t.EndDate = end_date;
            t.UID = "";
            t.LASTNAME = "";
            List<Trip> list_trip = QueryTrip(t);
            List<Person> list_oa = QueryAttList(arrUIDs, start_date, end_date, list_trip);
            DBAtt db = new DBAtt();
            string month = start_date.Substring(0, 7);
            int res = db.ExeCMD($@"delete from Detail where Month='{month}'");
            for (int i = 0; i < list.Count; i++)
            {
                Person p = FindPerson(list[i].UID, list_oa);
                if (p != null)
                {
                    list[i].MOBILE = p.MOBILE;
                    list[i].Department = p.Department;
                    list[i].Trip = p.Trip;
                    list[i].Leave0 = p.Leave0;
                    list[i].Leave1 = p.Leave1;
                    list[i].Leave2 = p.Leave2;
                    list[i].Leave3 = p.Leave3;
                    list[i].Leave4 = p.Leave4;
                    list[i].Leave5 = p.Leave5;
                    list[i].Leave6 = p.Leave6;
                    list[i].Leave7 = p.Leave7;
                    list[i].LOGINID = p.LOGINID;
                    
                }
                res = db.ExeCMD($@"insert into Detail(oa_uid,LOGINID,MOBILE,LASTNAME,Department,
                                WorkDay,AttDay,LateCount,EarlyCount,Trip,
                                Leave0,Leave1,Leave2,Leave3,Leave4,Leave5,Leave6,Leave7,Detail,Month) values
                                ('{list[i].UID}','{list[i].LOGINID}','{list[i].MOBILE}','{list[i].LASTNAME}','{list[i].Department}',
                                 {list[i].WorkDay},{list[i].AttDay},{list[i].LateCount},{list[i].EarlyCount},{list[i].Trip},
                                 {list[i].Leave0},{list[i].Leave1},{list[i].Leave2},{list[i].Leave3},{list[i].Leave4},{list[i].Leave5},{list[i].Leave6},{list[i].Leave7},
                                 '{JsonConvert.SerializeObject(list[i].ListDetail)}','{month}')");
            }
            db.Close();
            return list;
        }
        public static Person GetPersonAtt(string LOGINID,string Month)
        {
            DBAtt db = new DBAtt();
            DataSet ds = db.ExeQuery($@"select * from Detail where LOGINID='{LOGINID}' and Month='{Month}'");
            if (ds == null||ds.Tables[0].Rows.Count==0) return null;
            Person p = new Person();
            p.UID = ds.Tables[0].Rows[0]["oa_uid"].ToString();
            p.LOGINID = ds.Tables[0].Rows[0]["LOGINID"].ToString();
            p.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
            p.LASTNAME = ds.Tables[0].Rows[0]["LASTNAME"].ToString();
            p.Department = ds.Tables[0].Rows[0]["Department"].ToString();
            p.WorkDay = int.Parse(ds.Tables[0].Rows[0]["WorkDay"].ToString());
            p.AttDay = int.Parse(ds.Tables[0].Rows[0]["AttDay"].ToString());
            p.LateCount = int.Parse(ds.Tables[0].Rows[0]["LateCount"].ToString());
            p.EarlyCount = int.Parse(ds.Tables[0].Rows[0]["EarlyCount"].ToString());
            p.Trip = int.Parse(ds.Tables[0].Rows[0]["Trip"].ToString());
            p.Leave0 = double.Parse(ds.Tables[0].Rows[0]["Leave0"].ToString());
            p.Leave1 = double.Parse(ds.Tables[0].Rows[0]["Leave1"].ToString());
            p.Leave2 = double.Parse(ds.Tables[0].Rows[0]["Leave2"].ToString());
            p.Leave3 = double.Parse(ds.Tables[0].Rows[0]["Leave3"].ToString());
            p.Leave4 = double.Parse(ds.Tables[0].Rows[0]["Leave4"].ToString());
            p.Leave5 = double.Parse(ds.Tables[0].Rows[0]["Leave5"].ToString());
            p.Leave6 = double.Parse(ds.Tables[0].Rows[0]["Leave6"].ToString());
            p.Leave7 = double.Parse(ds.Tables[0].Rows[0]["Leave7"].ToString());
            p.ListDetail = JsonConvert.DeserializeObject<List<DayDetail>>(ds.Tables[0].Rows[0]["Detail"].ToString());
            db.Close();
            return p;
        }
        private Person FindPerson(string UID, List<Person> list_oa)
        {
            for (int i = 0; i < list_oa.Count; i++)
            {
                if (list_oa[i].UID == UID) return list_oa[i];
            }
            return null;
        }
        private double StringToFloat(string num)
        {
            try
            {
                double res = Convert.ToDouble(num);
                return res;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        private int StringToInt(string num)
        {
            try
            {
                int res = Convert.ToInt32(num);
                return res;
            }
            catch (Exception)
            {

                return 0;
            }
        }
    }
}