using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace Common
{
    public class DBHR : DBSQL
    {
        public DBHR()
        {
            this.ConnectSQLServerDB("192.168.27.190", "ehr", "admin", "CTzy60023925");//HR正式环境
            //this.ConnectSQLServerDB("192.168.27.130", "ehr", "sa", "CTzy60023925");//HR测试环境
        }
    }
    public class DBOA:DBOrc
    {
        private string connStr = "User Id=ecology;Password=ecology;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.27.185)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ORCL)))";
        public DBOA()
        {
            this.Connect(connStr);
        }
    }
    public class DB: DBSQL
    {
        public DB()
        {
            //this.ConnectSQLServerDB("127.0.0.1", "Contract", "sa", "01161036");
            this.ConnectSQLServerDB("192.168.27.190", "Contract", "admin", "CTzy60023925");
        }
    }
    public class DBSQL
    {
        SqlConnection conn;
        SqlCommand sm;
        public DBSQL()
        {
        }
        public DBSQL(string strConnectString)
        {
            //"Data Source=10.43.4.16;Initial Catalog=zbb;Persist Security Info=True;User ID=jfsys;Password=jfsys123"
            conn = new SqlConnection(strConnectString);
            conn.Open();
            sm = new SqlCommand("", conn);
        }
        public DBSQL(string strAddr, string DBName, string ID, string Password)
        {
            conn = new SqlConnection(string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", strAddr, DBName, ID, Password));
            conn.Open();
            sm = new SqlCommand("", conn);
        }
        public void ConnectSQLServerDB(string strAddr, string DBName, string ID, string Password)
        {
            conn = new SqlConnection(string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", strAddr, DBName, ID, Password));
            conn.Open();
            sm = new SqlCommand("", conn);
        }
        public void Close()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public DataSet ExeQuery(string sql)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        public DataSet ExeQuery(string sql, params SqlParameter[] AParams)
        {
            try
            {
                sm.Parameters.Clear();
                sm.CommandText = sql;
                sm.Parameters.AddRange(AParams);
                //foreach (SqlParameter param in AParams)
                //{
                //    sm.Parameters.Add(param);
                //}
                SqlDataAdapter Adapter = new SqlDataAdapter(sm);
                DataSet Result = new DataSet();
                Adapter.Fill(Result);
                return Result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void BeginTran()
        {
            sm.Transaction = conn.BeginTransaction();


        }
        public void RollBackTran()
        {
            sm.Transaction.Rollback();
        }
        public void CommitTran()
        {
            sm.Transaction.Commit();
        }
        public int ExeCMD(string sql)
        {
            try
            {
                sm.CommandText = sql;
                int res = sm.ExecuteNonQuery();
                return res;
            }
            catch
            {
                return -1;
            }
        }
        public int ExeCMD(string sql, params SqlParameter[] AParams)
        {
            try
            {
                sm.Parameters.Clear();
                sm.CommandText = sql;
                sm.Parameters.AddRange(AParams);
                //foreach (SqlParameter param in AParams)
                //{
                //    sm.Parameters.Add(param);
                //}
                return sm.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                // string s = e.Message;
                return -1;
            }


        }
    }

    public class DBOrc
    {
        OracleConnection conn;
        OracleCommand sm;
        public DBOrc()
        {
        }
        public DBOrc(string strConnectString)
        {
            //"Data Source=10.43.4.16;Initial Catalog=zbb;Persist Security Info=True;User ID=jfsys;Password=jfsys123"
            conn = new OracleConnection(strConnectString);
            conn.Open();
            sm = new OracleCommand("", conn);
        }
        public void Connect(string strConnectString)
        {
            conn = new OracleConnection(strConnectString);
            conn.Open();
            sm = new OracleCommand("", conn);
        }
        public void Close()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public DataSet ExeQuery(string sql)
        {
            try
            {
                OracleDataAdapter da = new OracleDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public DataSet ExeQuery(string sql, params OracleParameter[] AParams)
        {
            try
            {
                sm.Parameters.Clear();
                sm.CommandText = sql;
                sm.Parameters.AddRange(AParams);
                //foreach (SqlParameter param in AParams)
                //{
                //    sm.Parameters.Add(param);
                //}
                OracleDataAdapter Adapter = new OracleDataAdapter(sm);
                DataSet Result = new DataSet();
                Adapter.Fill(Result);
                return Result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void BeginTran()
        {
            sm.Transaction = conn.BeginTransaction();


        }
        public void RollBackTran()
        {
            sm.Transaction.Rollback();
        }
        public void CommitTran()
        {
            sm.Transaction.Commit();
        }
        public int ExeCMD(string sql)
        {
            try
            {
                sm.CommandText = sql;
                int res = sm.ExecuteNonQuery();
                return res;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public int ExeCMD(string sql, params OracleParameter[] AParams)
        {
            try
            {
                sm.Parameters.Clear();
                sm.CommandText = sql;
                sm.Parameters.AddRange(AParams);
                //foreach (SqlParameter param in AParams)
                //{
                //    sm.Parameters.Add(param);
                //}
                return sm.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                // string s = e.Message;
                return -1;
            }


        }

    }
}
