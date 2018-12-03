using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;
using RelexBarDLL;
using RelexBarBLL.EnumCommon;
using RelexBarBLL.Models;
using RelaxBarWeb_MVC.Utils;
using System.Threading.Tasks;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class FriendController : BaseController
    {
        // GET: Friend
        public ActionResult Info(string id)
        {

            return View();
        }
        /// <summary>
        /// 根据关键字查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public ActionResult FindFriend(string id)
        {
            FriendBLL bll = new FriendBLL();
            //List<Users> list= bll.FindFriend(id);
            //ViewData["id"] = id;
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            Users u = new UsersBLL().GetUserByPhone("", id);
            if (u != null)
            {
                return Redirect("/Friend/FriendInfo/" + u.ID);
            }
            return View();
        }
        /// <summary>
        /// 陌生用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public ActionResult FriendInfo(Guid id)
        {
            ViewData["name"] = UserInfo.Name;
            Guid ID = id;
            UsersBLL bll = new UsersBLL();
            Users user = bll.GetUserById(ID);
            if (user == null)
            {
                Redirect("FindFriend");
            }
            FriendBLL fbll = new FriendBLL();
            ViewData["isFriend"] = fbll.isFriend(UserInfo.ID, ID);
            ViewData["Remark"] = fbll.GetRemark(UserInfo.ID, ID);
            return View(user);

        }
        /// <summary>
        /// 添加好友页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public ActionResult AddFriend()
        {
            return View();
        }
        /// <summary>
        /// 好友请求
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult SendFriendRequest(Guid uid, string content)
        {
            FriendBLL bll = new FriendBLL();
            if (bll.isFriend(UserInfo.ID, uid))
            {
                return RJson("-1", "已经是好友了");
            }
            if (bll.haveFriendRequest(UserInfo.ID, uid))
            {
                return RJson("-1", "已经发送过好友请求");
            }
            Guid mid = Guid.NewGuid();
            var my = new APPFriendModel
            {
                ID = Guid.NewGuid(),
                UID = uid,
                FriendID = uid,
                Remark = UserInfo.Name,
                LastMID = mid,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Name = UserInfo.Name,
                HeadImg = UserInfo.HeadImg1,
                LastReadID = null,
                IsTop = 0,
                Ignore = 0,
                Phone = UserInfo.Phone,
                Sex = UserInfo.Sex,
                LastLoginTime = UserInfo.LastLoginTime,
            };
            int i = SendMessage(UserInfo.ID, uid, MessageTType.System, MessageType.FriendRequest, my.SerializeObject("yyyy-MM-dd HH:mm:ss"));

            //int i = SendMessage(UserInfo.ID, uid, MessageTType.System, MessageType.FriendRequest, content,mid);
            if (i > 0)
            {
                int j = bll.AutoAgreeFriend(mid);
                if (j > 0)
                {
                    APPFriendModel u = bll.GetFriendListByAPP(UserInfo.ID, uid).Take(1).FirstOrDefault();

                    SendMessage(Guid.Empty, UserInfo.ID, MessageTType.System, MessageType.FriendRequestNotice, u.SerializeObject("yyyy-MM-dd HH-mm-ss"));
                }
                return RJson("1", "发送成功");
            }
            else
            {
                return RJson("-1", "发送失败");
            }
        }
        /// <summary>
        /// 是否同意好友
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult Agree(int? id, Guid mid)
        {
            if (id == null)
            {
                return RJson("-1", "参数不正确");
            }
            ChatMessageBLL bll = new ChatMessageBLL();
            ChatMessage msg = bll.Get(mid);
            if (msg == null)
            {
                return RJson("-2", "记录不存在");
            }
            if (msg.HadRead > 0)
            {
                return RJson("-3", "已处理");
            }
            FriendBLL fbll = new FriendBLL();
            Guid FUID = msg.FUID;//Guid.Empty;//好友请求发送者
            //Guid.TryParse(msg.FUID,out FUID);
            if (fbll.isFriend(UserInfo.ID, FUID))
            {
                return RJson("-5", "已经是好友关系");
            }
            if (UserInfo.ID == FUID)
            {
                return RJson("-6", "参数错误");
            }
            if (id.Value == 1)
            {
                int i = fbll.AddFriend(UserInfo.ID, FUID, msg);

                if (i > 0)
                {
                    bll.Readed(msg.MID, 1);

                    SendMessage(Guid.Empty, FUID, MessageTType.System, MessageType.FriendRequestNotice, UserInfo.ID.ToString());

                    new RelexBarBLL.Services.HuanXinIM().AddFriend_Huanxin(UserInfo.ID, FUID);

                    return RJson("1", "添加成功");
                }
                else
                {
                    return RJson("-4", "添加失败");
                }
            }
            else
            {
                bll.Readed(msg.MID, 2);
                return RJson("-2", "已拒绝");
            }

        }

        /// <summary>
        /// 是否接受对方的打招呼
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult RecHello(int? id, Guid mid)
        {
            if (id == null)
            {
                return RJson("-1", "参数不正确");
            }
            ChatMessageBLL bll = new ChatMessageBLL();
            ChatMessage msg = bll.Get(mid);
            if (msg == null)
            {
                return RJson("-2", "记录不存在");
            }
            if (msg.HadRead > 0)
            {
                return RJson("-3", "已处理");
            }
            FriendBLL fbll = new FriendBLL();
            Guid FUID = msg.FUID;//Guid.Empty;//好友请求发送者
            if (UserInfo.ID == FUID)
            {
                return RJson("-6", "参数错误");
            }
            if (!fbll.CanAddFriend(UserInfo.ID))
            {
                return RJson("-4", "已经达到好友上限");
            }
            if (id.Value == 1)
            {
                int i = fbll.AddFriend(UserInfo.ID, FUID, msg);
                //接收红包，并添加成好友  TODO
                new RedPacksBLL().RecChatRed(UserInfo.ID, msg.MID);

                if (i > 0)
                {
                    bll.Readed(msg.MID, 1);

                    SendMessage(Guid.Empty, FUID, MessageTType.System, MessageType.FriendRequestNotice, UserInfo.ID.ToString());

                    return RJson("1", "添加成功");
                }
                else
                {
                    return RJson("-4", "添加失败");
                }
            }
            else
            {
                bll.Readed(msg.MID, 2);
                return RJson("-2", "已拒绝");
            }
        }

        /// <summary>
        /// 修改备注
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CRemark(Guid? id)
        {
            if (id == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return View(id.Value);
        }
        [HttpPost]
        public JsonResult CRemark(Guid fuid, string remark)
        {
            if (!remark.CheckString())
            {
                return RJson("-2", "不能包含特殊字符");
            }
            FriendBLL bll = new FriendBLL();
            int i = bll.ChangeRemark(UserInfo.ID, fuid, remark);
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
        /// 删除好友  关系数据并没有删除将isDel状态改为1
        /// </summary>
        /// <param name="fuid"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult DeleteFriend(Guid fuid)
        {
            ChatsBLL bll = new ChatsBLL();
            int i = bll.DeleteFriend(UserInfo.ID, fuid);
            if (i > 0)
            {
                SendMessage(Guid.Empty, fuid, MessageTType.System, MessageType.DeleteFriend, UserInfo.ID.ToString());
                return RJson("1", "删除成功");
            }
            else
            {
                return RJson("-1", "删除失败");
            }
        }
        /// <summary>
        /// 新的朋友
        /// </summary>
        /// <returns></returns>
        public ActionResult NewFriend()
        {
            return View();
        }
        public ActionResult NewFriendPartView(int? id)
        {
            if (id == null)
            {
                id = 1;
            }
            FriendBLL bll = new FriendBLL();
            List<NewFriendModel> list = bll.GetFriendRequestList(UserInfo.ID, id.Value, 10);
            return PartialView(list);
        }
        /// <summary>
        /// 未处理的好友申请数量
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult FriendRequestCount()
        {
            FriendBLL bll = new FriendBLL();
            int i = bll.GetFriendRequestCount(UserInfo.ID, 0);
            return RJson("1", i.ToString());
        }
        [Filter.CheckLogin]
        public JsonResult GetFriendList(Guid? fid)
        {
            FriendBLL bll = new FriendBLL();
            List<APPFriendModel> list = bll.GetFriendListByAPP(UserInfo.ID, fid);
            return Json2(list);
        }

        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetFriendApplyList()
        {
            FriendBLL bll = new FriendBLL();
            List<NewFriendModel> list = bll.GetFriendRequestList(UserInfo.ID, PageIndex, 10);
            return Json(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        /// <summary>
        /// 获取好友打招呼列表
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetSayHelloList()
        {
            FriendBLL bll = new FriendBLL();
            List<NewFriendModel> list = bll.GetFriendRequestList(UserInfo.ID, PageIndex, 10, MessageType.SayHello);
            return Json(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetFriendList()
        {
            FriendBLL bll = new FriendBLL();
            List<ShowReceiverModel> list = bll.GetFriendList(UserInfo.ID);
            return Json(new { code = 1, maxcount = bll.GetMyMaxFriendCount(UserInfo.ID, UserInfo.UserType), list = list });
        }

        #region
        /// <summary>
        /// 查找朋友
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult SearchFriend(string key)
        {
            FriendBLL bll = new FriendBLL();
            var ulist = new UsersBLL().SearchFriend(key);
            return Json(ulist);
        }
        /// <summary>
        /// 好友申请  app调用
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult SendFriendReq(Guid uid)
        {
            //var fu = new UsersBLL().GetUserById(uid);
            //if (fu==null) {
            //    return RJson("-4","发送失败");
            //}
            if (uid == UserInfo.ID)
            {
                return RJson("-1", "不能发送给自己");
            }
            FriendBLL bll = new FriendBLL();
            if (bll.isFriend(UserInfo.ID, uid))
            {
                return RJson("-2", "已经是好友了");
            }
            //if (bll.haveFriendRequest(UserInfo.ID, uid))
            //{
            //    return RJson("-3", "已经发送过好友请求");
            //}
            Guid mid = Guid.NewGuid();

            var my = new APPFriendModel
            {
                ID = Guid.NewGuid(),
                UID = uid,
                FriendID = UserInfo.ID,
                Remark = UserInfo.Name,
                LastMID = mid,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Name = UserInfo.Name,
                HeadImg = UserInfo.HeadImg1,
                LastReadID = null,
                IsTop = 0,
                Ignore = 0,
                Phone = UserInfo.Phone,
                Sex = UserInfo.Sex,
                CardNumber = UserInfo.CardNumber,
                LastLoginTime = UserInfo.LastLoginTime,
            };
            int i = SendMessage(UserInfo.ID, uid, MessageTType.System, MessageType.FriendRequest, my.SerializeObject("yyyy-MM-dd HH:mm:ss"));

            if (i > 0)
            {
                int j = bll.AutoAgreeFriend(mid);
                if (j > 0)
                {
                    APPFriendModel u = bll.GetFriendListByAPP(UserInfo.ID, uid).Take(1).FirstOrDefault();


                    SendMessage(Guid.Empty, UserInfo.ID, MessageTType.System, MessageType.FriendRequestNotice, u.SerializeObject("yyyy-MM-dd HH:mm:ss"));
                }
                return RJson("1", "发送成功");
            }
            else
            {
                return RJson("-4", "发送失败");
            }
        }
        #endregion


    }
}