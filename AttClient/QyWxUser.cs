using Attendance.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace AttClient
{
    public class QyWxReturn
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public List<QyWxUser> userlist { get; set; }
    }
    public class QyWxUser
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
    }
    public class QyWxUserManage
    {
        public static List<QyWxUser> list = new List<QyWxUser>();
        public static DateTime expires_time = DateTime.Now.AddHours(-1);
        public static int UpdateQyWxUserList()
        {
            if(expires_time< DateTime.Now)
            {
                string access_token = AttBiz.GetAccessToken();
                string data = AttBiz.Get($"https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={access_token}&department_id=3&fetch_child=1");
                QyWxReturn dataObj = JsonConvert.DeserializeObject<QyWxReturn>(data);
                if (dataObj.errcode == 0)
                {
                    list = dataObj.userlist;
                    expires_time = DateTime.Now.AddDays(1);
                    return list.Count;
                }
                return 0;
            }
            
            return 0;
        }
        public static string GetQyWxUseridByMobile(string mobile)
        {
            UpdateQyWxUserList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].mobile==mobile)
                {
                    return list[i].userid;
                }
            }
            return "";
        }

    }
}
