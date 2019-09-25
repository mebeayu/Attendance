using Attendance.Models;
using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Attendance.Common
{
    public static class DataCatche
    {
        private static DataSet _oa_dept_cache = null;
        private static DataSet _oa_user_cache = null;
        private static List<UserRel> _list_user_rel_cache = null;//用户对照
        private static DateTime expires_time_user_rel;
        private static DateTime expires_time_oa_user;
        private static DateTime expires_time_oa_dept;
        private static void init_oa_dept()
        {
            expires_time_oa_dept = DateTime.Now.AddDays(1);
            DBHR db190 = new DBHR();
            _oa_dept_cache = db190.ExeQuery("select * from oa_dept");
            db190.Close();
        }
        private static void init_user_rel()
        {
            expires_time_user_rel = DateTime.Now.AddDays(1);
            expires_time_oa_user = DateTime.Now.AddDays(1);
            DBAtt130 db130 = new DBAtt130();
            DataSet ds = null;
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
            DBOA dboa = new DBOA();//$@"select LASTNAME,MOBILE,DEPARTMENTNAME,LOGINID from HRMRESOURCE a left join HRMDEPARTMENT b on a.DEPARTMENTID = b.ID where a.id ={ uid}"
            _oa_user_cache = dboa.ExeQuery($@"select a.ID,LASTNAME,MOBILE,DEPARTMENTNAME,LOGINID,a.DEPARTMENTID from 
                                HRMRESOURCE a left join HRMDEPARTMENT b on a.DEPARTMENTID = b.ID");
            dboa.Close();
            string expression = "";      
            for (int i = 0; i < list_user_rel.Count; i++)
            {
                expression = $"MOBILE='{list_user_rel[i].mobile}'";
                DataRow[] rows = _oa_user_cache.Tables[0].Select(expression);
                if (rows.Length == 0)
                {
                    list_user_rel[i].oa_userid = "";
                }
                else
                {
                    list_user_rel[i].oa_userid = rows[0]["ID"].ToString();
                    list_user_rel[i].oa_login_id = rows[0]["LOGINID"].ToString();
                    list_user_rel[i].department = rows[0]["DEPARTMENTNAME"].ToString();
                    list_user_rel[i].oa_department_id = rows[0]["DEPARTMENTID"].ToString();
                }
            }
            db130.Close();
        }
        public static DataSet oa_dept_cache
        {
            get
            {
                if (_oa_dept_cache == null || expires_time_oa_dept <= DateTime.Now)
                {
                    init_oa_dept();
                    return _oa_dept_cache;
                }
                return _oa_dept_cache;
            }
        }
        public static DataSet oa_user_cache
        {
            get
            {
                if (_oa_user_cache==null||expires_time_oa_user<= DateTime.Now)
                {
                    init_user_rel();
                    return _oa_user_cache;
                }
                return _oa_user_cache;
            }
        }
        public static List<UserRel> list_user_rel_cache
        {
            get
            {
                if (_list_user_rel_cache == null)
                {
                    init_user_rel();
                    return _list_user_rel_cache;
                }
                if(expires_time_user_rel<=DateTime.Now)
                {
                    init_user_rel();
                    return _list_user_rel_cache;
                }
                return _list_user_rel_cache;
            }
        }
    }
}