using Attendance.Models;
using Common;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Attendance.Common
{
    public class AttBiz
    {
        DBOA dboa;
        //private static Dictionary<string, List<Trip>> cacheTripList;
        public AttBiz()
        {
            //cacheTripList = new Dictionary<string, List<Trip>>();
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
        public static List<Gongchu> QueryGongchu(string Name, string StartDate, string EndDate, TokenObj tokenObj = null)
        {
            if (Name == null)
            {
                Name = "";
            }
            DBOA dboa = new DBOA();
            DataSet ds = null;
            if (tokenObj == null || tokenObj.type != "0")
            {
                ds = dboa.ExeQuery($@"select a.*,b.LASTNAME,b.MOBILE,c.NOWNODETYPE from 
                                    (formtable_main_555 a left join  HRMRESOURCE b on (a.OWNER=b.id) ) 
                                    left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
								    where c.NOWNODETYPE=3 and date_time_gc>='{StartDate}' and date_time_gc<='{EndDate}' 
                                    and b.LASTNAME like '{Name}%' order by b.LASTNAME ASC");
            }
            else
            {
                ds = dboa.ExeQuery($@"select a.*,b.LASTNAME,b.MOBILE,c.NOWNODETYPE from 
                                    (formtable_main_555 a left join  HRMRESOURCE b on (a.OWNER=b.id) ) 
                                    left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
								    where c.NOWNODETYPE=3 and date_time_gc>='{StartDate}' and date_time_gc<='{EndDate}' 
                                    and b.LOGINID = '{tokenObj.uid}' order by b.LASTNAME ASC");
            }
            dboa.Close();
            List<Gongchu> list = new List<Gongchu>();
            Dictionary<string, Gongchu> tempDic = new Dictionary<string, Gongchu>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                string oa_uid = ds.Tables[0].Rows[i]["OWNER"].ToString();
                if (tempDic.Keys.Contains(oa_uid))
                {
                    if (ds.Tables[0].Rows[i]["RANGE"].ToString() == "0" || ds.Tables[0].Rows[i]["RANGE"].ToString() == "1") tempDic[oa_uid].day_count += 0.5;
                    else tempDic[oa_uid].day_count += 1;
                }
                else
                {
                    Gongchu g = new Gongchu();

                    g.oa_uid = oa_uid;
                    g.name = ds.Tables[0].Rows[i]["LASTNAME"].ToString();
                    g.mobile = ds.Tables[0].Rows[i]["MOBILE"].ToString();
                    if (ds.Tables[0].Rows[i]["RANGE"].ToString() == "0" || ds.Tables[0].Rows[i]["RANGE"].ToString() == "1") g.day_count = 0.5;
                    else g.day_count = 1;
                    tempDic.Add(oa_uid, g);
                }
            }
            foreach (KeyValuePair<string, Gongchu> g in tempDic)
            {
                list.Add(g.Value);
            }
            return list;
        }
        public List<LeaveQuest> QueryLeave(LeaveQuest obj, TokenObj tokenObj = null)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            DataSet ds = null;
            if (tokenObj == null || tokenObj.type != "0")
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
                new OracleParameter("LOGINID", tokenObj.uid));
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
                row.xjsq19 = (timeSpan.Days + 1).ToString();
                row.NOWNODETYPE = int.Parse(ds.Tables[0].Rows[i]["NOWNODETYPE"].ToString());
                list.Add(row);

            }
            return list;
        }

        public List<Trip> QueryTrip(Trip obj, TokenObj tokenObj = null, List<string> arrOAID = null)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            Dictionary<string, int> cacheDic = new Dictionary<string, int>();

            DataSet ds = null;
            if (arrOAID == null)
            {
                if (tokenObj == null || tokenObj.type != "0")
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
            }
            else
            {
                string OAID = string.Join(",", arrOAID);
                ds = dboa.ExeQuery($@"select b.LASTNAME,b.MOBILE,c.NOWNODETYPE,a.* from  
                (formtable_main_45 a left join  HRMRESOURCE b on (a.JBR=b.id) ) 
                left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
                where ((CC4>='{obj.StartDate}' and CC4<='{obj.EndDate}') or (CC5>='{obj.StartDate}' and CC5<='{obj.EndDate}')) and 
                b.ID in({OAID}) and c.NOWNODETYPE=3 order by b.LASTNAME ASC");
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
            //string cacheTripKey = "";
            //if (tokenObj != null)
            //{
            //    cacheTripKey = obj.StartDate + obj.EndDate + tokenObj.type;
            //}
            //else
            //{
            //    cacheTripKey = obj.StartDate + obj.EndDate;
            //}
            //cacheTripList[cacheTripKey] = list;
            return list;
        }
        public List<string> GetUIDinDate(string start_date, string end_date, List<Trip> list_trip)
        {
            List<string> arrUID = new List<string>();
            DataSet ds = dboa.ExeQuery($@"select distinct xjsq5 from formtable_main_242 where 
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
        public List<Person> QueryAttList(List<string> arrUID, string start_date, string end_date, List<Trip> list_trip, List<Gongchu> list_gongchu = null)
        {
            List<Person> list = new List<Person>();
            int n = arrUID.Count;
            for (int i = 0; i < n; i++)
            {
                Person p = QueryPersonAtt(arrUID[i], start_date, end_date, list_trip,null);
                list.Add(p);

            }
            return list;
        }
        //新版考勤
        public Person QueryPersonAtt_2(string oa_uid/*OA的用户ID*/,
            string att_userid,
            string start_date, string end_date, //这两个日期要和获取list_trip的一致
            List<Trip> list_trip,
            List<Att> list_att_record,//日期期间的打卡记录
            List<string> list_date,
            List<Gongchu> list_gongchu,
            string name,
            List<UserRel> list_user_rel)
        {
            Person p = QueryPersonAtt(oa_uid, start_date, end_date, list_trip, list_user_rel);
            if (p.LASTNAME == null || p.LASTNAME == "") p.LASTNAME = name;
            List<Att> list_att_record_person = new List<Att>();//当前用户的考勤记录
            for (int i = 0; i < list_att_record.Count; i++)
            {
                if (list_att_record[i].att_userid == att_userid)
                {
                    list_att_record_person.Add(list_att_record[i]);
                }
            }
            for (int i = 0; i < list_date.Count; i++)
            {
                Att oneAtt = FindAttDate(list_att_record_person, list_date[i]);
                if (oneAtt == null) p.NoAttDay += 1;
                else
                {
                    if (oneAtt.first == oneAtt.last)
                    {
                        p.NoAttDay += 0.5;
                        p.AttDay += 0.5;
                    }
                    else
                    {
                        p.AttDay += 1;
                        DateTime first_ok = DateTime.Parse($"{list_date[i]} 9:00");
                        DateTime last_ok = DateTime.Parse($"{list_date[i]} 17:00");
                        if (oneAtt.first > first_ok) p.LateCount += 1;
                        if (oneAtt.last < last_ok) p.EarlyCount += 1;
                    }

                }


            }
            for (int i = 0; i < list_gongchu.Count; i++)
            {
                if (list_gongchu[i].oa_uid == oa_uid)
                {
                    p.Gongchu = list_gongchu[i].day_count;
                }
            }
            p.SumAtt = p.AttDay + p.Trip + p.Leave0 + p.Leave1 + p.Leave2 + p.Leave3 + p.Leave4 + p.Leave5 + p.Leave6 + p.Leave7 + p.Gongchu;
            return p;
        }
        public List<Person> StcAttReport(string start_date, string end_date, TokenObj tokenObj)
        {
            DBAtt130 db130 = new DBAtt130();
            DataSet ds = null;

            if (tokenObj.type == "100")
            {

            }
            ds = db130.ExeQuery("select USERID,PAGER,NAME from USERINFO");

            List<UserRel> list_user_rel = new List<UserRel>();//用户对照
            List<string> mobiles = new List<string>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                UserRel ur = new UserRel();
                ur.att_userid = ds.Tables[0].Rows[i][0].ToString();
                ur.mobile = ds.Tables[0].Rows[i][1].ToString();
                ur.name = ds.Tables[0].Rows[i][2].ToString();
                list_user_rel.Add(ur);
                mobiles.Add($"'{ur.mobile}'");
            }
            string mobile_str = string.Join(",", mobiles);
            ds = db130.ExeQuery($@"select USERID,convert(varchar(10),CHECKTIME,120) as date_day,MIN(CHECKTIME) as f,MAX(CHECKTIME) as l 
                                from CHECKINOUT where convert(varchar(10),CHECKTIME,120)>='{start_date}' and convert(varchar(10),CHECKTIME,120)<='{end_date}'
                                GROUP BY convert(varchar(10),CHECKTIME,120),USERID");
            List<Att> list_att_record = new List<Att>();//考勤记录
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Att a = new Att();
                a.att_userid = ds.Tables[0].Rows[i]["USERID"].ToString();
                a.date_day = ds.Tables[0].Rows[i]["date_day"].ToString();
                a.first = DateTime.Parse(ds.Tables[0].Rows[i]["f"].ToString());
                a.last = DateTime.Parse(ds.Tables[0].Rows[i]["l"].ToString());
                list_att_record.Add(a);
            }
            db130.Close();
            DBOA dboa = new DBOA();//$@"select LASTNAME,MOBILE,DEPARTMENTNAME,LOGINID from HRMRESOURCE a left join HRMDEPARTMENT b on a.DEPARTMENTID = b.ID where a.id ={ uid}"
            ds = dboa.ExeQuery($@"select a.ID,LASTNAME,MOBILE,DEPARTMENTNAME,LOGINID from 
                                HRMRESOURCE a left join HRMDEPARTMENT b on a.DEPARTMENTID = b.ID  where a.MOBILE in({mobile_str})");
            dboa.Close();
            string expression = "";
            List<string> arrOAUID = new List<string>();
            for (int i = 0; i < list_user_rel.Count; i++)
            {
                expression = $"MOBILE='{list_user_rel[i].mobile}'";
                DataRow[] rows = ds.Tables[0].Select(expression);
                if (rows.Length == 0)
                {
                    list_user_rel[i].oa_userid = "";
                }
                else
                {
                    list_user_rel[i].oa_userid = rows[0]["ID"].ToString();
                    list_user_rel[i].oa_login_id = rows[0]["LOGINID"].ToString();
                    list_user_rel[i].department = rows[0]["DEPARTMENTNAME"].ToString();
                    arrOAUID.Add(list_user_rel[i].oa_userid);
                }
            }
            List<string> list_date = new List<string>();
            DateTime first_day = DateTime.Parse(start_date);
            DateTime last_day = DateTime.Parse(end_date);
            int work_day = 0;
            while (first_day <= last_day)
            {
                bool IsHoliday = IsHolidayByDate(first_day);

                if (IsHoliday == false)
                {
                    work_day++;
                    list_date.Add(first_day.ToString("yyyy-MM-dd"));
                }

                first_day = first_day.AddDays(1);
            }
            Trip t = new Trip();
            t.StartDate = start_date;
            t.EndDate = end_date;
            t.UID = "";
            t.LASTNAME = "";
            List<Trip> list_trip = QueryTrip(t, null, arrOAUID);
            List<Gongchu> list_gongchu = QueryGongchu("", start_date, end_date);
            List<Person> list_person = new List<Person>();
            for (int i = 0; i < list_user_rel.Count; i++)
            {
                if (tokenObj.type == "100")
                {
                    Person p = QueryPersonAtt_2(list_user_rel[i].oa_userid, list_user_rel[i].att_userid, start_date, end_date,
                   list_trip, list_att_record, list_date, list_gongchu, list_user_rel[i].name, list_user_rel);
                    p.att_userid = list_user_rel[i].att_userid;
                    p.WorkDay = work_day;
                    list_person.Add(p);
                }
                else
                {
                    if (list_user_rel[i].oa_login_id == tokenObj.uid)
                    {
                        Person p = QueryPersonAtt_2(list_user_rel[i].oa_userid, list_user_rel[i].att_userid, start_date, end_date,
                   list_trip, list_att_record, list_date, list_gongchu, list_user_rel[i].name, list_user_rel);
                        p.att_userid = list_user_rel[i].att_userid;
                        p.WorkDay = work_day;
                        list_person.Add(p);
                    }
                }

            }
            return list_person;
        }

        private Att FindAttDate(List<Att> list_att_record_person, string date)
        {
            for (int i = 0; i < list_att_record_person.Count; i++)
            {
                if (list_att_record_person[i].date_day == date) return list_att_record_person[i];
            }
            return null;
        }
        public Person QueryPersonAtt(string uid, string start_date, string end_date, List<Trip> list_trip, List<UserRel> list_user_rel)
        {
            DataSet ds = null;
            Person p = new Person();

            p.LASTNAME = "";
            if (uid=="")
            {
                return p;
            }
            if (list_user_rel == null)
            {
                ds = dboa.ExeQuery($@"select LASTNAME,MOBILE,DEPARTMENTNAME,LOGINID from HRMRESOURCE a 
                                    left join HRMDEPARTMENT b on a.DEPARTMENTID=b.ID where a.id={uid}");

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    dboa.Close();
                    return p;
                }
                p.UID = uid;
                p.LASTNAME = ds.Tables[0].Rows[0]["LASTNAME"].ToString();
                p.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
                p.Department = ds.Tables[0].Rows[0]["DEPARTMENTNAME"].ToString();
                p.LOGINID = ds.Tables[0].Rows[0]["LOGINID"].ToString();
            }
            else
            {
                for (int i = 0; i < list_user_rel.Count; i++)
                {
                    if (list_user_rel[i].oa_userid == uid)
                    {
                        p.UID = uid;
                        p.LASTNAME = list_user_rel[i].name;
                        p.MOBILE = list_user_rel[i].mobile;
                        p.Department = list_user_rel[i].department;
                        p.LOGINID = list_user_rel[i].oa_login_id;
                    }
                    
                }
            }

            
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
                dic[type] = dic[type] + (timeSpan.Days + 1);
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
                if (p.UID == list_trip[i].UID) p.Trip += list_trip[i].Days;
            }
            p.SumAtt = p.AttDay + p.Trip + p.Leave0 + p.Leave1 + p.Leave2 + p.Leave3 + p.Leave4 + p.Leave5 + p.Leave6 + p.Leave7;
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
                p.LASTNAME = excel.GetCellValue(index, 1);
                if (p.LASTNAME == "" || p.LASTNAME == null) break;
                try
                {
                    p.WorkDay = int.Parse(excel.GetCellValue(index, 4));
                    //p.AttDay = int.Parse(excel.GetCellValue("E", index));
                    //p.LateCount = int.Parse(excel.GetCellValue("F", index));
                    //p.EarlyCount = int.Parse(excel.GetCellValue("H", index));
                }
                catch (Exception)
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
                    daydetail.afternoon = excel.GetCellValue(index + 1, i);
                    if (daydetail.morning != "休")
                    {
                        if (daydetail.morning != "漏" && daydetail.morning != "") att += 0.5;
                        if (daydetail.afternoon != "漏" && daydetail.afternoon != "" && daydetail.afternoon != null) att += 0.5;
                        if (att > 0) p.AttDay += att;
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

        public List<Person> StcAttFromLoaclExcel(string path, string start_date, string end_date)
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
                    list[i].SumAtt = p.SumAtt + list[i].AttDay;

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
        private static string GetWeekString(int week)
        {
            string str = "";
            switch (week)
            {
                case 0:
                    str = "星期日";
                    break;
                case 1:
                    str = "星期一";
                    break;
                case 2:
                    str = "星期二";
                    break;
                case 3:
                    str = "星期三";
                    break;
                case 4:
                    str = "星期四";
                    break;
                case 5:
                    str = "星期五";
                    break;
                case 6:
                    str = "星期六";
                    break;
                default:
                    break;
            }
            return str;
        }
        public static List<Att> GetPersonAtt(string att_uid, string Month, string oa_login_id)
        {
            DBAtt130 db = new DBAtt130();
            DataSet ds = db.ExeQuery($@"SELECT
	                                    a.NAME,a.USERID,a.PAGER, 
	                                    CONVERT ( VARCHAR ( 10 ), CHECKTIME, 120 ) AS date,
	                                    MIN( CHECKTIME ) AS f,
	                                    MAX( CHECKTIME ) AS l 
                                    FROM
	                                    USERINFO a left join CHECKINOUT b on a.USERID=b.USERID 
	                                    where CONVERT ( VARCHAR ( 10 ), CHECKTIME, 120 )  like '{Month}%' and a.USERID={att_uid} 
                                    GROUP BY
	                                    CONVERT ( VARCHAR ( 10 ), CHECKTIME, 120 ),
	                                    a.USERID,a.NAME,a.PAGER ");
            db.Close();

            List<Att> list = new List<Att>();
            DateTime dd = DateTime.Parse($"{Month}-01");
            int Days = DateTime.DaysInMonth(dd.Year, dd.Month);
            Dictionary<string, Gongchu> dic_gongchu = new Dictionary<string, Gongchu>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                DBOA dboa = new DBOA();
                DataSet dsoa = dboa.ExeQuery($@"select a.*,b.LASTNAME,b.MOBILE,c.NOWNODETYPE from 
                                    (formtable_main_555 a left join  HRMRESOURCE b on (a.OWNER=b.id) ) 
                                    left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
								    where c.NOWNODETYPE=3 and date_time_gc>='{dd.ToString("yyyy-MM-dd")}' and date_time_gc<='{dd.AddDays(Days).ToString("yyyy-MM-dd")}' 
                                    and b.LOGINID = '{oa_login_id}' order by b.LASTNAME ASC");
                dboa.Close();
                for (int i = 0; i < dsoa.Tables[0].Rows.Count; i++)
                {

                    string range = dsoa.Tables[0].Rows[i]["RANGE"].ToString();
                    string date = dsoa.Tables[0].Rows[i]["DATE_TIME_GC"].ToString();
                    if (dic_gongchu.Keys.Contains(date) == false)
                    {
                        Gongchu g = new Gongchu();
                        g.memo = dsoa.Tables[0].Rows[i]["REASON"].ToString();
                        if (range == "0")
                        {
                            g.day_count = 0.5;
                            g.range = "上午";
                        }
                        else if (range == "1")
                        {
                            g.day_count = 0.5;
                            g.range = "下午";
                        }
                        else
                        {
                            g.day_count = 1;
                            g.range = "全天";
                        }
                        dic_gongchu.Add(date, g);
                    }
                    else
                    {
                        if (range == "0" || range == "1") dic_gongchu[date].day_count += 0.5;
                        else dic_gongchu[date].day_count += 1;
                    }

                }

            }
            Dictionary<string, Att> dicTemp = new Dictionary<string, Att>();
            for (int i = 0; i < Days; i++)
            {
                string key = dd.ToString("yyyy-MM-dd");
                Att a = new Att();
                a.date_day = key;
                a.week = GetWeekString((int)dd.DayOfWeek);
                if (dic_gongchu.Keys.Contains(key)) a.gongchu = $"{dic_gongchu[key].day_count}:{dic_gongchu[key].range}-{dic_gongchu[key].memo}";
                dicTemp.Add(key, a);
                dd = dd.AddDays(1);
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Att a = dicTemp[ds.Tables[0].Rows[i]["date"].ToString()];
                a.att_userid = ds.Tables[0].Rows[i]["USERID"].ToString();
                a.name = ds.Tables[0].Rows[i]["NAME"].ToString();
                a.mobile = ds.Tables[0].Rows[i]["PAGER"].ToString();
                a.date_day = ds.Tables[0].Rows[i]["date"].ToString();
                a.first = DateTime.Parse(ds.Tables[0].Rows[i]["f"].ToString());
                a.last = DateTime.Parse(ds.Tables[0].Rows[i]["l"].ToString());
                a.first_str = a.first.ToString("yyyy-MM-dd HH:mm:ss");
                a.last_str = a.last.ToString("yyyy-MM-dd HH:mm:ss");
                //a.week = ((int)DateTime.Parse(a.date_day).DayOfWeek).ToString();
                if (a.first == a.last)
                {
                    DateTime basetime = DateTime.Parse($"{a.date_day} 12:0:0");
                    if (a.first <= basetime)
                    {
                        //a.last = DateTime.Parse($"1900-01-01 0:0:0");
                        a.last_str = "";
                    }
                    else
                    {
                        //a.first = DateTime.Parse($"1900-01-01 0:0:0");
                        a.first_str = "";
                    }
                }
                //list.Add(a);
            }
            foreach (KeyValuePair<string, Att> item in dicTemp)
            {
                list.Add(item.Value);
            }
            return list;
        }
        public int AddLog(string uid, string text)
        {
            DBAtt db = new DBAtt();
            int res = db.ExeCMD($@"insert into user_log(uid,log_text,date) values('{uid}','{text}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
            db.Close();
            return res;
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
        /// <summary>
        /// 判断是不是周末/节假日
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>周末和节假日返回true，工作日返回false</returns>
        public static bool IsHolidayByDate(DateTime date)
        {
            var isHoliday = false;
            var webClient = new System.Net.WebClient();
            var PostVars = new System.Collections.Specialized.NameValueCollection
            {
                { "d", date.ToString("yyyyMMdd") }//参数
            };
            try
            {
                var day = date.DayOfWeek;
                string jsonData = $"{{ \"d\", {date.ToString("yyyyMMdd")} }}";//参数
                //判断是否为周末
                if (day == DayOfWeek.Sunday || day == DayOfWeek.Saturday)
                    return true;

                ////0为工作日，1为周末，2为法定节假日
                //ServicePointManager.Expect100Continue = false;
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://tool.bitefu.net/jiari/");
                //request.ServicePoint.Expect100Continue = false;//指定此属性为false
                //request.Method = "POST";
                //request.ContentType = "application/json";
                //Stream requestStream = request.GetRequestStream();
                //StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.GetEncoding("utf-8"));
                //streamWriter.Write(jsonData);
                //streamWriter.Flush();
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Stream responseStream = response.GetResponseStream();
                //StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                ////获取响应内容
                //using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                //{
                //    string result = reader.ReadToEnd();
                //    if (result == "1" || result == "2") isHoliday = true;
                //}



                //var byteResult = await webClient.UploadValuesTaskAsync("http://tool.bitefu.net/jiari/", "POST", PostVars);//请求地址,传参方式,参数集合
                // var result = Encoding.UTF8.GetString(byteResult);//获取返回值
                // if (result == "1" || result == "2")
                //     isHoliday = true;
            }
            catch
            {
                isHoliday = false;
            }
            return isHoliday;
        }
    }
}