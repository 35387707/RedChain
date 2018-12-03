using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RelaxBarWeb_MVC.Controllers
{
    public class StaticController : Controller
    {
        // GET: Static
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SaoleiRule() {
            return View();
        }
        /// <summary>
        /// 同步遮罩层
        /// </summary>
        /// <returns></returns>
        public ActionResult SyncView()
        {
            return PartialView();
        }
        public ActionResult CommonHead() {
            return PartialView();
        }
        public ActionResult ChatCommon() {
            return PartialView();
        }
    }
}