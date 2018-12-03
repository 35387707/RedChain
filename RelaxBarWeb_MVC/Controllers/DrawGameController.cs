using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    /// <summary>
    /// 抽奖相关
    /// </summary>
    public class DrawGameController : BaseController
    {
        /// <summary>
        /// 发起抽奖
        /// </summary>
        /// <returns></returns>
        public JsonResult CreateDrawgame(string img, string name, int count, int opentype, DateTime? opentime)
        {
            return Json("");
        }


    }
}