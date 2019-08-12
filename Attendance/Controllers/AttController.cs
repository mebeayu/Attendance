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
    }
}