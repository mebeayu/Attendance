using Attendance.Common;
using Attendance.Models;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttClient
{
    public partial class Form1 : Form
    {
        private DateTime time_line = DateTime.Now;
        private bool IsStop = false;
        Thread thread;
        DBAtt130 db = new DBAtt130();
        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {

            QyWxUserManage.UpdateQyWxUserList();
            thread = new Thread(Watch);
            thread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsStop = true;
            thread.Abort();
        }
        public void Watch()
        {
            DataSet ds = null;
            UpdateText($"--------------{time_line.ToString("yyyy-MM-dd HH:mm:ss")}---------------\r\n");
            bool hasData = false;
            int lineIndex = 1;
            while (IsStop == false)
            {
                try
                {
                    
                    ds = db.ExeQuery($@"SELECT  a.[USERID],b.NAME,b.PAGER
                                      ,[CHECKTIME]
                                      ,[CHECKTYPE]
                                      , a.[VERIFYCODE]
                                      ,[SENSORID]
                                      ,[Memoinfo]
                                      ,[WorkCode]
                                      ,[sn]
                                      ,[UserExtFmt]
                                  FROM[att].[dbo].[CHECKINOUT] a left join USERINFO b on b.USERID = a.USERID 
                                    where a.CHECKTIME>'{time_line.ToString("yyyy-MM-dd HH:mm:ss")}' order by a.CHECKTIME asc");
                    hasData = false;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string mobile = ds.Tables[0].Rows[i]["PAGER"].ToString();
                        DateTime CHECKTIME = DateTime.Parse(ds.Tables[0].Rows[i]["CHECKTIME"].ToString());
                        string SENSORID = ds.Tables[0].Rows[i]["SENSORID"].ToString();
                        string userid = QyWxUserManage.GetQyWxUseridByMobile(mobile);
                        if (userid != "")
                        {
                            QywxMessage msgObj = new QywxMessage();
                            msgObj.touser = userid;
                            msgObj.msgtype = "text";
                            msgObj.text = new text();
                            msgObj.text.content = $"最近打卡 {GetPos(SENSORID)} 时间:{CHECKTIME.ToString("yyyy-MM-dd HH:mm:ss")}";
                            msgObj.agentid = 1000018;
                            string access_token = AttBiz.GetAccessToken();
                            string url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={access_token}";
                            //dynamic res = JsonConvert.DeserializeObject<dynamic>(AttBiz.Post(url, msgObj));
                            AttBiz.Post(url, msgObj);


                        }
                        hasData = true;
                        time_line = CHECKTIME;
                        UpdateText($"{lineIndex}. {ds.Tables[0].Rows[i]["NAME"].ToString()} {GetPos(SENSORID)} 打卡时间 {CHECKTIME.ToString("yyyy-MM-dd HH:mm:ss")} \r\n");
                        lineIndex++;
                    }
                    Thread.Sleep(10000);
                    if (hasData)
                    {
                        UpdateText($"{lineIndex}. --------------{time_line.ToString("yyyy-MM-dd HH:mm:ss")}---------------\r\n");
                        lineIndex++;
                    }
                }
                catch (Exception ex)
                {
                    UpdateText(ex.Message + "\r\n");
                    Thread.Sleep(10000);
                    try { db.Close(); } catch { }
                    db = new DBAtt130();

                }
                
                
                
            }
           
        }
        private string GetPos(string SENSORID)
        {
            if (SENSORID == "101") return "-2楼";
            if (SENSORID == "102") return "1楼";
            if (SENSORID == "103") return "LG楼";
            if (SENSORID == "104") return "-1楼";

            return "";
        }
        private delegate void InvokeCallback(string msg); //定义回调函数（代理）格式
        //Invoke回调函数
        public void UpdateText(string text)
        {
            if (text_Info.InvokeRequired)//当前线程不是创建线程
                text_Info.Invoke(new InvokeCallback(UpdateText), new object[] { text });//回调
            else//当前线程是创建线程（界面线程）
            {
                int max = 201;
                text_Info.Text += text;//直接更新
                if(text_Info.Lines.Length> max)
                {
                    int index = text_Info.Text.IndexOf('\n');
                    text_Info.Text = text_Info.Text.Remove(0, index + 1);//删除第一行
                }
                text_Info.Select(text_Info.Text.Length, 0);
                text_Info.ScrollToCaret();

            }
        }
    }
}
