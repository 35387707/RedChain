using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;
using RelexBarDLL;
using RelexBarBLL.EnumCommon;
namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class SystemController : BaseController
    {
        // GET: System
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SysMsg(int index = 1, int pageSize = 20)
        {
            ChatMessageBLL bll = new ChatMessageBLL();
            int sum;
            List<ChatMessage> list = bll.getMessageByTType(MessageTType.System, Guid.Empty, UserInfo.ID, index, pageSize, out sum);
            ViewData["index"] = index;
            ViewData["pageSize"] = pageSize;
            ViewData["totalPage"] = GetTotalPage(pageSize, sum);
            return View(list);
        }
        [Filter.NoFilter]
        public JsonResult GetVersion()
        {
            VersionListBLL bll = new VersionListBLL();
            //return Json(bll.GetNewVersion());
            return Json(new { code = 1, list = bll.GetNewVersion() });//获取新版本列表
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <returns></returns>
        [Filter.NoFilter]
        public JsonResult GetHelpList()
        {
            return Json(new
            {
                code = 1,
                list = new UserHelpBLL().GetUserHelpSearch(string.Empty, PageSize, PageIndex, out DataCount).Where(m=>m.Status==(int)Common.enStatus.Enabled)
            });
        }

        /// <summary>
        /// 获取帮助详情
        /// </summary>
        /// <returns></returns>
        [Filter.NoFilter]
        public JsonResult GetHelpDetail(Guid ID)
        {
            return Json(new
            {
                code = 1,
                model = new UserHelpBLL().GetUserHelpById(ID)
            });
        }
    }
}