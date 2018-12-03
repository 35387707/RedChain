
using RelaxBarWeb_MVC.IM;
using RelaxBarWeb_MVC.Models;
using RelaxBarWeb_MVC.Utils;
using RelexBarBLL;
using RelexBarBLL.EnumCommon;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RelaxBarWeb_MVC.Controllers
{
    public class BaseController : Controller
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (filterContext.HttpContext.Session["user"] == null)
        //        filterContext.Result = new RedirectToRouteResult("Login", new RouteValueDictionary { { "from", Request.Url.ToString() } });

        //    base.OnActionExecuting(filterContext);
        //}
        private Users _userinfo = null;
        public Users UserInfo
        {
            get
            {
                if (_userinfo == null)
                {
                    if (Session["user"] != null)
                        _userinfo = Session["user"] as Users;
                }
                return _userinfo;
            }
            set
            {
                _userinfo = value;
                Session["user"] = _userinfo;
            }
        }
        public string VerifyCode
        {
            get
            { return Session["verifycode"] != null ? Session["verifycode"].ToString() : string.Empty; }
            set
            {
                Session["verifycode"] = value;
            }
        }
        /// <summary>
        /// 告诉uid有新消息
        /// </summary>
        /// <param name="uid"></param>
        [NonAction]
        public void HasNewMsg(Guid uid)
        {
            if (APPController.serviceList.ContainsKey(uid))
            {
                APPController.serviceList[uid].HasMessage = true;
            }
        }
        [NonAction]
        public JsonResult RJson(string icode, string imsg)
        {
            return Json(new { code = icode, msg = imsg });
        }
        [NonAction]
        public JsonResultPro Json2(object o, JsonRequestBehavior? j = null)
        {
            return new JsonResultPro(o, JsonRequestBehavior.AllowGet);
        }
        [NonAction]
        public JsonResultPro JsonPro(object o, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            return new JsonResultPro(o, JsonRequestBehavior.AllowGet, dateTimeFormat);
        }
        [NonAction]
        public static string MD5(string source)
        {
            return CommonClass.EncryptDecrypt.GetMd5Hash(source + SysConfigBLL.MD5Key);
        }
        [NonAction]
        public static string encodeHtml(string content)
        {
            if (content.IndexOf("<") != -1)
            {
                content = content.Replace("<", "~lt");
            }
            if (content.IndexOf(">") != -1)
            {
                content = content.Replace(">", "~gt");
            }
            //if (content.IndexOf("\n")!=-1) {
            //    content = content.Replace("\n","<br/>");
            //}
            return content;
        }
        [NonAction]
        public PushModel GetPushMessage(Guid sender, Guid tuid, MessageTType ttype, MessageType type, string msg)
        {
            PushModel m = new PushModel();
            m.Title = "系统消息";
            string name = string.Empty;
            string text = string.Empty;
            switch (ttype)
            {
                case MessageTType.User:
                    name = new UsersBLL().GetName(sender);
                    break;
                case MessageTType.Group:
                    name = new ChatsBLL().GetGroupName(sender);
                    break;
                case MessageTType.System:
                    break;
                case MessageTType.Gzh:
                    break;
                default:
                    break;
            }
            switch (type)
            {
                case MessageType.LoginOut:
                    return null;
                case MessageType.RedMsg:
                    return null;
                case MessageType.Revoke:
                    return null;
                case MessageType.Text:
                    text = msg;
                    break;
                case MessageType.Image:
                    text = "[您收到了一张图片]";
                    break;
                case MessageType.Red:
                    text = "[您收到一个红包]";
                    break;
                case MessageType.Transfer:
                    break;
                case MessageType.MyCard:
                    text = "[您收到了一张名片]";
                    break;
                case MessageType.HeartBeat:
                    return null;
                case MessageType.Link:
                    text = msg;
                    break;
                case MessageType.Video:
                    text = "[视频]";
                    break;
                case MessageType.Voice:
                    text = "[语音]";
                    break;
                case MessageType.SLRed:
                    text = "[扫雷红包]";
                    break;
                case MessageType.FriendRequest:
                    text = "[好友申请]";
                    break;
                case MessageType.FriendRequestNotice:
                    return null;
                case MessageType.DropGroup:
                    return null;
                case MessageType.JoinGroupNotice:
                    return null;
                case MessageType.ExitGroupNotice:
                    return null;
                case MessageType.RemoveGroupUser:
                    return null;
                case MessageType.ShouKuanOK:
                    text = "[收款通知]";
                    break;
                case MessageType.DeleteFriend:
                    return null;
                case MessageType.TransferSuccess:
                    return null;
                default:
                    return null;
            }
            m.Content = string.Format("{0}：{1}", name, text);
            return m;
        }
        /// <summary>
        /// 发送消息方法 方法有重载
        /// </summary>
        /// <param name="FUID">发送者id</param>
        /// <param name="TUID">接收者id</param>
        /// <param name="ttype">1用户，2群，3系统消息，4关注号消息</param>
        /// <param name="type">0文字/表情，1图片，6链接，4个人名片，5心跳包，7视频,8语音,2普通红包，9扫雷红包，3转账,10好友申请,9其他等</param>
        /// <param name="content"></param>
        /// <returns></returns>
        [NonAction]
        public int SendMessage(Guid FUID, Guid TUID, MessageTType ttype, MessageType type, string content, Guid? MID = null)
        {
            /*网页版逻辑
            ChatMessage msg = new ChatMessage();
            msg.MID =MID==null?Guid.NewGuid():MID.Value;
            msg.FUID = FUID;
            msg.TUID = TUID;
            msg.TType = (int)ttype;
            msg.MType = (int)type;
            msg.Content = encodeHtml(content);
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            ChatsBLL bll = new ChatsBLL();
            int i= bll.AddChatMessage(msg);
            if (i > 0)
            {
                if (AjaxIMController.OfflineMessage.ContainsKey(TUID))
                {
                    OfflineMessage offline = AjaxIMController.OfflineMessage[TUID];
                    offline.MessageList.AddLast(msg);
                }
            }
            return i;
            */

            //ChatMessage msg = new ChatMessage();
            //msg.MID = MID == null ? Guid.NewGuid() : MID.Value;
            //msg.FUID = FUID;
            //msg.TUID = TUID;
            //msg.TType = (int)ttype;
            //msg.MType = (int)type;
            //msg.Content = encodeHtml(content);
            //msg.HadRead = 0;
            //msg.IsDel = 0;
            //msg.Status = 1;
            //msg.CreateTime = DateTime.Now;
            //msg.UpdateTime = DateTime.Now;
            //ChatsBLL bll = new ChatsBLL();
            //int i = bll.AddChatMessage(msg);
            //if (i > 0)
            //    if (APPController.serviceList.ContainsKey(TUID))
            //    {
            //        APPController.serviceList[TUID].HasMessage = true;
            //        APPController.serviceList[TUID].AddPushMsg(GetPushMessage(FUID, TUID, ttype, type, msg.Content));
            //    }
            //return 1;

            //Message m = new Message("sendToUser", FUID, TUID, ttype, type, MessageStatus.Success, DateTime.Now, content);
            //IMController.sendToUser(m);

            //return 1;

            ChatMessage msg = new ChatMessage();
            msg.MID = MID == null ? Guid.NewGuid() : MID.Value;
            msg.FUID = FUID;
            msg.TUID = TUID;
            msg.TType = (int)ttype;
            msg.MType = (int)type;
            msg.Content = encodeHtml(content);
            msg.HadRead = msg.FUID == Guid.Empty ? (int)Common.enStatus.Enabled : (int)Common.enStatus.Unabled;//如果是系统消息，则自动默认该消息为已读
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            ChatsBLL bll = new ChatsBLL();
            int i = bll.AddChatMessage(msg);
            if (i > 0)
            {
                var toService = APPIMController.Get(TUID);
                if (toService != null)
                {
                    toService.HasMessage = true;//通知有新消息
                    //APPController.serviceList[TUID].AddPushMsg(GetPushMessage(FUID, TUID, ttype, type, msg.Content));
                }
            }
            return 1;
        }
        /// <summary>
        /// 消息转发
        /// </summary>
        /// <param name="UID">发送者</param>
        /// <param name="MID"></param>
        /// <param name="UIDS">接收者</param>
        /// <param name="TType">类型</param>
        /// <returns></returns>
        [NonAction]
        public int ForwardMessage(Guid UID, Guid MID, Guid[] UIDS, int[] TType)
        {
            /*网页版代码
            try
            {
                ChatsBLL bll = new ChatsBLL();
                ChatMessageBLL mbll = new ChatMessageBLL();

                ChatMessage msg = mbll.Get(MID);
                if (msg == null)
                {
                    return -1;
                }
                
                for (int i = 0; i < UIDS.Length; i++)
                {
                    ChatMessage smsg = new ChatMessage();

                    smsg.CreateTime = DateTime.Now;
                    smsg.UpdateTime = DateTime.Now;
                    smsg.MID = Guid.NewGuid();
                    smsg.FUID = UID;
                    smsg.TUID = UIDS[i];
                    smsg.TType = TType[i];
                    smsg.MType = msg.MType;
                    smsg.Content = msg.Content;
                    smsg.Status = 1;
                    smsg.IsDel = 0;
                    smsg.HadRead = 0;
                    int j = bll.AddChatMessage(smsg);
                    if (TType[i] == (int)MessageTType.User)
                    {//如果是用户
                        
                        if (j > 0)
                        {
                            if (AjaxIMController.OfflineMessage.ContainsKey(UIDS[i]))
                            {
                                OfflineMessage offline = AjaxIMController.OfflineMessage[UIDS[i]];
                                offline.MessageList.AddLast(smsg);
                            }
                        }
                    }
                    else if(TType[i]==(int)MessageTType.Group){//如果是群组
                        List<Guid> UIDS2 = bll.GetGroupUID(UIDS[i]);
                        foreach (Guid item in UIDS2)
                        {
                            if (AjaxIMController.OfflineMessage.ContainsKey(item))
                            {
                                OfflineMessage offline = AjaxIMController.OfflineMessage[item];
                                offline.MessageList.AddLast(smsg);
                            }
                        }
                    }

                    
                }
            }
            catch (Exception)
            {

                return -1;
            }
            */
            try
            {
                ChatsBLL bll = new ChatsBLL();
                ChatMessageBLL mbll = new ChatMessageBLL();

                ChatMessage msg = mbll.Get(MID);
                if (msg == null)
                {
                    return -1;
                }

                for (int i = 0; i < UIDS.Length; i++)
                {
                    ChatMessage smsg = new ChatMessage();

                    smsg.CreateTime = DateTime.Now;
                    smsg.UpdateTime = DateTime.Now;
                    smsg.MID = Guid.NewGuid();
                    smsg.FUID = UID;
                    smsg.TUID = UIDS[i];
                    smsg.TType = TType[i];
                    smsg.MType = msg.MType;
                    smsg.Content = msg.Content;
                    smsg.Status = 1;
                    smsg.IsDel = 0;
                    smsg.HadRead = 0;
                    int j = bll.AddChatMessage(smsg);
                    if (TType[i] == (int)MessageTType.User)
                    {//如果是用户

                        if (j > 0)
                        {
                            if (APPController.serviceList.ContainsKey(UIDS[i]))
                            {
                                IMService service = APPController.serviceList[UIDS[i]];
                                service.HasMessage = true;
                                if (UID != UIDS[i])
                                    service.AddPushMsg(GetPushMessage(UID, UIDS[i], (MessageTType)TType[i], (MessageType)msg.MType, msg.Content));
                            }
                        }
                    }
                    else if (TType[i] == (int)MessageTType.Group)
                    {//如果是群组
                        List<Guid> UIDS2 = bll.GetGroupUID(UIDS[i]);
                        foreach (Guid item in UIDS2)
                        {
                            if (APPController.serviceList.ContainsKey(item))
                            {
                                IMService service = APPController.serviceList[item];
                                service.HasMessage = true;
                                if (UID != item)
                                    service.AddPushMsg(GetPushMessage(UID, item, (MessageTType)TType[i], (MessageType)msg.MType, msg.Content));
                            }
                        }
                    }


                }
            }
            catch (Exception)
            {

                return -1;
            }


            return 1;
        }

        [NonAction]
        public int SendMessage(Guid UID, Guid RUID, MessageTType ttype, MessageType type, string content, out Guid MID)
        {
            /*网页版代码
            ChatMessage msg = new ChatMessage();
            msg.MID = Guid.NewGuid();
            msg.FUID = UID;
            msg.TUID = RUID;
            msg.TType = (int)ttype;
            msg.MType = (int)type;
            msg.Content = encodeHtml(content);
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            ChatsBLL bll = new ChatsBLL();
            int i = bll.AddChatMessage(msg);
            if (i > 0)
            {
                if (AjaxIMController.OfflineMessage.ContainsKey(RUID))
                {
                    OfflineMessage offline = AjaxIMController.OfflineMessage[RUID];
                    offline.MessageList.AddLast(msg);
                }
            }
            MID = msg.MID;
            return i;
            */
            ChatMessage msg = new ChatMessage();
            msg.MID = Guid.NewGuid();
            msg.FUID = UID;
            msg.TUID = RUID;
            msg.TType = (int)ttype;
            msg.MType = (int)type;
            msg.Content = encodeHtml(content);
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            ChatsBLL bll = new ChatsBLL();
            int i = bll.AddChatMessage(msg);
            if (i > 0)
            {
                if (APPController.serviceList.ContainsKey(RUID))
                {
                    IMService service = APPController.serviceList[RUID];
                    service.HasMessage = true;
                    service.AddPushMsg(GetPushMessage(UID, RUID, ttype, type, msg.Content));
                }
            }
            MID = msg.MID;
            return i;
        }

        public int SendGroupMessage(Guid MID, Guid UID, Guid GID, MessageType type, string content, bool saveDB = true)
        {
            /*网页版代码
            ChatsBLL bll = new ChatsBLL();
            List<Guid> UIDS = bll.GetGroupUID(GID);
            ChatMessage msg = new ChatMessage();
            msg.MID = MID;
            msg.FUID = UID;
            msg.TUID = GID;
            msg.TType = (int)MessageTType.Group;
            msg.MType = (int)type;
            msg.Content = content;
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            int i = bll.AddChatMessage(msg);
            if (i>0) {
                foreach (Guid item in UIDS)
                {
                    if (AjaxIMController.OfflineMessage.ContainsKey(item))
                    {
                        OfflineMessage offline = AjaxIMController.OfflineMessage[item];
                        offline.MessageList.AddLast(msg);
                    }
                }
            }
            return i;
            */
            ChatsBLL bll = new ChatsBLL();
            List<Guid> UIDS = bll.GetGroupUID(GID);
            ChatMessage msg = new ChatMessage();
            msg.MID = MID;
            msg.FUID = UID;
            msg.TUID = GID;
            msg.TType = (int)MessageTType.Group;
            msg.MType = (int)type;
            msg.Content = content;
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            int i = 0;
            if (saveDB)
                i = bll.AddChatMessage(msg);
            else
                i = 1;
            if (i > 0)
            {
                foreach (Guid item in UIDS)
                {
                    if (APPController.serviceList.ContainsKey(item))
                    {
                        IMService service = APPController.serviceList[item];
                        service.HasMessage = true;
                        if (UID != item)
                            service.AddPushMsg(GetPushMessage(UID, item, MessageTType.Group, type, msg.Content));
                    }
                }
            }
            return i;
        }

        public int GetTotalPage(int pageSize, int Sum)
        {
            return ((Sum - 1) / pageSize + 1);
        }
        protected bool CheckPayPSW(string psw)
        {
            UsersBLL bll = new UsersBLL();
            int result = bll.CheckPay(UserInfo.ID, psw);
            return result > 0;
        }
        /// <summary>
        /// 发送群组消息不存库
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="UID"></param>
        /// <param name="GID"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public void SendGroupMessageNoDB(Guid MID, Guid UID, Guid GID, MessageType type, string content, MessageTType ttype = MessageTType.Group)
        {
            /*网页版代码
            ChatsBLL bll = new ChatsBLL();
            List<Guid> UIDS = bll.GetGroupUID(GID);
            ChatMessage msg = new ChatMessage();
            msg.MID = MID;
            msg.FUID = UID;
            msg.TUID = GID;
            msg.TType = (int)MessageTType.Group;
            msg.MType = (int)type;
            msg.Content = content;
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            
            foreach (Guid item in UIDS)
            {
                if (AjaxIMController.OfflineMessage.ContainsKey(item))
                {
                    OfflineMessage offline = AjaxIMController.OfflineMessage[item];
                    offline.MessageList.AddLast(msg);
                }
            }
            */
            ChatsBLL bll = new ChatsBLL();
            List<Guid> UIDS = bll.GetGroupUID(GID);
            ChatMessage msg = new ChatMessage();
            msg.MID = MID;
            msg.FUID = UID;
            msg.TUID = GID;
            msg.TType = (int)ttype;
            msg.MType = (int)type;
            msg.Content = content;
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            int i = bll.AddChatMessage(msg);
            if (i > 0)
            {
                foreach (Guid item in UIDS)
                {
                    if (APPController.serviceList.ContainsKey(item))
                    {
                        IMService service = APPController.serviceList[item];
                        service.HasMessage = true;
                        if (UID != item)
                            if (UID != item)
                                service.AddPushMsg(GetPushMessage(UID, item, ttype, type, msg.Content));
                    }
                }
            }
        }
        /// <summary>
        /// 不存库
        /// </summary>
        /// <param name="FUID"></param>
        /// <param name="TUID"></param>
        /// <param name="ttype"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="MID"></param>
        /// <returns></returns>
        public void SendMessageNoDB(Guid FUID, Guid TUID, MessageTType ttype, MessageType type, string content, Guid? MID = null)
        {
            /*
            ChatMessage msg = new ChatMessage();
            msg.MID = MID == null ? Guid.NewGuid() : MID.Value;
            msg.FUID = FUID;
            msg.TUID = TUID;
            msg.TType = (int)ttype;
            msg.MType = (int)type;
            msg.Content = encodeHtml(content);
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            if (AjaxIMController.OfflineMessage.ContainsKey(TUID))
            {
                OfflineMessage offline = AjaxIMController.OfflineMessage[TUID];
                offline.MessageList.AddLast(msg);
            }
            */
            ChatsBLL bll = new ChatsBLL();
            ChatMessage msg = new ChatMessage();
            msg.MID = MID == null ? Guid.NewGuid() : MID.Value;
            msg.FUID = FUID;
            msg.TUID = TUID;
            msg.TType = (int)ttype;
            msg.MType = (int)type;
            msg.Content = encodeHtml(content);
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = DateTime.Now;
            msg.UpdateTime = DateTime.Now;
            int i = bll.AddChatMessage(msg);
            if (i > 0)
            {
                if (APPController.serviceList.ContainsKey(TUID))
                {
                    IMService service = APPController.serviceList[TUID];
                    service.HasMessage = true;
                    service.AddPushMsg(GetPushMessage(FUID, TUID, ttype, type, msg.Content));
                }
            }
        }
        [NonAction]
        public ActionResult AlertAndLinkTo(string msg, string url)
        {
            return Content("<script>alert(\"" + msg + "\");location.href='" + url + "';</script>");
        }

        private int _pagesize = 20;
        public int PageSize
        {
            set { _pagesize = value; }
            get { return _pagesize; }
        }
        public int PageIndex
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["pageindex"]))
                {
                    return int.Parse(Request.QueryString["pageindex"]);
                }
                else if (!string.IsNullOrEmpty(Request["pageindex"]))
                {
                    return int.Parse(Request["pageindex"]);
                }
                return 1;
            }
        }
        public int DataCount = 0;
        public int TotalPageCount
        {
            get
            {
                int dd = 0, maintain = 0;
                if (DataCount > 0)
                {
                    dd = DataCount / PageSize;
                    maintain = DataCount % PageSize;
                }
                return maintain > 0 ? ++dd : dd;//有余数，则加1
            }
        }
    }
}