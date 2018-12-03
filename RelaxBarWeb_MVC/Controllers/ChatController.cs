using RelexBarBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL.EnumCommon;
using RelexBarDLL;
using RelexBarBLL.Models;
using Newtonsoft.Json;
using RelaxBarWeb_MVC.Utils;
using RelaxBarWeb_MVC.Models;
using System.Threading.Tasks;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class ChatController : BaseController
    {
        /// <summary>
        /// 单人聊天  需要清空离线消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lastMID"></param>
        /// <returns></returns>
        public ActionResult ToOne(Guid? id, Guid? lastMID)
        {
            if (id == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            //Users fu = new UsersBLL().GetUserById(id.Value);
            FriendShip f = new FriendBLL().GetFriend(UserInfo.ID, id.Value);
            if (f == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            Users friend = new UsersBLL().GetUserById(f.FriendID);
            string token = MD5(UserInfo.ID + UserInfo.Psw);
            ViewData["id"] = UserInfo.ID;
            ViewData["token"] = token;
            ViewData["fHeadImg"] = friend.HeadImg1;
            ViewData["name"] = friend.Name;
            ViewData["HeadImg"] = UserInfo.HeadImg1;
            ViewData["lastMID"] = lastMID == null ? "" : lastMID.Value.ToString();
            return View(f);
        }
        /// <summary>
        /// 聊天群
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChatGroup(Guid id, Guid? lastMID)
        {
            ChatsBLL cbll = new ChatsBLL();
            string token = MD5(UserInfo.ID + UserInfo.Psw);
            ViewData["id"] = UserInfo.ID;
            ViewData["token"] = token;
            ViewData["HeadImg"] = UserInfo.HeadImg1;
            ViewData["lastMID"] = lastMID == null ? "" : lastMID.Value.ToString();
            ViewData["groupCount"] = cbll.GetGroupUsersCount(id);
            ChatGroup group = cbll.GetGroupDetail(id);
            if (group.Gtype != (int)GType.ChatGroup)
            {
                return Redirect("/Chat/SLChatGroup/" + id);
            }
            ViewData["groupHeadImg"] = new UsersBLL().GetGroupUserHeadImg(id);
            ViewData["userCount"] = cbll.GetGroupUsersCount(id);
            if (cbll.ExistGroup(UserInfo.ID, id))
            {
                return View(group);
            }
            else
            {
                return View(group);//如果不存在跳转到首页，以后修改
            }
        }
        /// <summary>
        /// 扫雷房间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SLChatGroup(Guid id, Guid? lastMID)
        {
            ChatsBLL cbll = new ChatsBLL();
            string token = MD5(UserInfo.ID + UserInfo.Psw);
            ViewData["id"] = UserInfo.ID;
            ViewData["token"] = token;
            ViewData["HeadImg"] = UserInfo.HeadImg1;
            ViewData["lastMID"] = lastMID == null ? "" : lastMID.Value.ToString();
            ChatGroup group = cbll.GetGroupDetail(id);
            if (group.Gtype != (int)GType.GameGroup)
            {
                return Redirect("/Chat/ChatGroup/" + id);
            }
            ViewData["groupHeadImg"] = new UsersBLL().GetGroupUserHeadImg(id);
            if (cbll.ExistGroup(UserInfo.ID, id))
            {
                return View(group);
            }
            else
            {
                return View(group);//如果不存在跳转到首页，以后修改
            }
        }
        /// <summary>
        /// 转账 （立刻到帐）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Transfer(Guid rid, decimal Price, string Remark, string paypwd)
        {
            if (string.IsNullOrEmpty(UserInfo.PayPsw))
            {
                return RJson("-2", "请先设置支付密码");
            }
            if (UserInfo.PayPsw != MD5(paypwd))
            {
                return RJson("-3", "支付密码不正确");
            }
            ChatsBLL bll = new ChatsBLL();
            UsersBLL ubll = new UsersBLL();
            if (Price < 0.1M)
            {
                return RJson("-4", "红包金额不能小于0.1元");
            }
            if (ubll.GetBalance(UserInfo.ID) < Price)
            {
                return RJson("-5", "您的余额不足，不能发送");
            }
            int i = bll.Transfer(UserInfo.ID, rid, Price, Remark);
            if (i > 0)
            {
                Guid MID = Guid.NewGuid();
                string content = Remark + "|" + Price;
                SendMessage(UserInfo.ID, rid, MessageTType.User, MessageType.Transfer, content, MID);

                return Json(new
                {
                    code = "1",
                    msg = MID.ToString(),
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                });
            }
            else
            {
                return RJson("-1", "转账失败");
            }
        }

        /// <summary>
        /// 转账 （点击才到账）
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="Price"></param>
        /// <param name="Remark"></param>
        /// <param name="paypwd"></param>
        /// <returns></returns>
        public JsonResult DoTransfer(Guid rid, decimal Price, string Remark, string paypwd)
        {
            if (Remark.IndexOf("|") != -1)
            {
                return RJson("-1", "不能包含“|”");
            }
            if (!CheckPayPSW(paypwd))
            {
                return RJson("-100", "密码不正确");
            }
            ChatsBLL bll = new ChatsBLL();
            Common.ErrorCode err;
            ChatMessage msg = new ChatMessage();
            msg.MID = Guid.NewGuid();
            msg.FUID = UserInfo.ID;
            msg.TUID = rid;
            msg.TType = (int)MessageTType.User;
            msg.MType = (int)MessageType.Transfer;
            string content = Remark + "|" + Price;
            msg.Content = encodeHtml(content);
            msg.HadRead = 0;
            msg.IsDel = 0;
            msg.Status = 1;
            msg.CreateTime = msg.UpdateTime = DateTime.Now;
            int i = bll.Transfer_V2(out err, UserInfo.ID, rid, Price, msg, Remark);
            if (i > 0)
            {
                HasNewMsg(rid);

                return Json(new
                {
                    code = "1",
                    msg = msg.MID.ToString(),
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                });
            }
            else
            {
                return RJson(i.ToString(), err.ToString());
            }
        }
        public JsonResult TransferClick(Guid mid)
        {
            ChatsBLL bll = new ChatsBLL();
            Common.ErrorCode err;
            Guid tuid;
            int i = bll.TransferClick(out err, mid, UserInfo.ID, out tuid);
            if (i > 0)
            {
                HasNewMsg(tuid);
                return RJson("1", "领取成功");
            }
            else
            {
                return Json(i.ToString(), err.ToString());
            }
        }

        /// <summary>
        /// 处理相关消息方法
        /// </summary>
        /// <param name="TType"></param>
        /// <param name="Rid"></param>
        /// <param name="Gid"></param>
        /// <param name="Type"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public JsonResult QueryMessages(int TType, Guid? Rid, Guid? Gid, int Type, string Content)
        {
            try
            {
                //LogsBLL.InsertAPILog("QueryMessages", UserInfo.ID, string.Format("TType:{0},Rid:{1},Gid:{2},Type:{3},Content:{4},",
                //                                                                    TType, Rid, Gid, Type, Content));

                if (string.IsNullOrEmpty(Content))
                {
                    return RJson("-1", "消息不能为空");
                }
                ChatsBLL bll = new ChatsBLL();

                Guid MID = Guid.NewGuid();
                DateTime dt = DateTime.Now;
                if (TType == (int)MessageTType.Group)
                {
                    if (Gid == null)
                    {
                        return RJson("-1", "群组id不正确");
                    }
                    if (!bll.ExistGroup(UserInfo.ID, Gid.Value))
                    {
                        return RJson("-1", "您没有在该群，无法发送");
                    }
                    int i = SendGroupMessage(MID, UserInfo.ID, Gid.Value, (MessageType)Type, Content);
                    if (i > 0)
                    {
                        return Json(new
                        {
                            code = "1",
                            msg = MID.ToString(),
                            time = dt.ToString("yyyy-MM-dd HH:mm:ss.fff")
                        });
                    }
                    else
                    {
                        return RJson("-1", "发送失败！");
                    }
                }
                else
                {
                    if (Rid == null)
                    {
                        return RJson("-1", "用户id不正确");
                    }
                    var mt = (MessageType)Type;
                    FriendBLL fbll = new FriendBLL();
                    switch (mt)
                    {
                        case MessageType.FriendRequest://加好友
                            {
                                if (UserInfo.ID == Rid.Value)
                                {
                                    return RJson("-5", "不能加自己哦");
                                }
                                if (!fbll.CanAddFriend(UserInfo.ID))
                                {
                                    return RJson("-3", "已经达到好友上限");
                                }
                                if (fbll.isFriend(UserInfo.ID, Rid.Value))
                                {
                                    return RJson("-2", "你们已经是好友了");
                                }
                                if (fbll.haveFriendRequest(UserInfo.ID, Rid.Value))
                                {
                                    return RJson("-4", "已经发送过好友请求");
                                }
                                else
                                {
                                    int i = SendMessage(UserInfo.ID, Rid.Value, MessageTType.User, (MessageType)Type, Content, MID);
                                    if (i > 0)
                                    {
                                        new RelexBarBLL.Services.HuanXinIM().SendMsg_Huanxin(Rid.Value, Content, "{ \"type\":\"" + Type + "\"}");

                                        return RJson("1", "发送成功");
                                    }
                                    else
                                    {
                                        return RJson("-1", "发送失败！");
                                    }
                                }
                            }
                        case MessageType.Red://普通红包消息
                        case MessageType.SayHello://打招呼红包消息
                            {
                                decimal price;
                                var c = CommonClass.ChangeData.ExchangeDataType(Content);
                                if (UserInfo.ID == Rid.Value)
                                {
                                    return RJson("-1", "不能打招呼自己哦！");
                                }
                                if (!decimal.TryParse(c["price"].ToString(), out price))
                                {
                                    return RJson("-1", "金额不正确！");
                                }
                                if (mt == MessageType.SayHello)//如果是打招呼信息
                                {
                                    int SayCount = bll.GetSayHelloCount(UserInfo.ID, Rid.Value, MessageTType.User, MessageType.SayHello, DateTime.Now);
                                    if (SayCount > 0)
                                    {
                                        return RJson("-1", "今日已打招呼！");
                                    }
                                }
                                RedPacksBLL rpb = new RedPacksBLL();
                                int result = rpb.SendRed(UserInfo.ID, Rid.Value, price);
                                if (result < 0)
                                {
                                    return RJson("-1", "打招呼失败！" + ((Common.ErrorCode)result).ToString());
                                }
                                else
                                {
                                    int i;
                                    if (fbll.isFriend(UserInfo.ID, Rid.Value))//如果已经是好友，则直接发送到对方
                                    {
                                        i = SendMessage(UserInfo.ID, Rid.Value, MessageTType.User, MessageType.Red, Content, MID);
                                    }
                                    else
                                    {
                                        //发送打招呼红包
                                        i = SendMessage(UserInfo.ID, Rid.Value, MessageTType.User, MessageType.SayHello, Content, MID);
                                    }
                                    if (i > 0)
                                    {
                                        return Json(new
                                        {
                                            code = "1",
                                            msg = MID.ToString(),
                                            time = dt.ToString("yyyy-MM-dd HH:mm:ss.fff")
                                        });
                                    }
                                    else
                                    {
                                        return RJson("-1", "发送失败！");
                                    }
                                }
                            }
                        default:
                            {
                                if (!(fbll.isFriend(UserInfo.ID, Rid.Value)))
                                {
                                    return RJson("-2", "不是好友");
                                }
                                int i = SendMessage(UserInfo.ID, Rid.Value, MessageTType.User, (MessageType)Type, Content, MID);
                                if (i > 0)
                                {
                                    return Json(new
                                    {
                                        code = "1",
                                        msg = MID.ToString(),
                                        time = dt.ToString("yyyy-MM-dd HH:mm:ss.fff")
                                    });
                                }
                                else
                                {
                                    return RJson("-1", "发送失败！");
                                }
                            }
                    }
                }
            }
            catch (Exception e)
            {
                LogsBLL.InsertAPILog("QueryMessagesError", UserInfo.ID, e.ToString());
                return RJson("-1", "发送失败：" + e.Message);
            }
        }

        /// <summary>
        /// 详细详情
        /// </summary>
        /// <param name="mid"></param>
        /// <returns></returns>
        public JsonResult MessageDetail(Guid mid)
        {
            ChatsBLL bll = new ChatsBLL();
            var i = bll.Detail(UserInfo.ID, mid);
            return Json(new { code = i == null ? "0" : "1", model = i });
        }

        /// <summary>
        /// 发送消息方法
        /// </summary>
        /// <param name="TType"></param>
        /// <param name="Rid"></param>
        /// <param name="Gid"></param>
        /// <param name="Type"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public JsonResult SendMessages(int TType, Guid? Rid, Guid? Gid, int Type, string Content)
        {
            try
            {
                if (Content == null)
                {
                    return RJson("-1", "消息不能为null");
                }
                ChatsBLL bll = new ChatsBLL();

                Guid MID = Guid.NewGuid();
                DateTime dt = DateTime.Now;
                if (TType == (int)MessageTType.Group)
                {
                    if (Gid == null)
                    {
                        return RJson("-1", "群组id不正确");
                    }
                    if (!bll.ExistGroup(UserInfo.ID, Gid.Value))
                    {
                        return RJson("-1", "您没有在该群，无法发送");
                    }
                    int i = SendGroupMessage(MID, UserInfo.ID, Gid.Value, (MessageType)Type, Content);
                    if (i > 0)
                    {
                        return Json(new
                        {
                            code = "1",
                            msg = MID.ToString(),
                            time = dt.ToString("yyyy-MM-dd HH:mm:ss.fff")
                        });
                    }
                    else
                    {
                        return RJson("-1", "发送失败！");
                    }
                }
                else
                {
                    if (Rid == null)
                    {
                        return RJson("-1", "用户id不正确");
                    }
                    if (!(new FriendBLL().isFriend(UserInfo.ID, Rid.Value)))
                    {
                        return RJson("-2", "不是好友");
                    }
                    int i = SendMessage(UserInfo.ID, Rid.Value, MessageTType.User, (MessageType)Type, Content, MID);
                    if (i > 0)
                    {
                        return Json(new
                        {
                            code = "1",
                            msg = MID.ToString(),
                            time = dt.ToString("yyyy-MM-dd HH:mm:ss.fff")
                        });
                    }
                    else
                    {
                        return RJson("-1", "发送失败！");
                    }
                }
            }
            catch (Exception e)
            {
                return RJson("-1", "发送失败：" + e.Message);
            }


        }
        /// <summary>
        /// 获取消息记录
        /// </summary>
        /// <param name="TType"></param>
        /// <param name="lastMID"></param>
        /// <param name="gid"></param>
        /// <returns></returns>
        public JsonResult MessageRecord(int TType, Guid? lastMID, Guid? gid, Guid? fid)
        {
            ChatsBLL bll = new ChatsBLL();
            if (TType == (int)MessageTType.Group)
            {
                if (gid == null)
                {
                    return RJson("-1", "群组不正确");
                }
                List<ChatMessage> list = bll.GetGroupChatMessage(gid.Value, lastMID);
                return RJson("1", list.SerializeObject("yyyy-MM-dd HH:mm"));
            }
            else if (TType == (int)MessageTType.User)
            {
                if (fid == null)
                {
                    return RJson("-1", "消息获取失败");
                }
                List<ChatMessage> list = bll.GetChatMessage(UserInfo.ID, fid.Value, lastMID);
                return RJson("1", list.SerializeObject("yyyy-MM-dd HH:mm"));
            }
            return null;
        }
        /// <summary>
        /// 获取消息-往上滑动
        /// </summary>
        /// <param name="TType"></param>
        /// <param name="lastMID"></param>
        /// <param name="gid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public JsonResult MessageRecordButtom(int TType, Guid? lastMID, Guid? gid, Guid? fid)
        {
            if (lastMID == null)
            {
                return RJson("1", "[]");
            }
            ChatsBLL bll = new ChatsBLL();
            if (TType == (int)MessageTType.Group)
            {
                if (gid == null)
                {
                    return RJson("-1", "群组不正确");
                }
                List<ChatMessage> list = bll.GetGroupChatMessageButtom(gid.Value, lastMID);
                return RJson("1", list.SerializeObject("yyyy-MM-dd HH:mm"));
            }
            else if (TType == (int)MessageTType.User)
            {
                if (fid == null)
                {
                    return RJson("-1", "消息获取失败");
                }
                List<ChatMessage> list = bll.GetChatMessageButtom(UserInfo.ID, fid.Value, lastMID);
                return RJson("1", list.SerializeObject("yyyy-MM-dd HH:mm"));
            }
            return null;
        }
        /// <summary>
        /// 创建群
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateGroup(int? id)
        {
            return View(id);
        }
        [HttpPost]
        public JsonResult DoCreateGroup(string GName, string Gtype)
        {
            if (string.IsNullOrEmpty(GName))
            {
                return RJson("-1", "名称不能为空");
            }
            GType gtype = GType.ChatGroup;
            if (Gtype != "1")
            {
                gtype = GType.GameGroup;
            }
            ChatsBLL bll = new ChatsBLL();
            Guid? gid = Guid.NewGuid();
            int i = bll.CreateGroup(UserInfo.ID, GName, gtype, null, gid);
            if (i > 0)
            {
                return Json(new { code = "1", msg = "创建成功", gid = gid });
                //return RJson("1", "创建成功");
            }
            else
            {
                return RJson("-1", "创建失败");
            }
        }

        /// <summary>
        /// 群信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GroupInfo(Guid id)
        {
            ChatsBLL bll = new ChatsBLL();
            ChatGroup group = bll.GetGroupDetail(id);
            if (group.Gtype != 1)
            {//不是群聊
                return SLGroupInfo(id);
            }
            List<GroupUser> list = bll.GetGroupUsers(id);
            ViewData["Users"] = list;
            ViewData["isAdmin"] = group.MUID == UserInfo.ID;
            ViewData["nick"] = bll.GetGroupNick(id, UserInfo.ID);
            ViewData["isTop"] = bll.GetGroupisTop(id, UserInfo.ID);
            return View(group);
        }
        /// <summary>
        /// 扫雷房间信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SLGroupInfo(Guid id)
        {
            ChatsBLL bll = new ChatsBLL();
            ChatGroup group = bll.GetGroupDetail(id);
            if (group.Gtype != 2)
            {//不是游戏群 
                return GroupInfo(id);
            }
            List<GroupUser> list = bll.GetGroupUsers(id);
            ViewData["Users"] = list;
            ViewData["nick"] = bll.GetGroupNick(id, UserInfo.ID);
            ViewData["isTop"] = bll.GetGroupisTop(id, UserInfo.ID);
            ViewData["isAdmin"] = group.MUID == UserInfo.ID;
            return View(group);
        }
        /// <summary>
        /// 拉人到群组 好友列表从本地读取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddFriendToGroup(Guid? id)
        {
            if (id == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            List<Guid> groupUID = new ChatsBLL().GetGroupUsersUID(id.Value);
            ViewData["UIDS"] = string.Join(",", groupUID);
            return View(id.Value);
        }
        [HttpPost]
        public JsonResult AddFriendToGroup(Guid gid, Guid uid)
        {
            ChatsBLL bll = new ChatsBLL();
            if (bll.ExistGroup(uid, gid))
            {
                return RJson("-2", "用户已存在");
            }
            if (!bll.ExistGroup(UserInfo.ID, gid))
            {
                return RJson("-3", "您已不在此群聊");
            }
            int i = bll.AddFriendToGroup(uid, gid);
            if (i > 0)
            {
                SendGroupMessage(Guid.NewGuid(), Guid.Empty, gid, MessageType.JoinGroupNotice, uid.ToString());
                //SendMessage(Guid.Empty, gid, MessageTType.System, MessageType.JoinGroupNotice, uid.ToString());
                return RJson("1", "添加成功");
            }
            return RJson("-1", "添加失败");
        }

        /// <summary>
        /// 修改群公告
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupAnnounce(Guid id)
        {
            ChatsBLL bll = new ChatsBLL();
            ChatGroup group = bll.GetGroupDetail(id);
            return View(group);
        }
        [HttpPost]
        public JsonResult DoGroupAnnounce(Guid id, string Notice)
        {
            ChatsBLL bll = new ChatsBLL();
            int i = bll.UpdateGroupNotice(id, Notice);
            if (i > 0)
            {
                return RJson("1", "群公告修改成功");
            }
            else
            {
                return RJson("-1", "群公告修改失败");
            }
        }
        /// <summary>
        /// 修改群名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GroupName(Guid id)
        {
            ChatsBLL bll = new ChatsBLL();
            ChatGroup group = bll.GetGroupDetail(id);
            return View(group);
        }
        [HttpPost]
        public JsonResult DoGroupName(Guid id, string GName)
        {
            ChatsBLL bll = new ChatsBLL();
            int i = bll.UpdateGroupName(id, GName);
            if (i > 0)
            {
                return RJson("1", "群名称修改成功");
            }
            else
            {
                return RJson("-1", "群名称修改失败");
            }
        }
        [HttpGet]
        public ActionResult GroupList()
        {
            return View();
        }
        /// <summary>
        /// 删除群组好友
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RemoveGroupUser(Guid id)
        {
            List<GroupUser> list = new ChatsBLL().GetGroupUsers(id);
            ViewData["uid"] = UserInfo.ID;
            ViewData["gid"] = id;
            return View(list);

        }
        /// <summary>
        /// 删除群组中的用户
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoRemoveGroupUser(Guid GID, Guid UID)
        {
            if (UserInfo.ID == UID)
            {
                return RJson("-1", "删除失败");
            }
            ChatsBLL bll = new ChatsBLL();

            if (bll.isGroupAdmin(GID, UserInfo.ID))
            {
                int i = bll.RemoveGroupUser(GID, UID);
                if (i > 0)
                {
                    SendGroupMessage(Guid.NewGuid(), Guid.Empty, GID, MessageType.RemoveGroupUser, UID.ToString());
                    SendMessage(Guid.Empty, UID, MessageTType.System, MessageType.RemoveGroupUser, GID.ToString());
                    return RJson("1", "删除成功");
                }
                else
                {
                    return RJson("-1", "删除失败");
                }
            }
            else
            {
                return RJson("-1", "权限不足");
            }
        }
        /// <summary>
        /// 退出群
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExitGroup(Guid GID)
        {
            ChatsBLL bll = new ChatsBLL();
            if (bll.isGroupAdmin(GID, UserInfo.ID))
            {
                return RJson("-2", "管理员不能退出");
            }
            int i = bll.RemoveGroupUser(GID, UserInfo.ID);
            if (i > 0)
            {
                SendGroupMessage(Guid.NewGuid(), Guid.Empty, GID, MessageType.ExitGroupNotice, UserInfo.ID.ToString());
                return RJson("1", "退出成功");
            }
            else
            {
                return RJson("-1", "退出失败");
            }

        }
        /// <summary>
        /// 解散群/改变群状态
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public JsonResult DropGroup(Guid GID)
        {
            ChatsBLL bll = new ChatsBLL();

            if (bll.isGroupAdmin(GID, UserInfo.ID))
            {
                int i = bll.UpdateGroupStatus(GID, Common.enStatus.Unabled);
                if (i > 0)
                {
                    SendGroupMessage(Guid.NewGuid(), Guid.Empty, GID, MessageType.DropGroup, GID.ToString());
                    return RJson("1", "删除成功");
                }
                else
                {
                    return RJson("-1", "删除失败");
                }
            }
            else
            {
                return RJson("-1", "权限不足");
            }
        }
        /// <summary>
        /// 修改我在群里的昵称
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public JsonResult UpdateGroupNick(Guid GID, string Nick)
        {
            ChatsBLL bll = new ChatsBLL();
            int i = bll.UpdateGroupNick(GID, UserInfo.ID, Nick);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-1", "修改失败");
            }
        }
        /// <summary>
        /// 消息置顶
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="isTop"></param>
        /// <returns></returns>
        public JsonResult UpdateIsTop(Guid? GID, Guid? FUID, int isTop)
        {
            ChatsBLL bll = new ChatsBLL();
            int i = 0;
            if (GID != null)
            {
                i = bll.UpdateGroupIsTop(GID.Value, UserInfo.ID, isTop);
            }
            else
            {
                i = bll.UpdateFriendIsTop(FUID.Value, UserInfo.ID, isTop);
            }

            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-1", "修改失败");
            }
        }
        /// <summary>
        /// 查询聊天记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchChatMessageRecord(Guid id, string param, string back)
        {
            ViewData["param"] = param;

            if (param == "UID")
            {
                ViewData["showname"] = new FriendBLL().GetRemark(UserInfo.ID, id);
                ViewData["showimg"] = new UsersBLL().GetHeadImg(id);
            }
            else
            {
                ViewData["showname"] = new ChatsBLL().GetGroupName(id);
                ViewData["back"] = back;
            }

            return View(id);
        }
        [HttpPost]
        public JsonResult SearchChatMessageRecord(Guid? GID, Guid? UID, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return RJson("1", "[]");
            }
            ChatsBLL bll = new ChatsBLL();
            List<RelexBarDLL.ChatMessage> result;

            if (GID != null)
            {
                result = bll.SearchMessage(UserInfo.ID, GID.Value, content, MessageType.Text);
            }
            else if (UID != null)
            {
                result = bll.SearchMessage(UserInfo.ID, UID.Value, content, MessageType.Text);
            }
            else
            {
                return RJson("-1", "参数不正确");
            }
            return RJson("1", result.SerializeObject("yyyy-MM-dd HH:mm"));
        }
        /// <summary>
        /// 选择好友并创建群聊
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddFriendToNewGroup(Guid ID)
        {
            return View(ID);
        }
        public JsonResult CreateNewGroup(string UIDS)
        {
            string[] id = UIDS.Split(',');
            Guid[] uids = new Guid[id.Length];
            try
            {
                for (int i = 0; i < id.Length; i++)
                {
                    uids[i] = Guid.Parse(id[i]);
                }
                ChatsBLL bll = new ChatsBLL();
                Guid gid;
                int j = bll.CreateGroup(UserInfo.ID, "未命名", GType.ChatGroup, out gid, uids);
                if (j > 0)
                {
                    try
                    {
                        for (int i = 0; i < uids.Length; i++)
                        {
                            SendGroupMessage(Guid.NewGuid(), Guid.Empty, gid, MessageType.JoinGroupNotice, uids[i].ToString());



                            //SendMessageGroup(Guid.Empty, gid, MessageTType.System, MessageType.JoinGroupNotice, uids[i].ToString());
                            //SendMessage(Guid.Empty, uids[i], MessageTType.System, MessageType.JoinGroupNotice, gid.ToString());
                        }

                    }
                    catch (Exception)
                    {

                    }
                    return RJson("1", gid.ToString());
                }
                else
                {
                    return RJson("-1", "创建失败");
                }
            }
            catch (Exception ex)
            {
                return RJson("-1", "创建失败" + ex.Message);
            }

        }
        /// <summary>
        /// 发送名片
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="FID"></param>
        /// <param name="CID">名片id</param>
        /// <returns></returns>
        public JsonResult SendUserCard(Guid? GID, Guid? FID, Guid CID)
        {
            if (GID == null && FID == null)
            {
                return RJson("-3", "参数不正确");
            }
            Users user = new UsersBLL().GetUserById(CID);
            if (user == null)
            {
                return RJson("-2", "用户不存在");
            }
            int i;
            Guid MID = Guid.NewGuid();
            string content = user.ID + "|" + user.Name + "|" + user.Phone + "|" + user.HeadImg1 + "|" + MID;
            if (GID != null)
            {
                i = SendGroupMessage(MID, UserInfo.ID, GID.Value, MessageType.MyCard, content);
            }
            else
            {
                i = SendMessage(UserInfo.ID, FID.Value, MessageTType.User, MessageType.MyCard, content, MID);
            }
            if (i > 0)
            {
                return Json(new
                {
                    code = "1",
                    msg = content,
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                });
            }
            else
            {
                return RJson("-1", "发送失败");
            }
        }
        /// <summary>
        /// 撤销消息
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="TType">1用户，2群</param>
        /// <returns></returns>
        public JsonResult RevokeMSG(int TType, Guid? Rid, Guid? Gid, Guid MID)
        {
            try
            {
                ChatsBLL bll = new ChatsBLL();
                ChatMessageBLL mbll = new ChatMessageBLL();
                if (TType == (int)MessageTType.Group)
                {
                    if (Gid == null)
                    {
                        return RJson("-1", "群组id不正确");
                    }
                    if (!bll.ExistGroup(UserInfo.ID, Gid.Value))
                    {
                        return RJson("-1", "您没有在该群，无法发送");
                    }
                    int j = mbll.DeleteMsg(UserInfo.ID, MID);
                    if (j > 0)
                    {
                        SendGroupMessageNoDB(Guid.NewGuid(), UserInfo.ID, Gid.Value, MessageType.Revoke, MID.ToString());
                    }
                    else
                    {
                        return RJson("-1", "撤回失败");
                    }

                }
                else
                {
                    if (Rid == null)
                    {
                        return RJson("-1", "群组id不正确");
                    }
                    int j = mbll.DeleteMsg(UserInfo.ID, MID);
                    if (j > 0)
                    {
                        SendMessageNoDB(UserInfo.ID, Rid.Value, MessageTType.User, MessageType.Revoke, MID.ToString());
                    }
                    else
                    {
                        return RJson("-1", "撤回失败");
                    }


                }
                return RJson("1", "撤回成功");
            }
            catch (Exception)
            {
                return RJson("-1", "撤回失败");
            }
        }
        //转发消息
        public JsonResult ForwardMessage(Guid MID, string UIDS, string TType)
        {
            try
            {
                string[] uids = UIDS.Split(',');
                string[] ttype = TType.Split(',');
                if (uids.Length != ttype.Length)
                {
                    return RJson("-2", "参数不正确");
                }
                Guid[] UIDS2 = Array.ConvertAll<string, Guid>(uids, u => Guid.Parse(u));
                int[] TType2 = Array.ConvertAll<string, int>(ttype, t => int.Parse(t));
                int i = ForwardMessage(UserInfo.ID, MID, UIDS2, TType2);
                if (i > 0)
                {
                    return RJson("1", "转发成功");
                }
                else
                {
                    return RJson("-1", "转发失败");
                }

            }
            catch (Exception ex)
            {
                return RJson("-1", ex.Message);
            }

        }
        /// <summary>
        /// 获取接收者时间大于date的消息
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public JsonResult GetMyRecMessage(string date)
        {
            LogsBLL.InsertAPILog("/Chat/GetMyRecMessage", UserInfo.ID, date == null ? "" : date);
            //if (APPController.serviceList.ContainsKey(UserInfo.ID))
            //{
            //    IMService service = APPController.serviceList[UserInfo.ID];
            //    if (service != null)
            //    {
            //        service.HasMessage = false;
            //    }
            //}
            var imService = APPIMController.Get(UserInfo.ID);
            if (imService != null)
            {
                imService.HasMessage = false;
            }
            DateTime? dateTime = null;
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    dateTime = Common.GetTime(date);
                }
                catch (Exception)
                {

                }
            }

            List<ChatMessage> list = new List<ChatMessage>();
            if (dateTime.HasValue)
            {
                list = new ChatMessageBLL().GetMyRecMessage(UserInfo.ID, dateTime);
            }

            string timeStamp = "";
            if (list.Count > 0)
            {
                timeStamp = Common.GetTimeStamp(list[0].CreateTime.Value);
            }
            else
            {
                timeStamp = Common.GetTimeStamp(DateTime.Now);
            }

            dynamic rdata = new
            {
                code = "1",
                timeStamp = timeStamp,
                list = list.OrderBy(m => m.CreateTime)
            };
            return new JsonResultPro(rdata, JsonRequestBehavior.AllowGet, "yyyy-MM-dd HH:mm:ss.fff");
        }
        #region app修改接口
        /// <summary>
        /// 撤销消息
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="TType">1用户，2群</param>
        /// <returns></returns>
        public JsonResult RevokeMessage(int TType, Guid? Rid, Guid? Gid, Guid MID)
        {
            try
            {
                ChatsBLL bll = new ChatsBLL();
                ChatMessageBLL mbll = new ChatMessageBLL();
                if (TType == (int)MessageTType.Group)
                {
                    int j = mbll.DeleteMessage(UserInfo.ID, MID);
                    if (j > 0)
                    {
                        SendGroupMessageNoDB(Guid.NewGuid(), UserInfo.ID, Gid.Value, MessageType.Revoke, MID.ToString(), MessageTType.System);
                    }
                    else
                    {
                        return RJson("-1", "撤回失败");
                    }

                }
                else
                {
                    int j = mbll.DeleteMessage(UserInfo.ID, MID);
                    if (j > 0)
                    {
                        SendMessageNoDB(UserInfo.ID, Rid.Value, MessageTType.System, MessageType.Revoke, MID.ToString());
                    }
                    else
                    {
                        return RJson("-1", "撤回失败");
                    }


                }
                return RJson("1", "撤回成功");
            }
            catch (Exception)
            {
                return RJson("-1", "撤回失败");
            }
        }
        /// <summary>
        /// 获取最后阅读id
        /// </summary>
        /// <param name="TType"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetLastReadMID(MessageTType TType, Guid ID)
        {
            ChatMessageBLL bll = new ChatMessageBLL();
            Guid? g = bll.GetLastReadMID(UserInfo.ID, TType, ID);
            if (g == null)
            {
                return RJson("-1", "");
            }
            return RJson("1", g.Value.ToString());
        }
        #endregion
    }
}