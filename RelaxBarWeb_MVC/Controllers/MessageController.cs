using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;
using RelexBarDLL;
using RelaxBarWeb_MVC.Utils;
using System.Text.RegularExpressions;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class MessageController : BaseController
    {
        /// <summary>
        /// 聊天信息-用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChatInfo(Guid? id)
        {
            if (id == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            ViewData["isTop"] = new ChatsBLL().GetFriendisTop(id.Value, UserInfo.ID);
            Users fu = new UsersBLL().GetUserById(id.Value);
            return View(fu);
        }
        /// <summary>
        /// 设置与好友聊天的最后一条消息记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public void SetFriendLastMessageID(Guid FriendID)
        {
            FriendBLL bll = new FriendBLL();

            bll.SetFriendLastMessageID(UserInfo.ID, FriendID, null);
        }
        /// <summary>
        /// 设置群组聊天最够一条阅读记录
        /// </summary>
        /// <param name="GID"></param>
        public void SetGroupLastMessageID(Guid GID)
        {
            FriendBLL bll = new FriendBLL();

            bll.SetGroupLastMessageID(UserInfo.ID, GID, null);
        }
        /// <summary>
        /// 获取时间大于mid的我的消息
        /// </summary>
        /// <param name="MID"></param>
        /// <returns></returns>
        public JsonResult GetNewMessageByLastMID(Guid MID)
        {
            ChatMessageBLL bll = new ChatMessageBLL();
            List<ChatMessage> list = bll.GetNewMessageByLastMID(UserInfo.ID, MID);
            return Json2(list);
        }

        public JsonResult UndoMessageCount()
        {
            FriendBLL bll = new FriendBLL();
            int a = 0, b = 0;
            DateTime? saylt = null, reqlt = null;
            string sayname = "", reqname = "";

            bll.GetUndoRequestCount(UserInfo.ID, ref a, ref b, ref saylt, ref reqlt, ref sayname, ref reqname);
            return Json(new { code = "1", say = new { count = a, lt = saylt, content = sayname }, req = new { count = b, lt = reqlt, content = reqname } });
        }

        public JsonResult SystemMessage(DateTime? time)
        {
            ChatMessageBLL bll = new ChatMessageBLL();
            return Json(new { code = "1", time = DateTime.Now, list = bll.GetSystemMessage(UserInfo.ID, time) });
        }
    }

}