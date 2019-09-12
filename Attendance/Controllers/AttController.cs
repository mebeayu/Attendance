using Attendance.Common;
using Attendance.Models;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attendance.Controllers
{
    public class AttController : Controller
    {
        // GET: Att
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StcOALeave()
        {
            return View();
        }
        public ActionResult StcOATrip()
        {
            return View();
        }
        public ActionResult StcAttr()
        {
            return View();
        }
        public ActionResult StcAttrOld()
        {
            return View();
        }
        public ActionResult ShowDetail()
        {
            return View();
        }
        public ActionResult PointOA()
        {
            return View();
        }
        
        public ActionResult AuthWX()
        {
            string redirect_uri = System.Web.HttpUtility.UrlEncode("http://kq.ynctzy.com:86/Att/AuthRedirect");
            string url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={AttBiz.corpid}&redirect_uri={redirect_uri}&response_type=code&scope=snsapi_base&state=kq_auth#wechat_redirect";
            Response.Redirect(url);

            return null;
        }
        
        
        public ActionResult AuthRedirect()
        {
            string code = Request.Params["code"];
            string access_token = AttBiz.GetAccessToken();
            //Response.Write(JsonConvert.SerializeObject(access_token_obj_cache));
            string request_uri = $"https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={access_token}&code={code}";
            string user_str = AttBiz.Get(request_uri);
            //Response.Write("<br/><br/><br/>" + request_uri + "<br/><br/><br/>" + user_str);
            dynamic wx_user = JsonConvert.DeserializeObject<dynamic>(user_str);
            string user_id = wx_user.UserId;
            Response.Write(user_id);
            user_str = AttBiz.Get($" https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={access_token}&userid={user_id}");
            wx_user = JsonConvert.DeserializeObject<dynamic>(user_str);
            string mobile = wx_user.mobile;
            Response.Write(mobile);
            try
            {
                AttBiz att = new AttBiz();
                string login_id = att.GetOALoginIDByIMobile(mobile);
                att.Close();
                Response.Write(login_id);
                Response.Redirect($"/Att/PointOA?oa_login_id={login_id}");
                Response.End();
            }
            catch (Exception e)
            {

                Response.Write(e.Message+e.StackTrace);
            }
            
            
            return null;
        }
        
    }
}