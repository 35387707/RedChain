using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;

namespace RelaxBarWeb_MVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Save(string company, string name, string phone, string descr)
        {
            ProblemBLL p = new ProblemBLL();
            p.ContactUs(company, name, phone, descr);
            return Json(1);
        }

        public ActionResult IIndex()
        {
            return View();
        }


    }
}