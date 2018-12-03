using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarDLL;
using RelaxBarWeb_MVC.Models;
using System.Threading;
using RelaxBarWeb_MVC.Utils;
using RelexBarBLL.EnumCommon;

namespace RelaxBarWeb_MVC.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Filter.CheckLogin]
    public class AjaxIMController : Controller
    {
        /// <summary>
        /// 离线消息池
        /// </summary>
        public static Dictionary<Guid, OfflineMessage> OfflineMessage = new Dictionary<Guid, OfflineMessage>();
        // GET: AjaxIM
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <returns></returns>
        public JsonResult Init() {
            Users user = Session["user"] as Users;
            Guid offlineid = Guid.NewGuid();
            if (OfflineMessage.ContainsKey(user.ID))
            {
                OfflineMessage[user.ID].SetID(offlineid);
              //  OfflineMessage.Add(user.ID, new OfflineMessage(offlineid));
            }
            else
            {
                OfflineMessage.Add(user.ID, new OfflineMessage(offlineid));
            }
            return Json(new { code="1",msg=offlineid});
        }
        /// <summary>
        /// 客户端轮询方法
        /// </summary>
        /// <returns></returns>
        public JsonResult IM(Guid? imtoken)
        {
            Users user = Session["user"] as Users;//有过滤器，直接转换用户
            if (imtoken==null) {
                ChatMessage message = new ChatMessage();
                message.MID = Guid.NewGuid();
                message.FUID = Guid.Empty;
                message.TUID =user.ID;
                message.TType = (int)MessageTType.System;
                message.MType = (int)MessageType.LoginOut;
                message.Content = "请重新登陆";
                message.HadRead = 0;
                message.IsDel = 0;
                message.Status = 1;
                message.CreateTime = DateTime.Now;
                message.UpdateTime = DateTime.Now;
                List<ChatMessage> resplist = new List<ChatMessage>();
                resplist.Add(message);
                return new JsonResultPro(resplist,JsonRequestBehavior.AllowGet);
            }
            
            OfflineMessage offline;
            if (OfflineMessage.ContainsKey(user.ID))
            {
                offline = OfflineMessage[user.ID];

                
            }
            else
            {//如果用户没有离线消息池，添加
                offline = new OfflineMessage(imtoken.Value);
                OfflineMessage.Add(user.ID, offline);
            }
            DateTime now;
            offline.ConnectTime=now = DateTime.Now;
            if (offline.GetID()!=imtoken.Value) {
                ChatMessage message = new ChatMessage();
                message.MID = Guid.NewGuid();
                message.FUID = Guid.Empty;
                message.TUID = user.ID;
                message.TType = (int)MessageTType.System;
                message.MType = (int)MessageType.LoginOut;
                message.Content = "您已在其他地方登陆，请重新登陆";
                message.HadRead = 0;
                message.IsDel = 0;
                message.Status = 1;
                message.CreateTime = DateTime.Now;
                message.UpdateTime = DateTime.Now;
                List<ChatMessage> resplist = new List<ChatMessage>();
                resplist.Add(message);
                return new JsonResultPro(resplist,JsonRequestBehavior.AllowGet);
            }


            offline.Online = true;
            int count = 0;//检查消息次数
            List<ChatMessage> responselist = new List<ChatMessage>();
            
            while (count<20&&imtoken.Value==offline.GetID()&&offline.ConnectTime==now) {
                
                if (offline.MessageList.Count == 0)
                {
                    Thread.Sleep(500);//500毫秒检查一下消息池
                    count++;
                }
                else {
                    int mcount = offline.MessageList.Count;//消息数量
                    for (int i = 0; i < mcount; i++)
                    {
                        responselist.Add(offline.MessageList.First.Value);
                        offline.MessageList.RemoveFirst();
                    }
                    break;
                }
            }
            offline.Online = false;
            return new JsonResultPro(responselist, JsonRequestBehavior.AllowGet);
        }
    }
}