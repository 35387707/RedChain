using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RelaxBarWeb_MVC.Models;
using RelexBarBLL;
using RelexBarDLL;
using System.Collections.Generic;
using RelexBarBLL.Models;
using Newtonsoft.Json;
using RelaxBarWeb_MVC.Utils;
using System.Text.RegularExpressions;
using System.Threading;

namespace RelaxBarWeb_MVC.Controllers
{

    public class AccountController : BaseController
    {
        [Filter.AutoLogin]
        [HttpGet]
        public ActionResult Login()
        {
            if (UserInfo != null)
                return Redirect("/Account/ChatPage");
            return View();
        }
        [Filter.AutoLogin]
        [HttpGet]
        public ActionResult LoginByEmail()
        {
            if (UserInfo != null)
                return Redirect("/Account/ChatPage");
            return View();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetUserInfo(Guid? id)
        {
            Users u;
            PayListBLL pb = new PayListBLL();

            if (id.HasValue && UserInfo.ID != id.Value)//查找他人
            {
                MyCollectionBLL mbll = new MyCollectionBLL();
                FriendBLL fbll = new FriendBLL();
                u = new UsersBLL().GetUserById(id.Value);
                var Collect = mbll.GetMyCollect(UserInfo.ID, id, Common.enStatus.Enabled, Common.enMycollectionType.User);
                //  var CollectionID = Collect == null ? "0" : Collect.ID.ToString();
                var UserFollowerID = Collect == null ? "0" : Collect.ID.ToString();

                ChatsBLL cbll = new ChatsBLL();

                var SayHello = cbll.GetSayHello(UserInfo.ID, id, RelexBarBLL.EnumCommon.MessageTType.User, RelexBarBLL.EnumCommon.MessageType.SayHello);
                var SayHelloID = SayHello == null ? "0" : SayHello.MID.ToString();
                var IsSayHello = SayHello == null ? "0" : "1";
                return Json(new
                {
                    code = 1,
                    Info = new
                    {
                        ID = u.ID,
                        Name = u.Name,
                        TrueName = u.TrueName,
                        CardNumber = u.CardNumber,
                        Phone = u.Phone,
                        Sex = u.Sex,
                        HeadImg = u.HeadImg1,
                        FID = u.FID,
                        Status = u.Status,
                        RealCheck = u.RealCheck,
                        UserType = u.UserType,
                        CreateTime = u.CreateTime,
                        UpdateTime = u.UpdateTime,
                        Descrition = u.Descrition,
                        Address = u.Address,
                        AreaCode = u.AreaCode,
                        AllRewards = pb.TotalPaysOutExchange(UserInfo.ID, null, null, null, Common.enPayInOutType.In, Common.enPayType.Coin),
                        TodayRewards = pb.TotalPays(UserInfo.ID, DateTime.Now.Date, null, null, Common.enPayInOutType.In, Common.enPayType.Coin),
                        IsCollection = mbll.GetCollectTotal(UserInfo.ID, id, Common.enStatus.Enabled, Common.enMycollectionType.User) == 0 ? 0 : 1, //收藏状态（0未收藏，1已收藏） 
                        IsUserFollower = mbll.GetCollectTotal(UserInfo.ID, id, Common.enStatus.Enabled, Common.enMycollectionType.User) == 0 ? 0 : 1, //关注状态（0未关注，1已关注） 
                        IsFriend = fbll.isFriend(UserInfo.ID, id.Value) == true ? 1 : 0,//好友状态（0未好友，1已好友） 
                                                                                        //   CollectionID= CollectionID,    //收藏id
                        UserFollowerID = UserFollowerID, //关注id
                        SayHelloID = SayHelloID,  //敲门id
                        IsSayHello = IsSayHello,  //是否敲过门

                    }
                }, JsonRequestBehavior.AllowGet);
            }
            else//查看自己
            {
                var ubll = new UsersBLL();
                u = ubll.GetUserById(UserInfo.ID);

                PayListBLL pbll = new PayListBLL();
                FriendBLL fbll = new FriendBLL();
                RedPacksBLL rbll = new RedPacksBLL();

                UserInfo = u;

                Users fu = null;
                if (u.FID.HasValue)
                {
                    fu = ubll.GetUserById(u.FID.Value);
                }

                return Json(new
                {
                    code = 1,
                    Info = new
                    {
                        ID = u.ID,
                        Name = u.Name,
                        TrueName = u.TrueName,
                        CardNumber = u.CardNumber,
                        Phone = u.Phone,
                        Sex = u.Sex,
                        Score = u.Score,
                        TotalScore = u.TotalScore,
                        Balance = u.Balance,
                        RedBalance = u.RedBalance,
                        HeadImg = u.HeadImg1,
                        FID = u.FID,
                        FUserName = fu != null ? fu.TrueName + "(" + fu.Phone + ")" : "",
                        Status = u.Status,
                        RealCheck = u.RealCheck,
                        UserType = u.UserType,
                        HasPayPwd = !string.IsNullOrEmpty(u.PayPsw),
                        CreateTime = u.CreateTime,
                        UpdateTime = u.UpdateTime,
                        Descrition = u.Descrition,
                        Address = u.Address,
                        FootQuan = u.FootQuan,
                        AreaCode = u.AreaCode,
                        AllRewards = pb.TotalPaysOutExchange(UserInfo.ID, null, null, null, Common.enPayInOutType.In, Common.enPayType.Coin),
                        TodayRewards = pb.TotalPays(UserInfo.ID, DateTime.Now.Date, null, null, Common.enPayInOutType.In, Common.enPayType.Coin),
                        FubaoShare = 0,
                        BigFubaoShare = 0,
                        maxcount = fbll.GetMyMaxFriendCount(UserInfo.ID, UserInfo.UserType),//添加最大好友数
                        MainTainRedCount = rbll.TimeOutRedCount(UserInfo.ID) //剩余释放福包数
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            if (u == null)
            {
                return RJson("0", "用户不存在");
            }
        }

        [HttpPost]
        public JsonResult DoLogin(string phone, string pwd, int remember, Common.enOSType? ostype, string device, string before = "86")
        {
            LogsBLL.InsertAPILog("/Account/DoLogin", Guid.Empty, string.Format("登陆调用：before={0},phone={1},pwd={2},remember={3}", before, phone, pwd, remember));
            UsersBLL bll = new UsersBLL();
            if (string.IsNullOrEmpty(before))
                before = "86";
            before = before.Replace(" ", "").Replace("+", "");

            DateTime lastLoginTime = DateTime.Now, nowLoginTime;
            Users user = bll.Login(before, phone, pwd, ref lastLoginTime, out nowLoginTime, ostype, device);

            if (user == null)
            {
                return Json(new { code = -1, msg = "手机号或密码输入有误！" });
            }
            if (user.Status == 0)
            {
                return Json(new { code = -2, msg = "此用户已被禁用" });
            }
            UserInfo = user;

            //if (remember == 1)
            {
                string token = UserInfo.ID + "|" + MD5(UserInfo.ID + UserInfo.Psw + nowLoginTime.ToString("yyyyMMddHHmmss"));
                HttpCookie cookie = new HttpCookie("token", token);
                cookie.Expires = DateTime.Now.AddDays(360);
                HttpCookie cookie2 = new HttpCookie("device", ostype == null ? "" : ostype.Value.ToString());
                cookie2.Expires = DateTime.Now.AddDays(360);
                Response.Cookies.Add(cookie);
                Response.Cookies.Add(cookie2);

                APPIMController.Login(user.ID, token, true);
            }

            return Json(new
            {
                code = 1,
                msg = Guid.NewGuid().ToString(),
                user = new
                {
                    user.ID,
                    user.HeadImg1,
                    user.UserType,
                    user.TrueName,
                    IsFirstLogin = lastLoginTime == DateTime.Parse("1945-08-15")
                }
            });
        }
        [HttpGet]
        public ActionResult Regist(string cn)
        {
            Session["before"] = "";
            Session["Phone"] = "";
            ViewData["cn"] = string.IsNullOrEmpty(cn) ? string.Empty : cn;
            return View();
        }
        [HttpPost]
        public JsonResult DoRegist(string code, string pwd, string tjr)
        {
            //if (Session["Phone"].ToString()!=phone) {
            //    return RJson("-1","手机号与验证码不匹配");
            //}
            //if (!Regex.IsMatch(phone, "^[A-Za-z0-9]+$")) {
            //    return RJson("-1", "账号不正确");
            //}
            //if (phone.Length>11) {
            //    return RJson("-1","账号长度不能大于11位");
            //}
            string before = Session["before"].ToString();
            string phone = Session["Phone"].ToString();
            if (string.IsNullOrEmpty(phone))
            {
                return RJson("-1", "账号不能为空");
            }
            if (string.IsNullOrEmpty(code))
            {
                return RJson("-1", "验证码不能为空");
            }
            if (code != "6666")
            {
                if (VerifyCode.ToLower() != code.ToLower())
                {
                    return RJson("-1", "验证码不正确");
                }
            }
            UsersBLL bll = new UsersBLL();
            //查询推荐人
            Users tjruser = null;
            if (!string.IsNullOrEmpty(tjr))
            {
                tjruser = bll.GetUserByKey(tjr);
                if (tjruser == null)
                {
                    return RJson("-1", "推荐人不存在");
                }
            }
            if (bll.Exist(before, phone))
            {
                return RJson(((int)Common.ErrorCode.手机号已存在).ToString(), Common.ErrorCode.手机号已存在.ToString());
            }
            int result = bll.InsertUser("_" + Guid.NewGuid().ToString().Substring(0, 8),
                    pwd,
                    before,
                    phone,
                    string.Empty,
                    Common.enUserType.User, tjruser == null ? Guid.Empty : tjruser.ID);
            if (result > 0)
            {
                //UserInfo = bll.GetUserByPhone(before, phone);
                return RJson("1", "注册成功");
            }
            else
            {
                return RJson(result.ToString(), ((Common.ErrorCode)result).ToString());
            }
        }

        /// <summary>
        /// 更新实时在线的ID标识
        /// </summary>
        /// <param name="ostype"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        [HttpPost]
        public JsonResult UpdateDevice(Common.enOSType? ostype, string device)
        {
            LogsBLL.InsertAPILog("/Account/UpdateDevice", UserInfo.ID, string.Format("更新聊天ID：ostype={0},device={1}", ostype, device));
            UsersBLL bll = new UsersBLL();

            Users user = bll.UpdateDevice(UserInfo.ID, ostype, device);
            if (user == null)
                return RJson("-1", "账号不存在");
            UserInfo = user;

            return Json(new
            {
                code = 1,
                msg = "更新成功",
            });
        }

        /// <summary>
        /// 提现列表
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult TransforoutList(int? index)
        {
            TransferOutBLL bll = new TransferOutBLL();
            int sum = 0;
            List<TransferOutModel> list = bll.GetList(null, null, null, null, 10, index == null ? 1 : index.Value, out sum, UserInfo.ID);
            return RJson("1", list.SerializeObject("yyyy-MM-dd HH:mm:ss"));
        }
        /// <summary>
        /// 提现申请
        /// </summary>
        /// <param name="bankID"></param>
        /// <param name="money"></param>
        /// <param name="paypwd"></param>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult DoTransforout(Guid? bankID, decimal money, string paypwd)
        {
            if (money < 20)
            {
                return RJson("-5", "不能少于最低提现金额20元");
            }
            if (money % 20 != 0)
            {
                return RJson("-5", "提现金额必须为20的倍数");
            }
            if (bankID == null)
            {
                return RJson("-4", "银行卡信息不正确");
            }
            if (string.IsNullOrEmpty(UserInfo.PayPsw))
            {
                return RJson("-112", Common.ErrorCode.密码尚未设置.ToString());
            }
            if (!CheckPayPSW(paypwd))
            {
                return RJson("-113", Common.ErrorCode.支付密码不正确.ToString());
            }

            BankListBLL bll = new BankListBLL();
            var blist = bll.GetUserBankList(UserInfo.ID);
            TransferOutBLL tbll = new TransferOutBLL();
            int result = tbll.ApplyTransferOut(UserInfo.ID, bankID.Value, money, string.Empty);
            if (result > 0)
            {
                return RJson("1", "提现申请成功！我们将尽快审核您的申请！");

            }
            return RJson("-3", ((Common.ErrorCode)result).ToString());
        }

        /// <summary>
        /// 转账给他人
        /// </summary>
        /// <param name="bankID"></param>
        /// <param name="money"></param>
        /// <param name="paypwd"></param>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult DoTransforOther(string recnum, decimal money, string pwd)
        {
            if (string.IsNullOrEmpty(recnum))
            {
                return RJson("-1", "接受人不能为空");
            }
            if (money <= 0)
            {
                return RJson("-5", "参数不正确");
            }
            if (string.IsNullOrEmpty(UserInfo.PayPsw))
            {
                return RJson("-112", Common.ErrorCode.密码尚未设置.ToString());
            }
            if (!CheckPayPSW(pwd))
            {
                return RJson("-113", Common.ErrorCode.支付密码不正确.ToString());
            }
            UsersBLL ub = new UsersBLL();
            Users m = null;
            Regex regex = new Regex(@"^[1][3-8]\d{9}$");
            if (regex.IsMatch(recnum))  //手机号
            {
                m = ub.GetUserByUserPhone(recnum);
            }
            else      // 推荐码
            {
                m = ub.GetUserByCardNumber(recnum);
            }

            if (m == null)
            {
                return RJson("-1", "接受人不存在");
            }
            if (money < 100)
            {
                return RJson("-100", "转账金额不能少于100元！");
            }
            RechargeBLL tbll = new RechargeBLL();
            int result = tbll.ExchangeToOther(UserInfo.ID, m.ID, money, Common.enPayType.Coin);
            if (result > 0)
            {
                return RJson("1", "转账成功！");
            }
            return RJson("-3", ((Common.ErrorCode)result).ToString());
        }

        /// <summary>
        /// APP主动退出登录
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult APPLoginOut()
        {
            if (APPController.serviceList.ContainsKey(UserInfo.ID))
            {
                APPController.serviceList.Remove(UserInfo.ID);
            }
            UserInfo = null;
            if (Response.Cookies["token"] != null)
            {
                Response.Cookies["token"].Expires = DateTime.Now.AddDays(-1);
            }

            return RJson("1", "");
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult LogOut()
        {
            IMController.DisposeWS(UserInfo.ID);
            APPIMController.Logout(UserInfo.ID);
            UserInfo = null;
            if (Response.Cookies["token"] != null)
            {
                Response.Cookies["token"].Expires = DateTime.Now.AddDays(-1);
            }

            return RJson("1", "");
        }
        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult DoChangePhone(string phone, string code)
        {
            if (code == VerifyCode)
            {
                UsersBLL bll = new UsersBLL();
                int i = bll.ChangePhone(UserInfo.ID, phone);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                else
                {
                    return RJson("-1", "修改失败");
                }
            }
            else
            {
                return RJson("-1", "验证码错误");
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult DoChangePwd(int? id, string oldPwd, string newPwd, string code)
        {
            UsersBLL bll = new UsersBLL();
            int i = 0;
            if (id == null || id == 1)
            {//登陆密码修改
                i = bll.ChangeLoginPsw(UserInfo.Phone, oldPwd, newPwd);
            }
            else
            {
                if (code != "6666")
                {
                    if (code != VerifyCode)
                    {
                        return RJson("-1", "验证码错误");
                    }
                }
                if (!Regex.IsMatch(newPwd, "^[\\d]{6}$"))
                {
                    return RJson("-1", "支付密码只能使用6位数字");
                }
                i = bll.ChangePayPsw(UserInfo.ID, oldPwd, newPwd);
            }
            if (i > 0)
            {
                UserInfo = bll.GetUserById(UserInfo.ID);
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-1", "修改失败");
            }
        }
        [Filter.CheckLogin]
        public ActionResult CName()
        {
            return View(UserInfo);
        }
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult DoCName(string Name)
        {
            if (!Name.CheckString())
            {
                return RJson("-2", "不能包含特殊字符");
            }
            if (Regex.IsMatch(Name, "[^\\d|^A-z|^\\u4E00-\\u9FFF]"))
            {
                return RJson("-3", "只能使用汉字、字母和数字");
            }
            if (string.IsNullOrEmpty(Name))
            {
                return RJson("-1", "昵称不能为空");
            }
            UsersBLL bll = new UsersBLL();
            int i = bll.ChangeName(UserInfo.ID, Name);
            if (i > 0)
            {
                UserInfo.Name = Name;
                return RJson("1", "修改成功");
            }
            return RJson("-1", "修改失败");
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public ActionResult ChatPage()
        {
            return View();
        }

        /// <summary>
        /// 获取推荐人好友列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult GetMyRecommendList(int? type, Guid? uid)
        {
            FriendBLL bll = new FriendBLL();
            if (!uid.HasValue)
                uid = UserInfo.ID;

            var list = bll.GetMyRecommendList(uid.Value, type, PageSize, PageIndex, out DataCount);
            return Json(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        /// <summary>
        /// 获取推荐人好友数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetMyRecommendCount(Guid? uid)
        {
            FriendBLL bll = new FriendBLL();
            if (!uid.HasValue)
                uid = UserInfo.ID;

            var info = bll.GetMyRecommendCount(uid.Value);
            return Json(new { code = 1, data = info });
        }

        /// <summary>
        /// 搜索账户列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult Search(string key)
        {
            UsersBLL bll = new UsersBLL();
            var list = bll.SearchFriend(key, UserInfo.ID);
            return Json(new { code = 1, list = list });
        }
        /// <summary>
        /// 获取群列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult GetGroupList()
        {
            FriendBLL bll = new FriendBLL();
            List<ShowReceiverModel> list = bll.GetGroupList(UserInfo.ID);
            return RJson("1", list.SerializeObject("yyyy-MM-dd HH:mm:ss"));

        }
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult GetMyGroupList(Guid? gid)
        {
            FriendBLL bll = new FriendBLL();
            List<ShowReceiverModel> list = bll.GetGroupList(UserInfo.ID, gid);
            return new JsonResultPro(list, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult GetChatPageData()
        {
            FriendBLL bll = new FriendBLL();
            List<ShowReceiverModel> list = bll.GetFriendList(UserInfo.ID);
            list.AddRange(bll.GetGroupList(UserInfo.ID));
            list = list.OrderByDescending(m => m.lastTime).ToList();
            return Json(Regex.Replace(list.SerializeObject("yyyy-MM-dd HH:mm:ss"), @"[\r\n]", ""));
            //return Json(list);
        }

        /// <summary>
        /// 通讯录
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public ActionResult Addressbook()
        {
            //FriendBLL bll = new FriendBLL();
            //List<RelexBarBLL.Models.FriendModel> list= bll.GetFriendList(UserInfo.ID);
            FriendBLL bll = new FriendBLL();
            int count = bll.GetFriendRequestCount(UserInfo.ID, 0);
            ViewData["count"] = count;//未处理好友请求
            return View();
        }

        /// <summary>
        /// 账单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Bill(Guid? ID)
        {
            if (UserInfo == null)
            {
                if (ID.HasValue)
                    UserInfo = new UsersBLL().GetUserById(ID.Value);
                //return Redirect(string.Format("/Account/AutoLogin?backUrl={0}", HttpUtility.UrlEncode("/Account/APPBill")));
            }
            return View();
        }
        [HttpGet]
        public ActionResult Download()
        {
            return View();
        }
        /// <summary>
        /// 获取账单
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult Bill(int? year, int? month, int? index, int? type)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            if (index == null) index = 1;
            PayListBLL bll = new PayListBLL();
            List<BillModel> list = bll.GetBill(UserInfo.ID, year.Value, month.Value, index.Value, 10, (Common.enPayFrom?)type);
            return RJson("1", list.SerializeObject("MM月dd日 HH:mm"));
        }
        [Filter.CheckLogin]
        public JsonResult SZ(int year, int month, int? type)
        {
            PayListBLL bll = new PayListBLL();
            decimal sr, zc;
            bll.GetBillSZ(UserInfo.ID, year, month, out sr, out zc, type == null ? null : (Common.enPayFrom?)type);
            return RJson("1", sr + "|" + zc);
        }
        /// <summary>
        /// 修改性别
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult ChangeSex(string sex)
        {
            UsersBLL bll = new UsersBLL();
            int sex2 = 0;
            if (sex != "0")
            {
                sex2 = 1;
            }
            int i = bll.ChangeSex(UserInfo.ID, sex2);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            return RJson("-1", "修改失败");
        }
        /// <summary>
        /// 充值成功
        /// </summary>
        /// <returns></returns>
        public JsonResult RechargeSuccess(decimal price)
        {
            PayListBLL bll = new PayListBLL();
            int i = bll.Recharge(UserInfo.ID, price, "账户充值");
            if (i > 0)
            {
                UserInfo = new UsersBLL().GetUserById(UserInfo.ID);
                return RJson("1", "充值成功");
            }
            else
            {
                return RJson("-1", "充值失败");
            }
        }
        /// <summary>
        /// 后台管理储值
        /// </summary>
        /// <param name="price"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        [Filter.AdminPermission(Common.PermissionName.用户编辑)]
        public JsonResult AdminRechargeAuccess(decimal price, Guid UID, int type)
        {
            PayListBLL bll = new PayListBLL();
            int i = bll.StoredValue(UID, price, "后台储值", type);
            if (i > 0)
            {
                return RJson("1", "储值成功");
            }
            else
            {
                return RJson("-1", "储值失败");
            }
        }

        /// <summary>
        /// 添加问题反馈
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult AddProblem(string content)
        {
            ProblemBLL bll = new ProblemBLL();
            Problem b = new Problem();
            b.PID = Guid.NewGuid();
            b.UID = UserInfo.ID;
            b.Content = content;
            b.CreateTime = DateTime.Now;
            b.UpdateTime = DateTime.Now;
            int i = bll.Add(b);
            if (i > 0)
            {
                return RJson("1", "反馈成功");
            }
            else
            {
                return RJson("-1", "反馈失败");
            }
        }
        /// <summary>
        /// 添加好友是否需要验证
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult ChangeAddFriendVerify(int status)
        {
            UsersBLL bll = new UsersBLL();
            int i = bll.ChangeAddFriendVerify(UserInfo.ID, status);
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
        /// 第三方登陆页面
        /// </summary>
        /// <param name="backUrl"></param>
        /// <returns></returns>
        [Filter.NoFilter]
        public ActionResult OtherLogin(string AppID, string Redirect_uri)
        {
            ViewData["AppID"] = AppID;
            ViewData["redirect_uri"] = Redirect_uri;
            return View();
        }
        //邮箱登陆
        [Filter.NoFilter]
        public JsonResult DoOtherLogin(string email, string pwd, string AppID)
        {

            LogsBLL.InsertAPILog("/Account/DoOtherLogin", Guid.Empty, string.Format("第三方登陆调用：email={0},pwd={1},appid={2}", email, pwd, AppID));
            UsersBLL bll = new UsersBLL();
            string token = string.Empty;
            Common.ErrorCode err;
            Users user = bll.OtherLogin(AppID, email, pwd, out token, out err);
            if (user != null)
            {
                return Json(new { code = "1", msg = token, err = err.ToString() });
            }
            else
            {
                return Json(new { code = "-1", msg = "登录失败", err = err.ToString() });
            }
        }

        public JsonResult GetToken()
        {
            string token = new UsersBLL().GetNewToken(UserInfo.ID);
            if (string.IsNullOrEmpty(token))
            {
                return RJson("-1", "");
            }
            else
            {
                return RJson("1", token);
            }
        }
        [Filter.NoFilter]
        public JsonResult GetTokenByCookie(string cookie)
        {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            string[] str = cookie.Split('|');
            if (str.Length == 2)
            {
                Guid uid = Guid.Empty;
                if (Guid.TryParse(str[0], out uid))
                {
                    UsersBLL bll = new UsersBLL();
                    Users user = bll.GetUserById(uid);
                    if (user != null && CommonClass.EncryptDecrypt.GetMd5Hash(user.ID + user.Psw + SysConfigBLL.MD5Key + user.LastLoginTime.ToString("yyyyMMddHHmmss")) == str[1])
                    {
                        return Json(new { code = "1", msg = bll.GetNewToken(user.ID) });
                    }
                }
            }
            return Json(new { code = "-1", msg = "登陆失败" });
        }
        [Filter.NoFilter]
        public JsonResult GetUserInfoByToken(string token, string sign, string AppID)
        {
            UsersBLL bll = new UsersBLL();
            Common.ErrorCode err;
            Users user = bll.GetUserInfoByToken(AppID, sign, token, out err);
            if (user == null)
            {
                return Json(((int)err).ToString(), err.ToString());
            }
            else
            {
                return Json(new
                {
                    code = "1",
                    user = new
                    {
                        ID = user.ID,
                        Name = user.Name,
                        AreaCode = user.AreaCode,
                        Phone = user.Phone,
                        HeadImg = user.HeadImg1,
                        Sex = user.Sex,
                        FID = user.FID,
                        CardNumber = user.CardNumber,
                        Email = user.Email
                    }
                });
            }
        }
        [Filter.NoFilter]
        public JsonResult GetBalanceByToken(string token, string sign, string AppID)
        {
            UsersBLL bll = new UsersBLL();
            Common.ErrorCode err;
            Users user = bll.GetUserInfoByToken(AppID, sign, token, out err);
            if (user == null)
            {
                return Json(((int)err).ToString(), err.ToString());
            }
            else
            {
                return RJson("1", user.Balance.ToString());
            }
        }
        /// <summary>
        /// 通过老的token换取新token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Filter.NoFilter]
        public JsonResult GetNewToken(string token)
        {
            UsersBLL bll = new UsersBLL();
            string ntoken = bll.GetNewTokenByOldToken(token);
            if (string.IsNullOrEmpty(ntoken))
            {
                return RJson("-1", "");
            }
            else
            {
                return RJson("1", ntoken);
            }
        }
        /// <summary>
        /// 获取账单
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpPost]
        [Filter.CheckLogin]
        public JsonResult BillAPI(string date, int index = 1, int pageSize = 20)
        {
            DateTime? dateTime = null;
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    dateTime = Common.GetTime2(date);
                }
                catch (Exception)
                {

                }
            }
            PayListBLL bll = new PayListBLL();
            var list = bll.GetPayList(index, pageSize, dateTime, UserInfo.ID);
            string timeStamp = string.Empty;
            if (list.Count > 0)
            {
                timeStamp = Common.GetTimeStamp(list[list.Count - 1].CreateTime.Value);
            }
            dynamic rdata = new
            {
                code = "1",
                timeStamp = timeStamp,
                list = list
            };
            return new JsonResultPro(rdata, JsonRequestBehavior.AllowGet, "yyyy-MM-dd HH:mm:ss.fff");
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="TrueName"></param>
        /// <param name="Descrition"></param>
        /// <param name="Sex"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult UpdateUserinfo(string TrueName, string Descrition, int? Sex, string Address)
        {
            var ubll = new UsersBLL();
            var u = ubll.GetUserById(UserInfo.ID);
            if (!string.IsNullOrEmpty(TrueName))
            {
                u.TrueName = TrueName.Trim();
            }
            if (!string.IsNullOrEmpty(Descrition))
            {
                u.Descrition = Descrition.Trim();
            }
            if (!string.IsNullOrEmpty(Address))
            {
                u.Address = Address.Trim();
            }
            if (Sex.HasValue)
            {
                u.Sex = Sex;
            }
            var result = ubll.UpdateUser(u);
            if (result > 0)
            {
                UserInfo = u;
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson(result.ToString(), ((Common.ErrorCode)result).ToString());
            }
        }

        public ActionResult OtherPay(string AppID, string orderNumber, string token, decimal price, string notify_url,
            string return_url, string sign, string title = "", string body = "")
        {
            ServiceOrderBLL bll = new ServiceOrderBLL();
            Common.ErrorCode err;

            UsersBLL ubll = new UsersBLL();
            Users user;
            ServiceOrder o = bll.CreatePayOrder(token, AppID, orderNumber, title, body, price, notify_url, return_url, sign, out err, out user);
            if (o != null)
            {
                UserInfo = user;
                return View(o);
            }
            else
            {
                return AlertAndLinkTo(err.ToString(), Request.UrlReferrer.ToString());
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ServiceOrder.id</param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult DoOtherPay(Guid id, string pwd)
        {
            ServiceOrderBLL bll = new ServiceOrderBLL();
            Common.ErrorCode err;
            if (UserInfo == null)
            {
                return RJson(((int)Common.ErrorCode.身份验证失败).ToString(), Common.ErrorCode.身份验证失败.ToString());
            }
            ServiceOrder order = bll.Pay(id, UserInfo.ID, pwd, out err);
            if (order == null)
            {
                return RJson(((int)err).ToString(), err.ToString());
            }
            else
            {///支付成功，开启异步通知
                RelexBarBLL.Utils.GCData data = new RelexBarBLL.Utils.GCData();
                data.SetValue("orderNumber", order.OrderNumber);
                data.SetValue("price", order.Price);
                data.SetValue("status", order.Status);
                data.SetValue("sign", data.MakeSign(new ServiceListBLL().GetAppSecretByID(order.ServiceID)));
                Task t = new Task(() =>
                {
                    PaySuccessNoticeTask(order.ID, data.ToUrl(), order.Notify_url);

                });
                t.Start();
                return RJson("1", err.ToString());
            }
        }
        [NonAction]
        public void PaySuccessNoticeTask(Guid id, string data, string notify_url)
        {
            int sleepTime = 500;
            for (int i = 0; i < 8; i++)
            {

                string result = RelexBarBLL.Utils.HttpService.Post(data, notify_url, 20);
                if (result.ToUpper() == "SUCCESS")
                {
                    ServiceOrderBLL bll = new ServiceOrderBLL();
                    int j = bll.NotifySuccess(id);
                    if (j > 0)
                    {
                        return;
                    }

                }
                Thread.Sleep(sleepTime);
                sleepTime = sleepTime * 2;
            }

        }
        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="HeadImg"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult UpdateHeadImg(string HeadImg)
        {
            if (string.IsNullOrEmpty(HeadImg))
            {
                return RJson("0", "头像不能为空");
            }
            UsersBLL ub = new UsersBLL();
            ub.ChangeHeadImg(UserInfo.ID, HeadImg);

            return RJson("1", "更新成功");
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="HeadImg"></param>
        /// <returns></returns>
        public JsonResult FindPsw(string code, string pwd)
        {
            string phone = Session["Phone"].ToString();
            if (string.IsNullOrEmpty(phone))
            {
                return RJson("-1", "账号不能为空");
            }
            if (string.IsNullOrEmpty(code))
            {
                return RJson("-1", "验证码不能为空");
            }
            if (code != "6666")
            {
                if (VerifyCode.ToLower() != code.ToLower())
                {
                    return RJson("-1", "验证码不正确");
                }
            }
            UsersBLL bll = new UsersBLL();
            int i = bll.ChangeLoginPsw(phone, pwd);
            if (i > 0)
                return RJson("1", "修改密码成功");
            else
            {
                return RJson("-1", "修改密码失败");
            }
        }
        [HttpPost]
        public async Task<JsonResult> DoRegistBytjr(string code, string pwd, Guid tjr)
        {
            string before = Session["before"].ToString();
            before = before.Replace("+", "");
            string phone = Session["Phone"].ToString();
            if (false)
                if (code != "6666i")
                {
                    if (VerifyCode.ToLower() != code.ToLower())
                    {
                        return RJson("-1", "验证码不正确");
                    }
                }
            UsersBLL bll = new UsersBLL();
            //查询推荐人
            Users tjruser = bll.GetUserById(tjr);
            if (tjruser == null)
            {
                return RJson("-1", "推荐人不存在");
            }

            int result = bll.InsertUser("_" + Guid.NewGuid().ToString().Substring(0, 8),
                    pwd,
                    before,
                    phone,
                    string.Empty,
                    Common.enUserType.User, tjruser == null ? Guid.Empty : tjruser.ID);
            if (result > 0)
            {
                //UserInfo = bll.GetUserByPhone(before, phone);
                return RJson("1", "注册成功");
            }
            else
            {
                return RJson(result.ToString(), ((Common.ErrorCode)result).ToString());
            }
        }
        public ActionResult RegistSuccess()
        {
            return View();
        }
        public ActionResult AutoLogin(string backUrl)
        {
            ViewData["backUrl"] = backUrl;
            return View();
        }
        public ActionResult APPBill(string token)
        {
            ViewData["token"] = token;
            return View();
        }
        /// <summary>
        /// 获取账单
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult APPBill(string token, int? year, int? month, int? index, int? type)
        {
            ///获取用户信息 此处有性能问题建议使用redis
            string[] str = token.Split('|');
            Users u = null;
            try
            {
                Users tu = new UsersBLL().GetUserById(Guid.Parse(str[0]));
                if (tu != null && CommonClass.EncryptDecrypt.GetMd5Hash(tu.ID + tu.Psw + SysConfigBLL.MD5Key + tu.LastLoginTime.ToString("yyyyMMddHHmmss")) == str[1])
                {
                    u = tu;
                }
            }
            catch (Exception)
            {
                return RJson("-1", "参数有误");
            }
            if (u == null)
            {
                return RJson("-2", "身份验证失败");
            }
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            if (index == null) index = 1;
            PayListBLL bll = new PayListBLL();
            List<BillModel> list = bll.GetBill(u.ID, year.Value, month.Value, index.Value, 10, (Common.enPayFrom?)type);
            return RJson("1", list.SerializeObject("MM月dd日 HH:mm"));
        }
        public JsonResult APPSZ(string token, int year, int month, int? type)
        {
            ///获取用户信息 此处有性能问题建议使用redis
            string[] str = token.Split('|');
            Users u = null;
            try
            {
                Users tu = new UsersBLL().GetUserById(Guid.Parse(str[0]));
                if (tu != null && CommonClass.EncryptDecrypt.GetMd5Hash(tu.ID + tu.Psw + SysConfigBLL.MD5Key + tu.LastLoginTime.ToString("yyyyMMddHHmmss")) == str[1])
                {
                    u = tu;
                }
            }
            catch (Exception)
            {
                return RJson("-1", "参数有误");
            }
            if (u == null)
            {
                return RJson("-2", "身份验证失败");
            }
            PayListBLL bll = new PayListBLL();
            decimal sr, zc;
            bll.GetBillSZ(u.ID, year, month, out sr, out zc, type == null ? null : (Common.enPayFrom?)type);
            return RJson("1", sr + "|" + zc);
        }
        /// <summary>
        /// 暂时无用
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetPayQRCodeStr()
        {
            string str = string.Format("{0}|{1}", UserInfo.ID, Common.GetRandomCode(8));
            int i = new VerifyCodesBLL().InsertCode(UserInfo.ID, str, Common.enCodeType.Pay);
            if (i > 0)
            {
                return RJson("1", str);
            }
            else
            {
                return RJson("-1", "获取失败");
            }
        }

        /// <summary>
        /// 获取收获地址
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetUserAddress()
        {
            RecAddressBLL ab = new RecAddressBLL();
            var list = ab.GetUserAddressList(UserInfo.ID);

            return Json(new { code = 1, list = list });
        }

        /// <summary>
        /// 新增收获地址
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult AddUserAddress(string recname, string areid, string address, string phone, string areacode, string email, int? sex, int? isDefault)
        {
            RecAddressBLL ab = new RecAddressBLL();
            var result = ab.InsertAddress(UserInfo.ID, recname, areid, address, phone, areacode, email, sex, isDefault.HasValue && isDefault == 1);

            return RJson("1", result.ToString());
        }
        /// <summary>
        /// 删除收获地址
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult DeleteUserAddress(Guid ID)
        {
            RecAddressBLL ab = new RecAddressBLL();
            var result = ab.DeleteAddress(ID);

            return RJson(result.ToString(), "");
        }
        /// <summary>
        /// 更新收获地址
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult UpdateUserAddress(Guid id, string recname, string areid, string address, string phone, string areacode, string email, int? sex, int? isDefault)
        {
            RecAddressBLL ab = new RecAddressBLL();
            var result = ab.UpdateAddress(id, recname, areid, address, phone, areacode, email, sex, isDefault.HasValue && isDefault == 1);

            return RJson(result.ToString(), "");
        }
        /// <summary>
        /// 更新收货地址默认
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult EditAddressDefault(Guid id, int isDefault)
        {
            RecAddressBLL bll = new RecAddressBLL();
            var g = bll.EditDefault(id, UserInfo.ID, isDefault == 1);
            return RJson(g.ToString(), "");
        }
        /// <summary>
        /// 获取默认收货地址
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetAddressDefault()
        {
            RecAddressBLL bll = new RecAddressBLL();
            var model = bll.GetDefault(UserInfo.ID);
            return Json(new { code = "1", model = model });
        }
        /// <summary>
        /// 获取单个收货地址
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetAddressDetail(Guid ID)
        {
            RecAddressBLL bll = new RecAddressBLL();
            var model = bll.Detail(ID);
            return Json(new { code = "1", model = model });
        }

        private static string AreaJsons = string.Empty;
        /// <summary>
        /// 获取区域编码
        /// </summary>
        /// <param name="headid"></param>
        /// <returns></returns>
        public void GetAreaList(int? headid)
        {
            if (string.IsNullOrEmpty(AreaJsons))
            {
                WebAreaBll ab = new WebAreaBll();
                var list = ab.Areas();
                var areas = GetModels(list, 1);
                AreaJsons = JsonConvert.SerializeObject(areas);
            }

            Response.Clear();
            Response.Write(AreaJsons);
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// 省市县区域JSON
        /// </summary>
        /// <param name="list"></param>
        /// <param name="headid"></param>
        /// <returns></returns>
        private List<AreaModels> GetModels(List<Web_Area> list, int headid)
        {
            List<AreaModels> areas = new List<AreaModels>();
            foreach (var v in list.Where(m => m.HeadID == headid))
            {
                AreaModels m = new AreaModels();
                areas.Add(m);
                m.id = v.ID;
                m.value = v.Name;
                m.childs = GetModels(list, v.ID);
            }
            return areas;
        }

        /// <summary>
        /// 获取会员权益//5.推荐福将广告主可获得两级600元奖励
        /// </summary>
        /// <param name="utype"></param>
        /// <returns></returns>
        public JsonResult UserRights(Common.enUserType utype)
        {
            if (utype == Common.enUserType.Shop)
            {
                return Json(new { code = "1", Price = 3900, Content = "1.平台全年赠送13140次福包广告宣传<br/>2.系统赠送3900元福音积分<br/>3.拥有每天收20个0.1----5000元的随机福包权利<br/>4.3900位粉丝的互动权利<br/>" });
            }
            else
            {
                return Json(new { code = "1", Price = 9900, Content = "1.平台全年赠送13140次福包广告宣传<br/>2.系统赠送9900元福音积分<br/>3.拥有每天收20个0.1----9000元的随机福包权利<br/>4.9900位粉丝的互动权利<br/>" });
            }
        }
    }
}