using Attendance.Models;
using Common;
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
        public List<LeaveQuest> QueryLeave(LeaveQuest obj)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            
            DataSet ds = dboa.ExeQuery(@"SELECT b.LASTNAME,b.MOBILE,xjsq5,xjsq19,xjsq9,xjsq10,xjsq17,c.NOWNODETYPE from 
                (formtable_main_242 a left join HRMRESOURCE b on b.ID=a.xjsq5) 
                left join workflow_nownode c on a.REQUESTID=c.REQUESTID 
                where xjsq10>=:StartDate and xjsq10<=:EndDate and b.LASTNAME like :LASTNAME and c.NOWNODETYPE=3 order by b.LASTNAME ASC",
                new OracleParameter("StartDate", obj.StartDate),
                new OracleParameter("EndDate", obj.EndDate),
                new OracleParameter("LASTNAME", "%" + obj.LASTNAME + "%"));
           
            int n = ds.Tables[0].Rows.Count;
            List<LeaveQuest> list = new List<LeaveQuest>();
            for (int i = 0; i < n; i++)
            {
                LeaveQuest row = new LeaveQuest();
                row.uid = ds.Tables[0].Rows[i]["xjsq5"].ToString();
                row.MOBILE = ds.Tables[0].Rows[i]["MOBILE"].ToString();
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
                row.xjsq9 = type.ToString();
                row.xjsq10 = ds.Tables[0].Rows[i]["xjsq10"].ToString();
                row.xjsq17 = ds.Tables[0].Rows[i]["xjsq17"].ToString();
                row.NOWNODETYPE = int.Parse(ds.Tables[0].Rows[i]["NOWNODETYPE"].ToString());
                list.Add(row);

            }
            return list;
        }
        public List<Trip>  QueryTrip(Trip obj)
        {
            if (obj.LASTNAME == null)
            {
                obj.LASTNAME = "";
            }
            
            DataSet ds = dboa.ExeQuery(@"select b.LASTNAME,b.MOBILE,c.NOWNODETYPE,a.* from  
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
                trip.MOBILE = ds.Tables[0].Rows[i]["MOBILE"].ToString();
                trip.LASTNAME = ds.Tables[0].Rows[i]["LASTNAME"].ToString();
                trip.Title = ds.Tables[0].Rows[i]["CC3"].ToString();
                trip.Path = ds.Tables[0].Rows[i]["CC6"].ToString();
                trip.REQUESTID = ds.Tables[0].Rows[i]["REQUESTID"].ToString();
                trip.StartDate = ds.Tables[0].Rows[i]["CC4"].ToString();
                trip.EndDate = ds.Tables[0].Rows[i]["CC5"].ToString();
                DateTime s = DateTime.Parse(trip.StartDate);
                DateTime e = DateTime.Parse(trip.EndDate);
                TimeSpan timeSpan = e - s;
                trip.Days = timeSpan.Days + 1;
                list.Add(trip);
                string strIDS = ds.Tables[0].Rows[i]["CC8"].ToString();
                string[] arrIDs = strIDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < arrIDs.Length; j++)
                {
                    if (arrIDs[j] == trip.UID) continue;
                    Trip trip1 = new Trip();
                    trip1.UID = arrIDs[j];

                    DataSet ds1 = dboa.ExeQuery($@"select LASTNAME,MOBILE from HRMRESOURCE where id={trip1.UID}");
                    trip.MOBILE = ds1.Tables[0].Rows[0][1].ToString();
                    trip1.LASTNAME = ds1.Tables[0].Rows[0][0].ToString();
                    trip1.Title = trip.Title;
                    trip1.Path = trip.Path;
                    trip1.REQUESTID = trip.REQUESTID;
                    trip1.StartDate = trip.StartDate;
                    trip1.EndDate = trip.EndDate;
                    trip1.Days = trip.Days;
                    list.Add(trip1);
                }

            }
            
            return list;
        }
        public List<string> GetUIDinDate(string start_date, string end_date, List<Trip> list_trip)
        {
            List<string> arrUID = new List<string>();
            DataSet ds= dboa.ExeQuery($@"select distinct xjsq5 from formtable_main_242 where xjsq10>='{start_date}' and xjsq10<='{end_date}'");
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
            DataSet ds = dboa.ExeQuery($@"select LASTNAME,MOBILE from HRMRESOURCE where id={uid}");
            if (ds.Tables[0].Rows.Count==0)
            {
                return null;
            }
            Person p = new Person();
            p.UID = uid;
            p.LASTNAME = ds.Tables[0].Rows[0]["LASTNAME"].ToString();
            p.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
            ds = dboa.ExeQuery($@"select * from formtable_main_242 where xjsq5={uid} and xjsq10>='{start_date}' and xjsq10<='{end_date}'");
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
                int type = StringToInt(ds.Tables[0].Rows[i]["xjsq9"].ToString());
                dic[type] = dic[type] + StringToFloat(ds.Tables[0].Rows[i]["xjsq19"].ToString());
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