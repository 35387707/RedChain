using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarDLL;
using RelexBarBLL;
using RelaxBarWeb_MVC.Utils;
using static RelexBarBLL.Common;
using System.Data.SqlClient;
using System.Data;
using RelexBarBLL.Models;
using System.IO;
using System.Text;
using NPOI;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Threading.Tasks;

namespace RelaxBarWeb_MVC.Controllers
{
    public static class FinalData
    {
        public static object syncobj = new object();
        public static string Date { get; set; }
        public static string NewUser { get; set; }
        public static string loginCount { get; set; }
        public static DateTime NewUserCreateTime { get; set; }

        public static string NewShopUser { get; set; }

        public static string NewAgentUser { get; set; }

        public static string OrderPayCount { get; set; }
        public static string OrderPriceCount { get; set; }

        public static string OrderPayPeopleCount { get; set; }

        public static string OrderPriceTotal { get; set; }
    }
    [Filter.CheckAdmin]
    public class AdminController : Controller
    {
        [NonAction]
        public JsonResult RJson(string icode, string imsg)
        {
            return Json(new { code = icode, msg = imsg });
        }
        [NonAction]
        public string MD5(string source)
        {
            return CommonClass.EncryptDecrypt.GetMd5Hash(source + SysConfigBLL.MD5Key);
        }
        [Filter.NoFilter]
        public ActionResult Login()
        {

            return View();
        }

        public ActionResult Index()
        {
            //if (Response.Cookies["adminToken"] == null || Session["admin"] == null)
            //    return Redirect("Login");
            return View(Session["admin"] as AdminUser);
        }


        public JsonResult GetLogList()
        {
            LogsBLL bll = new LogsBLL();
            var list = bll.GetLogsByLogType();

            return Json(new { code = 1, list = list });//获取日志列表
        }

        public ActionResult Default()
        {
            ReportBLL report = new ReportBLL();
            if (string.IsNullOrEmpty(FinalData.Date) || string.IsNullOrEmpty(FinalData.NewUser) || DateTime.Now.Day != FinalData.NewUserCreateTime.Day)
            {
                lock (FinalData.syncobj)
                {
                    //string[] lable = new string[7];
                    //string[] newusercount = new string[7];
                    //string[] logincount = new string[7];

                    //for (int i = 1; i <= 7; i++)
                    //{
                    //    lable[i - 1] = "'" + DateTime.Now.AddDays(0 - i).ToString("MM-dd") + "'";
                    //    newusercount[i - 1] = report.GetNewUser(DateTime.Now.AddDays(0 - i),enUserType.User).ToString();
                    //    logincount[i - 1] = report.GetLoginCount(DateTime.Now.AddDays(0 - i)).ToString();
                    //}
                    //string newshopusercount = report.GetShopUser(DateTime.Now.AddDays(-1),enUserType.Shop).ToString();
                    //string newagentusercount = report.GetAgentUser(DateTime.Now.AddDays(-1),enUserType.Agent).ToString();
                    //FinalData.Date = string.Join(",", lable);
                    //FinalData.NewUser = string.Join(",", newusercount);
                    //FinalData.loginCount = string.Join(",", logincount);
                    //FinalData.NewUserCreateTime = DateTime.Now;
                    //FinalData.NewShopUser = newshopusercount;
                    //FinalData.NewAgentUser = newagentusercount;
                }
            }

            //ViewData["lable"] = FinalData.Date;
            //ViewData["newusers"] = FinalData.NewUser;
            //ViewData["loginCount"] = FinalData.loginCount;

            ViewData["totalUser"] = report.GetUserTotal(null);//会员总数
            ViewData["NowUser"] = report.GetUserTypeByDate(DateTime.Now, null).ToString(); //今日新增会员

            ViewData["newuser"] = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.User).ToString(); //昨日新增福星
            ViewData["newshopuser"] = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.Shop).ToString(); //昨日新增福相
            ViewData["newagentuser"] = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.Agent).ToString(); //昨日新增福相

            // ViewData["systemRedpack"] = report.GetRedpackByRedType(enRedType.System);//系统发福包总数
            ViewData["systemRedpack"] = report.GetRedpackByRedType(null);//系统发福包总数

            ViewData["singleRedpack"] = report.GetRedpackByRedType(enRedType.Single);//用户发福包总数

            ViewData["userRedpack"] = report.GetRepackByStatus(enPacketStatus.Used, null);//已收福包数
            ViewData["activedRedpack"] = report.GetRepackByStatus(enPacketStatus.Actived, null);//待收福包数

            ViewData["SendReds"] = report.TotalPays(Common.enPayFrom.RedPaged, Common.enPayInOutType.Out, Common.enPayType.Coin); //发福包总额

            ViewData["RecReds"] = report.TotalPays(Common.enPayFrom.RedPaged, Common.enPayInOutType.In, Common.enPayType.Coin);  //收福包总额

            ViewData["OtherPay"] = report.OtherPayPrice(); //第三方收入

            ViewData["TotalIn"] = report.GetTotalInComeVal(); //总收入

            ViewData["TotalOut"] = report.GetTotalOutComeVal(); //总支出

            ViewData["FUIDCount"] = report.GetFUIDCount(); //总推荐数

            ViewData["loginCount"] = report.GetLoginCount(); //总访问数

            return PartialView();
        }
        #region 用户管理

        //用户管理
        public ActionResult UserManager()
        {
            ReportBLL report = new ReportBLL();
            ViewData["totalUser"] = report.GetUserTotal(null).ToString();//会员总数
            ViewData["oneUser"] = report.GetUserTotal(enUserType.User).ToString();//福星总数
            ViewData["shopUser"] = report.GetUserTotal(enUserType.Shop).ToString();//福将总数
            ViewData["agentUser"] = report.GetUserTotal(enUserType.Agent).ToString();//福相总数

            ViewData["YesterDaynewuser"] = report.GetYesterDayUser(DateTime.Now.AddDays(-1)).ToString(); //昨日新增福星
            ViewData["newuser"] = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.User).ToString(); //昨日新增福星
            ViewData["newshopuser"] = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.Shop).ToString(); //昨日新增福相
            ViewData["newagentuser"] = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.Agent).ToString(); //昨日新增福相
            return PartialView();
        }
        public ActionResult UserList(int? index, string key, string UserType, int pageSize = 10)
        {
            RelexBarBLL.UsersBLL bll = new RelexBarBLL.UsersBLL();
            int sum;
            List<Users> list = bll.GetUsersSearch(key, UserType, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        
        public ActionResult UserDetailManager(Guid ID)
        {
            ViewData["IID"] = ID;

            RelexBarBLL.UsersBLL bll = new RelexBarBLL.UsersBLL();
            Users list = bll.GetUserById(ID);

            ViewData["FName"] = list.FID.HasValue && list.FID.Value != Guid.Empty ? bll.GetUserById(list.FID.Value).Phone : "无";

            return PartialView();
        }

        public ActionResult UserQuickManager()
        {
            return PartialView();
        }

        public ActionResult UserQuickList(string phone, string wxname, string name, enUserType? UserType, string FromTo, string card, string tag, string shopnum, string shoptype, string money, string money2, int index = 1, int pageSize = 10)
        {
            UsersBLL bll = new UsersBLL();
            int sum = 0;
            List<RelexBarBLL.Models.SearchQuickUser> list = bll.GetUsersQuickSearch(phone, wxname, name, UserType, FromTo, card, tag, shopnum, shoptype, money, money2, index, pageSize, out sum);
            @ViewData["sum"] = sum;
            return PartialView(list);
            //PayListBLL bll = new PayListBLL();
            //int sum = 0;
            //List<RelexBarBLL.Models.AdminPayListModel> list = bll.GetPayList(phone, FromTo, InOut, beginTime, endTime, index, pageSize, out sum);
            //@ViewData["sum"] = sum;
        }

        /// <summary>
        /// 获取银行列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBankList(Guid UID)
        {
            BankListBLL bll = new BankListBLL();
            List<BankList> list = bll.GetUserBankList(UID);
            return RJson("1", list.SerializeObject());
        }
        /// <summary>
        /// 分页获取银行列表
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetBankListManager(Guid UID, int? index, int pageSize = 10)
        {
            BankListBLL bll = new BankListBLL();
            int sum;
            List<BankList> list = bll.GetUsersBankList(UID, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);

        }

        /// <summary>
        /// 分页获取团队列表
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetMyRecommendList(Guid UID, int? index, int pageSize = 10)
        {
            FriendBLL bll = new FriendBLL();
            UsersBLL bll2 = new UsersBLL();
            Users list2 = bll2.GetUserById(UID);
            int DataCount;
            //if (list2.FID.HasValue && list2.FID.Value != Guid.Empty)
            //{
            //    var list = bll.GetMyRecommendList(list2.FID.Value, null, pageSize, index == null ? 1 : index.Value, out DataCount);

            //    return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json(new { code = 1, pagecount = 0, list = "" }, JsonRequestBehavior.AllowGet);
            //}
            var list = bll.GetMyRecommendList(UID, null, pageSize, index == null ? 1 : index.Value, out DataCount);
            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分页获取充值记录
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetRechargeList(Guid UID, int? index, int pageSize = 10)
        {

            PayListBLL bll = new PayListBLL();
            int DataCount;
            var list = bll.GetRechargeList(UID, pageSize, index == null ? 1 : index.Value, out DataCount);
            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 分页获取储值金额
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetStoredList(Guid UID, int? index, int pageSize = 10)
        {

            PayListBLL bll = new PayListBLL();
            int DataCount;
            var list = bll.GetStoredList(UID, pageSize, index == null ? 1 : index.Value, out DataCount);
            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 分页获取转账记录
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetExchangeList(Guid UID, int? index, int pageSize = 10)
        {

            PayListBLL bll = new PayListBLL();
            int DataCount;
            var list = bll.GetExchangeList(UID, pageSize, index == null ? 1 : index.Value, out DataCount);
            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);

        }

        /// 获取已发出的红包列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSendPacketList(Guid UID, int? index, int pageSize = 10)
        {
            int DataCount;
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetSendPacketList(UID, pageSize, index == null ? 1 : index.Value, out DataCount);

            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);//获取所有红包列表
        }

        /// <summary>
        /// 获取已领取红包列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRecPacketList(Guid UID, int? index, int pageSize = 10)
        {
            int DataCount;
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetRecPacketList(UID, pageSize, index == null ? 1 : index.Value, out DataCount);
            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);//获取所有红包列表
        }
        /// <summary>
        /// 更改用户状态
        /// </summary>
        /// <returns></returns>
        [Filter.AdminPermission(Common.PermissionName.用户编辑)]
        public JsonResult CUStatus(Guid UID, int status)
        {
            UsersBLL bll = new UsersBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeUserStatus(UID, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult EditUser(Guid? ID)
        {
            Users u;
            if (ID != null)
            {
                u = new UsersBLL().GetUserById(ID.Value);
            }
            else
            {
                u = new Users();
            }
            return View(u);
        }
        public JsonResult DoEditUser(Guid? ID, string AreaCode, string Phone, string TrueName, string HeadImg1, int Sex, decimal Balance
            , int UserType, string Psw)
        {
            AreaCode = AreaCode.Replace("+", "");
            UsersBLL bll = new UsersBLL();
            if (ID == null || ID == Guid.Empty)
            {
                int i = bll.InsertUser("_" + Guid.NewGuid().ToString().Substring(0, 8), MD5(Psw), AreaCode, Phone, string.Empty, (enUserType)UserType, Guid.Empty);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败：" + (ErrorCode)i);
                }

            }
            else
            {
                Users user = null;
                if (!string.IsNullOrEmpty(Phone))
                {
                    user = bll.GetUserByPhone(AreaCode, Phone);
                    if (user != null && user.ID != ID)
                    {
                        return RJson("-3", "手机号已被使用");
                    }
                }
                Users u;
                if (user == null)
                {
                    u = bll.GetUserById(ID.Value);
                }
                else
                {
                    u = user;
                }
                if (u == null)
                {
                    return RJson("-1", "用户不存在");
                }

                u.Phone = Phone;
                u.TrueName = TrueName;
                u.HeadImg1 = HeadImg1;
                u.Sex = Sex;
                u.Balance = Balance;
                u.UserType = UserType;
                u.UpdateTime = DateTime.Now;
                int i = bll.UpdateUser(u);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }
        }


        //用户身份审核
        public ActionResult UserCheckManager()
        {
            return PartialView();
        }

        public ActionResult UserCheckList(int? index, string key, int pageSize = 10)
        {
            RelexBarBLL.UsersBLL bll = new RelexBarBLL.UsersBLL();
            int sum;
            List<Users> list = bll.GetUsersRealCheckSearch(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        public JsonResult GetTeamUser(Guid UID)
        {
            int count;
            FriendBLL bll = new FriendBLL();
            var list = bll.GetMyRecommendList(UID, null, 300, 1, out count);
            return Json(new { code = 1, list = list });
        }

        public JsonResult GetUser(Guid UID)
        {
            UsersBLL bll = new UsersBLL();
            var list = bll.GetUserById(UID);
            return Json(new { code = 1, list = list });
        }

        [Filter.AdminPermission(Common.PermissionName.用户审核)]
        public JsonResult UserCheckUpdate(Guid ID, int status, string remark)
        {

            UsersBLL bll = new UsersBLL();
            enRealCheckStatus s = status == 2 ? enRealCheckStatus.已验证 : enRealCheckStatus.不通过;
            int i = bll.AcceptRealCheck(ID, s, remark);
            if (i > 0)
            {
                return RJson("1", "已处理");
            }
            else
            {
                return RJson(i.ToString(), "处理失败");
            }

        }



        public ActionResult UserStoredManager()
        {
            return PartialView();
        }

        public ActionResult UserStoredList(int? index, string key, string UserType, int pageSize = 10)
        {
            RelexBarBLL.UsersBLL bll = new RelexBarBLL.UsersBLL();
            int sum;
            List<Users> list = bll.GetUsersSearch(key, UserType, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        #endregion

        #region 账单管理

        //账单管理
        public ActionResult PayListManager()
        {
            ReportBLL report = new ReportBLL();

            //ViewData["SystemPaylist"] = report.GetPayListValByFromTo(null).ToString();//系统福包总计

            //ViewData["RedPagedPaylist"] = report.GetPayListValByFromTo(enPayFrom.RedPaged).ToString();//用户福包总计

            ViewData["SystemPaylist"] = report.GetRedpackTotalPriceByRedType(null).ToString();//系统福包总计

            ViewData["RedPagedPaylist"] = report.GetRedpackTotalPriceByRedType(enRedType.Single).ToString();//用户福包总计

            // ViewData["OtherPayPaylist"] = report.GetPayListValByFromTo(enPayFrom.OtherPay).ToString();//平台福包总计
            ViewData["StoredPayPaylist"] = report.GetPayListValByFromTo(enPayFrom.StoredValue).ToString();//储值

            ViewData["RechargePaylist"] = report.GetPayListValByFromTo(enPayFrom.Recharge).ToString();//充值总计

            //  ViewData["TransforPaylist"] = report.GetPayListValByFromTo(enPayFrom.Transfor).ToString();//提现总计
            ViewData["TransforPaylist"] = report.GetTransferOutValByStatus(enApplyStatus.Success).ToString();

            ViewData["CommissionPaylist"] = report.GetPayListValByFromTo(enPayFrom.Reward).ToString();//福包分润总计

            ViewData["RewardPayPaylist"] = report.GetPayListValByFromTo(enPayFrom.Commission).ToString();//推荐奖励总计

            ViewData["ExchangePaylist"] = report.GetPayListValByFromTo(enPayFrom.Exchange).ToString();//转账总计

            return PartialView();
        }

        /// <summary>
        /// 查询交易记录
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="FromTo"></param>
        /// <param name="InOut"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult PayList(string phone, enPayFrom? FromTo, DateTime? beginTime, DateTime? endTime, enPayInOutType? InOut, enPayType? PayType, string remark, int index = 1, int pageSize = 10)
        {
            PayListBLL bll = new PayListBLL();
            int sum = 0;
            List<RelexBarBLL.Models.AdminPayListModel> list = bll.GetPayList(phone, FromTo, InOut, PayType, beginTime, endTime, remark, index, pageSize, out sum);
            @ViewData["sum"] = sum;
            return PartialView(list);
        }

        public JsonResult GetTotalInOut(DateTime? beginTime, DateTime? endTime)
        {
            PayListBLL bll = new PayListBLL();
            return Json(bll.GetTotalInOut(beginTime, endTime));
        }

        //收支管理中系统福包/用户福包
        public JsonResult GetRedPacketListByRedType(string key, enRedType? redType, string userType, DateTime? beginTime, DateTime? endTime, int? index, int pageSize = 10)
        {
            int DataCount;
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetRedPacketListByRedType(key, userType, redType, beginTime, endTime, pageSize, index == null ? 1 : index.Value, out DataCount);

            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);//获取所有红包列表
        }

        //收支管理提现
        public JsonResult GetTransforoutlist(string key, DateTime? beginTime, DateTime? endTime, int? index, int pageSize = 10)
        {
            int DataCount;
            TransferOutBLL bll = new TransferOutBLL();
            var list = bll.GetList(key, null, null, beginTime, endTime, pageSize, index == null ? 1 : index.Value, out DataCount);

            return Json(new { code = 1, pagecount = DataCount, list = list }, JsonRequestBehavior.AllowGet);//获取提现记录
        }

        #endregion

        #region 提现管理
        //提现管理
        public ActionResult TransforoutManager()
        {
            return PartialView();
        }
        /// <summary>
        /// 提现列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult TransforoutList(int? index, string key, int pageSize = 10)
        {
            int sum;
            TransferOutBLL bll = new TransferOutBLL();
            List<RelexBarBLL.Models.TransferOutModel> list = bll.GetList(key, null, null, null, pageSize, index == null ? 1 : index.Value, out sum, null);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        /// <summary>
        /// 更改提现状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [Filter.AdminPermission(Common.PermissionName.财务提现)]
        public JsonResult TransforoutUpdate(Guid ID, int status, string remark)
        {
            TransferOutBLL bll = new TransferOutBLL();
            enApplyStatus s = status == 1 ? enApplyStatus.Success : enApplyStatus.Faild;
            int i = bll.UpdateStatus(ID, s, remark, -1);
            if (i > 0)
            {
                return RJson("1", "已处理");
            }
            else
            {
                return RJson(i.ToString(), "处理失败");
            }
        }
        #endregion

        #region 管理员管理
        //管理员管理
        public ActionResult AdminManager()
        {
            return PartialView();
        }
        /// <summary>
        /// 获得管理员列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult AdminList(int? index, string key, int pageSize = 10)
        {
            int sum;
            AdminUserBLL bll = new AdminUserBLL();
            List<AdminUser> list = bll.GetAdminList(index == null ? 1 : index.Value, pageSize, key, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        /// <summary>
        /// 修改管理员状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Filter.AdminPermission(Common.PermissionName.管理员编辑)]
        public JsonResult UpdateAdminStatus(Guid ID, int status)
        {
            AdminUserBLL bll = new AdminUserBLL();
            AdminUser a = bll.GetAdminByID(ID);
            if (a == null)
            {
                return RJson("-1", "管理员不存在");
            }
            a.Status = status == 1 ? 1 : 0;
            int i = bll.UpdateAdminUser(a);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-2", "修改失败");
            }
        }
        /// <summary>
        /// 修改管理员密码
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [Filter.AdminPermission(Common.PermissionName.管理员编辑)]
        public JsonResult UpdateAdminPwd(Guid ID, string pwd)
        {
            AdminUserBLL bll = new AdminUserBLL();
            AdminUser a = bll.GetAdminByID(ID);
            if (a == null)
            {
                return RJson("-1", "管理员不存在");
            }
            if (string.IsNullOrEmpty(pwd))
            {
                return RJson("-3", "密码不能为空");
            }
            a.Psw = CommonClass.EncryptDecrypt.GetMd5Hash(pwd + SysConfigBLL.MD5Key);
            int i = bll.UpdateAdminUser(a);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-2", "修改失败");
            }
        }
        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Pwd"></param>
        /// <returns></returns>
        public JsonResult AddAdmin(string Name, string Pwd)
        {

            AdminUserBLL bll = new AdminUserBLL();
            if (bll.Exist(Name))
            {
                return RJson("-1", "用户名已被使用");
            }
            int i = bll.InsertAdminUser(Name, Pwd, 10);
            if (i > 0)
            {
                return RJson("1", "添加成功");
            }
            else
            {
                return RJson("1", "添加失败");
            }
        }
        #endregion
        #region 系统配置
        //系统配置
        public ActionResult SysConfig()
        {
            return PartialView();
        }
        /// <summary>
        /// 获取配置信息列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SysConfigList(int? index, string key, int pageSize = 10)
        {
            int sum;
            SysConfigBLL bll = new SysConfigBLL();
            List<SysConfig> list = bll.GetAllConfig(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        /// <summary>
        /// 修改配置状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Filter.AdminPermission(Common.PermissionName.系统编辑)]
        public JsonResult UpdateSysConfigStatus(int ID, int status)
        {
            SysConfigBLL bll = new SysConfigBLL();
            enStatus s = status == 1 ? enStatus.Enabled : enStatus.Unabled;
            int i = bll.UpdateStatus(ID, s);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-1", "修改失败");
            }
        }
        [Filter.AdminPermission(Common.PermissionName.系统新增)]
        public JsonResult AddSysConfig(string name, string value, string des)
        {
            SysConfigBLL bll = new SysConfigBLL();
            int i = bll.Insert(name, value, des, 1);
            if (i > 0)
            {
                return RJson("1", "新增成功");
            }
            return RJson("-1", "新增失败");
        }
        [Filter.AdminPermission(Common.PermissionName.系统编辑)]
        public JsonResult EditSysConfig(int id, string value, string des)
        {
            SysConfigBLL bll = new SysConfigBLL();
            int i = bll.Update(id, value.Replace(" ", "+").Replace("\n", ""), des, null);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            return RJson("-1", "修改失败");
        }
        #endregion

        #region 问题反馈
        //问题反馈
        public ActionResult ProblemManager()
        {
            return PartialView();
        }
        public ActionResult ProblemList(int? index, string key, int pageSize = 10)
        {
            int sum;
            ProblemBLL bll = new ProblemBLL();
            List<ProblemModel> list = bll.GetProblem(index == null ? 1 : index.Value, pageSize, key, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        #endregion

        #region 系统收益
        //系统收益
        public ActionResult SysPriceManager()
        {
            return PartialView();
        }
        public ActionResult SysPriceList(int? index, string key, DateTime? mindate, DateTime? maxdate, int pageSize = 10)
        {
            int sum;
            SysPriceBLL bll = new SysPriceBLL();
            List<SysPrice> list = bll.GetSysPrice(index == null ? 1 : index.Value, pageSize, key, mindate, maxdate, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        public JsonResult GetSysTotalPrice(DateTime? mindate, DateTime? maxdate)
        {
            SysPriceBLL bll = new SysPriceBLL();
            decimal[] d = bll.GetTotalPrice(mindate, maxdate);
            return RJson("1", d[0] + "||" + d[1]);
        }
        #endregion

        [Filter.NoFilter]
        public JsonResult DoLogin(string name, string pwd)
        {
            AdminUserBLL bll = new AdminUserBLL();
            AdminUser admin = bll.Login(name, pwd);
            if (admin == null)
            {
                return RJson("-1", "用户名或密码有误");
            }
            if (admin.Status == 0)
            {
                return RJson("-2", "该账户已被禁用");
            }
            Session["admin"] = admin;
            string token = MD5(admin.ID + admin.Psw);
            HttpCookie cookie = new HttpCookie("adminToken", admin.ID + "|" + token);
            Response.Cookies.Add(cookie);
            return RJson("1", "登陆成功");
        }
        public ActionResult LoginOut()
        {
            Session["admin"] = null;
            if (Response.Cookies["adminToken"] != null)
                Response.Cookies["adminToken"].Expires = DateTime.Now.AddDays(-1);
            return Redirect("Login");
        }

        #region 扫雷群管理
        public ActionResult GroupManager()
        {
            return PartialView();
        }
        public ActionResult GroupList(int? index, string key, int pageSize = 10)
        {
            int sum;

            ChatsBLL bll = new ChatsBLL();
            List<RelexBarBLL.Models.ChatGroupModel> list = bll.GetGroupList(key, index == null ? 1 : index.Value, pageSize, 2, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        public JsonResult AddGroup(string name)
        {
            ChatsBLL bll = new ChatsBLL();
            Guid gid;
            if (string.IsNullOrEmpty(name))
            {
                return RJson("-2", "参数不能为空");
            }
            int i = bll.CreateGroup(Guid.Empty, name, RelexBarBLL.EnumCommon.GType.GameGroup, out gid);
            if (i > 0)
            {
                return RJson("1", "创建成功");
            }
            else
            {
                return RJson("-1", "创建失败");
            }
        }
        #endregion

        #region APP版本号管理
        public ActionResult VersionManager()
        {
            return PartialView();
        }
        public ActionResult VersionList(int? index, int pageSize = 10)
        {
            int sum;
            VersionListBLL bll = new VersionListBLL();
            List<VersionList> list = bll.GetVersionList(index == null ? 1 : index.Value, pageSize, out sum);
            return PartialView(list);
        }
        public JsonResult EditVersion(Guid? id, string internalVersion, string externalVersion, string updateLog,string DownLink)
        {
            VersionListBLL bll = new VersionListBLL();
            if (id == null)
            {
                VersionList model = new RelexBarDLL.VersionList();
                model.ID = Guid.NewGuid();
                model.InternalVersion = internalVersion;
                model.ExternalVersion = externalVersion;
                model.Status = 1;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                model.UpdateLog = updateLog;
                model.DownLink = DownLink;
                int i = bll.Add(model);
                if (i > 0)
                {
                    return RJson("1", "添加成功");
                }
                else
                {
                    return RJson("-1", "添加失败");
                }
            }
            else
            {
                int i = bll.Update(id.Value, internalVersion, externalVersion, updateLog, DownLink);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                else
                {
                    return RJson("-1", "修改失败");
                }
            }

        }
        #endregion



        #region 广告管理


        public ActionResult AdsManager()
        {
            return PartialView();
        }
        public ActionResult AdsList(int? index, string key, int pageSize = 10)
        {
            RelexBarBLL.AdsListBLL bll = new RelexBarBLL.AdsListBLL();
            int sum;
            List<AdsList> list = bll.GetAdsSearch(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        public ActionResult EditAds(Guid? ID)
        {
            AdsList u;
            if (ID != null)
            {
                u = new AdsListBLL().GetAdsById(ID.Value);
            }
            else
            {
                u = new AdsList();
            }
            return View(u);
        }

        public JsonResult DoEditAds(Guid? ID, string Name, string Title, string LinkTo, string Img, string Descrition, DateTime? BeginTime, DateTime? EndTime)
        {

            AdsListBLL bll = new AdsListBLL();
            if (ID == null || ID == Guid.Empty)
            {
                int i = bll.Insert(Name, Title, LinkTo, Img, Descrition, BeginTime, EndTime);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败：" + (ErrorCode)i);
                }

            }
            else
            {
                AdsList Ad = null;
                AdsList u;
                if (Ad == null)
                {
                    u = bll.GetAdsById(ID.Value);
                }
                else
                {
                    u = Ad;
                }
                u.Name = Name;
                u.Title = Title;
                u.LinkTo = LinkTo;
                u.Img = Img;
                u.Descrition = Descrition;
                u.BeginTime = BeginTime;
                u.EndTime = EndTime;
                //  u.UpdateTime = DateTime.Now;
                int i = bll.UpdateAds(u);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }
        }

        /// <summary>
        /// 更改广告状态
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult CAdStatus(Guid UID, int status)
        {
            AdsListBLL bll = new AdsListBLL();
            // UsersBLL bll = new UsersBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeAdsStatus(UID, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }

        #endregion


        #region 红包管理


        //发布红包
        public ActionResult RedPacketManager()
        {
            ReportBLL report = new ReportBLL();

            ViewData["SingleRedPacket"] = report.GetRedpackByRedType(enRedType.Single).ToString();//用户送出福包总计

            ViewData["RewardSendRedPacket"] = report.GetRedpackByRedType(enRedType.Timeout).ToString();//会员升级福包总计

            ViewData["OnlineSystemSendRedPacket"] = report.GetRedpackByRedType(enRedType.System).ToString();//平台福包总计

            ViewData["SystemRedPacket"] = report.GetRedpackByRedType(enRedType.Auto_UserRecRed).ToString();//福包池总计


            return PartialView();
        }

        //public ActionResult RedPacketList(int? index, string key, int pageSize = 10)
        //{
        //    RelexBarBLL.RedPacksBLL bll = new RelexBarBLL.RedPacksBLL();
        //    int sum;
        //    List<RedPacket> list = bll.GetRedPacketSearch(key, pageSize, index == null ? 1 : index.Value, out sum);
        //    ViewData["sum"] = sum;
        //    return PartialView(list);
        //}

        public ActionResult RedPacketList(string key, enRedType type, string userType, DateTime? beginTime, DateTime? endTime, int index = 1, int pageSize = 10)
        {
            int sum = 0;
            RedPacksBLL bll = new RedPacksBLL();
            List<RelexBarBLL.Models.RedPacksModel> list = bll.GetRedPacketListByRedType(key, userType, type, beginTime, endTime, pageSize, index, out sum);
            @ViewData["sum"] = sum;
            return PartialView(list);
        }


        //抢红包
        public ActionResult RedPacketListGrabManager()
        {
            ReportBLL report = new ReportBLL();

            ViewData["yRedpackCount"] = report.GetRepackByStatus(enPacketStatus.Used, DateTime.Now.AddDays(-1));//昨日收福包数量
            ViewData["RedpackCount"] = report.GetRepackByStatus(enPacketStatus.Used, null);//收福包数量总计

            ViewData["yRedpackMoney"] = report.GetRepackListMoneyByStatus(enPacketStatus.Used, DateTime.Now.AddDays(-1)); //昨日收福包金额
            ViewData["RedpackMoney"] = report.GetRepackListMoneyByStatus(enPacketStatus.Used, null);//收福包金额总计

            return PartialView();
        }

        public ActionResult RedPacketGrabList(int? index, string key, string redType, string userType, DateTime? beginTime, DateTime? endTime, int pageSize = 10)
        {
            int sum;
            RedPacksBLL bll = new RedPacksBLL();
            List<RelexBarBLL.Models.RedPacksListModels> list = bll.GetRedPacketListSearch(key, userType, redType, beginTime, endTime, pageSize, index == null ? 1 : index.Value, out sum, null);
            ViewData["sum"] = sum;
            return PartialView(list);
        }


        public ActionResult EditRedPacket(Guid? ID)
        {
            RedPacket u;
            if (ID != null)
            {
                u = new RedPacksBLL().GetRedPacketById(ID.Value);
            }
            else
            {
                u = new RedPacket();
            }
            return View(u);
        }

        [Filter.AdminPermission(Common.PermissionName.红包新增)]
        public JsonResult DoEditRedPackList(Guid? RID, Guid? UID, string Title, string imglist, decimal SinglePrice, decimal TotalPrice)
        {
            RedPacksBLL bll = new RedPacksBLL();
            //  AdsListBLL bll = new AdsListBLL();
            if (RID == null || RID == Guid.Empty)
            {
                //int i = bll.InsertRedPacket(Title, imglist, SinglePrice,TotalPrice);

                //  var user= DBContext.AdminUser.Where(m => m.ID == UID).FirstOrDefault();

                //if (i > 0)
                //{
                //    return RJson("1", "新增成功");
                //}
                //else
                //{
                //    return RJson("-1", "新增失败：" + (ErrorCode)i);
                //}

                if (new RedPacksBLL().SendRedPacket(Guid.Empty, Common.enRedType.System, Title, imglist, "", null, null, SinglePrice, TotalPrice, null, null, null))
                    return RJson("1", "红包发送成功");
                else
                    return RJson("-2", "红包发送失败");
            }
            else
            {
                RedPacket Ad = null;
                RedPacket u;
                if (Ad == null)
                {
                    u = bll.GetRedPacketById(RID.Value);
                }
                else
                {
                    u = Ad;
                }
                u.title = Title;
                u.imglist = imglist;
                u.SinglePrice = SinglePrice;
                u.TotalPrice = TotalPrice;
                //  u.UpdateTime = DateTime.Now;
                int i = bll.UpdateRedPacket(u);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }
        }

        //更改红包状态
        public JsonResult CRedPacketStatus(Guid RID, int status)
        {
            RedPacksBLL bll = new RedPacksBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeRedPacketStatus(RID, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");

        }

        public JsonResult GetRedPacketList(Guid RID)
        {
            RedPacksBLL bll = new RedPacksBLL();
            List<RedPacketList> list = bll.GetRedPacketList(RID);
            return RJson("1", list.SerializeObject());
        }


        public JsonResult GetRankList(int type, int sv)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetRankList(type, sv);
            return Json(new { code = 1, list = list });//获取所有红包列表
        }

        #endregion


        #region 代理管理


        //public ActionResult UserManager()
        //{
        //    return PartialView();
        //}
        //public ActionResult UserList(int? index, string key, int pageSize = 10)
        //{
        //    RelexBarBLL.UsersBLL bll = new RelexBarBLL.UsersBLL();
        //    int sum;
        //    List<Users> list = bll.GetUsersSearch(key, pageSize, index == null ? 1 : index.Value, out sum);
        //    ViewData["sum"] = sum;
        //    return PartialView(list);
        //}


        public ActionResult UserAgentManager()
        {
            return PartialView();
        }
        public ActionResult UserAgentList(int? index, string key, int pageSize = 10)
        {
            RelexBarBLL.UsersBLL bll = new RelexBarBLL.UsersBLL();
            int sum;
            List<Users> list = bll.GetUsersAgentSearch(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }


        #endregion



        #region 

        public ActionResult PayInComeManager()
        {
            return PartialView();
        }

        public ActionResult PayInComeList(string phone, int index = 1, int pageSize = 10)
        {
            PayListBLL bll = new PayListBLL();
            int sum = 0;
            List<RelexBarBLL.Models.AdminPayListModel> list = bll.GetPayInComeSearch(phone, index, pageSize, out sum);
            @ViewData["sum"] = sum;
            return PartialView(list);
        }


        #endregion


        #region 佣金管理

        public ActionResult PayRewarsManager()
        {
            return PartialView();
        }

        public ActionResult PayRewarsList(string phone, int index = 1, int pageSize = 10)
        {
            PayListBLL bll = new PayListBLL();
            int sum = 0;
            List<RelexBarBLL.Models.AdminPayListModel> list = bll.GetPayRewarsSearch(phone, index, pageSize, out sum);
            @ViewData["sum"] = sum;
            return PartialView(list);
        }

        #endregion



        #region 订单管理


        public ActionResult OrderListManager()
        {
            return PartialView();
        }

        public ActionResult OrderList(string phone, enOrderStatus? Status, DateTime? beginTime, DateTime? endTime, int index = 1, int pageSize = 10)
        {
            OrdersBLL bll = new OrdersBLL();
            int sum = 0;
            List<RelexBarBLL.Models.OrderListModel> list = bll.GetOrderList(phone, Status, beginTime, endTime, index, pageSize, out sum);
            @ViewData["sum"] = sum;
            return PartialView(list);
        }
        public JsonResult GetPriceTotalOrder(DateTime? beginTime, DateTime? endTime)
        {
            OrdersBLL bll = new OrdersBLL();
            return Json(bll.GetPriceTotalOrder(beginTime, endTime));
        }

        [Filter.AdminPermission(Common.PermissionName.订单发货)]
        //修改支付后发货状态 1-2
        public JsonResult ChangeOrderStatus(Guid ID, int status)
        {

            OrdersBLL bll = new OrdersBLL();
            enOrderStatus s = enOrderStatus.Payed;
            if (status == 2)
            {
                s = enOrderStatus.Sended;
            }
            int i = bll.ChangeOrderStatus(ID, s);
            if (i > 0)
                return RJson("1", "发货成功");
            return RJson("-1", "发货失败");
        }

        #region  订单成交额


        public ActionResult OrderTurnoverManager()
        {
            //if (string.IsNullOrEmpty(FinalData.Date))
            //{
            lock (FinalData.syncobj)
            {
                string[] lable = new string[7];
                string[] OrderPayCount = new string[7];
                string[] OrderPriceCount = new string[7];
                string[] OrderPayPeopleCount = new string[7];
                string[] OrderPriceTotal = new string[7];
                ReportBLL report = new ReportBLL();
                for (int i = 1; i <= 7; i++)
                {
                    lable[i - 1] = "'" + DateTime.Now.AddDays(1 - i).ToString("MM-dd") + "'";
                    // OrderPayCount[i - 1] = report.GetOrderPayCount(DateTime.Now.AddDays(1 - i)).ToString(); //付款订单数
                    // OrderPriceCount[i - 1] = report.GetOrderPriceCount(DateTime.Now.AddDays(1 - i)).ToString();   //订单金额
                    OrderPriceCount[i - 1] = report.GetOrderPayPriceTotal(DateTime.Now.AddDays(1 - i)).ToString();
                    OrderPriceTotal[i - 1] = report.GetOrderPriceTotal(DateTime.Now.AddDays(1 - i)).ToString();  //下单金额

                }
                string newshopusercount = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.Shop).ToString();
                string newagentusercount = report.GetUserTypeByDate(DateTime.Now.AddDays(-1), enUserType.Agent).ToString();
                FinalData.Date = string.Join(",", lable);
                FinalData.OrderPayCount = string.Join(",", OrderPayCount);//付款订单数
                FinalData.OrderPriceCount = string.Join(",", OrderPriceCount); //订单金额
                FinalData.OrderPriceTotal = string.Join(",", OrderPriceTotal); //下单金额
                                                                               //  FinalData.OrderPayPeopleCount = string.Join(",", OrderPayPeopleCount);
                FinalData.NewUserCreateTime = DateTime.Now;
            }
            //   }

            ViewData["lable"] = FinalData.Date;
            ViewData["OrderPayCount"] = FinalData.OrderPayCount; //付款订单数
            ViewData["OrderPriceCount"] = FinalData.OrderPriceCount; //订单金额
            ViewData["OrderPriceTotal"] = FinalData.OrderPriceTotal; //下单金额

            return PartialView();
        }


        #endregion


        #endregion


        #region 储值管理

        public JsonResult GetRechargeTotal()
        {
            RechargeBLL bll = new RechargeBLL();
            return Json(bll.GetRechargeTotal());
        }

        #endregion




        #region 系统动态管理

        public ActionResult SysNoticeManager()
        {
            return PartialView();
        }
        /// <summary>
        /// 获取系统公告列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //public ActionResult SysNoticeList(int? index, string key, int pageSize = 10)
        //{
        //    int sum;
        //    UserMsgBLL bll = new UserMsgBLL();
        //    List<UserMsg> list = bll.GetAllListByType(key, pageSize, index == null ? 1 : index.Value, out sum);
        //    ViewData["sum"] = sum;
        //    return PartialView(list);
        //}
        public ActionResult SysNoticeList(int? index, string key, int pageSize = 10)
        {
            int sum;
            ChatMessageBLL bll = new ChatMessageBLL();
            List<ChatMessage> list = bll.GetAllList(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        //public ActionResult EditSysNotice(Guid? ID)
        //{
        //    UserMsg u;
        //    if (ID != null)
        //    {
        //        u = new UserMsgBLL().GetUserMsgById(ID.Value);
        //    }
        //    else
        //    {
        //        u = new UserMsg();
        //    }
        //    return View(u);
        //}

        [Filter.AdminPermission(Common.PermissionName.系统编辑)]
        public JsonResult DoEditSysNotice(Guid? ID, string Subject, string Content, string Title, string Img)
        {
            // AreaCode = AreaCode.Replace("+", "");
            UserMsgBLL bll = new UserMsgBLL();
            if (ID == null || ID == Guid.Empty)
            {
                int i = bll.Insert(Guid.Empty, Guid.Empty, Subject, Content, enMessageType.Other, Img, Title);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败：" + (ErrorCode)i);
                }

            }
            else
            {
                UserMsg msg = null;

                msg = bll.GetUserMsgById(ID.Value);

                if (msg == null)
                {
                    return RJson("-1", "公告不存在");
                }

                msg.Subject = Subject;
                msg.Content = Content;
                msg.Title = Title;
                msg.Img = Img;
                msg.UpdateTime = DateTime.Now;
                int i = bll.UpdateMsg(msg);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }
        }
        [Filter.AdminPermission(Common.PermissionName.系统编辑)]
        public JsonResult UpdateUserMsgShow(Guid ID, int status)
        {
            UserMsgBLL bll = new UserMsgBLL();
            enStatus s = status == 1 ? enStatus.Enabled : enStatus.Unabled;
            int i = bll.UpdateShow(ID, s);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-1", "修改失败");
            }
        }
        [Filter.AdminPermission(Common.PermissionName.系统编辑)]
        public JsonResult UpdateUserMsgStatus(Guid ID)
        {
            UserMsgBLL bll = new UserMsgBLL();
            //enStatus s = status == 1 ? enStatus.Enabled : enStatus.Unabled;
            int i = bll.UpdateStatus(ID);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-1", "修改失败");
            }
        }

        #region 系统公告2018-10-26 14:46:58
        public ActionResult EditSysNotice(Guid? ID)
        {

            ChatMessage C;
            if (ID != null)
            {
                C = new ChatMessageBLL().Get(ID.Value);
            }
            else
            {
                C = new ChatMessage();
            }
            return View(C);

        }

        [Filter.AdminPermission(Common.PermissionName.系统编辑)]
        public JsonResult DoEditChatSysNotice(Guid? MID, string Content)
        {

            ChatMessageBLL bll = new ChatMessageBLL();
            if (MID == null || MID == Guid.Empty)
            {
                int i = bll.Insert(Guid.Empty, Guid.Empty, Content);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败：" + (ErrorCode)i);
                }

            }
            else
            {
                ChatMessage msg = null;

                msg = bll.Get(MID.Value);

                if (msg == null)
                {
                    return RJson("-1", "公告不存在");
                }

                msg.Content = Content;
                msg.UpdateTime = DateTime.Now;
                int i = bll.UpdateMsg(msg);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }

        }



        [Filter.AdminPermission(Common.PermissionName.系统编辑)]
        public JsonResult UpdateChatUserMsgStatus(Guid MID, int status)
        {
            ChatMessageBLL bll = new ChatMessageBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.UpdateStatus(MID, (int)s);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-1", "修改失败");
            }
        }

        #endregion

        #endregion


        #region 用户通知管理

        public ActionResult UserMsgManager()
        {
            return PartialView();
        }


        public ActionResult UserMsgList(string key, int? index, int? type, int pageSize = 10)
        {
            int sum;
            UserMsgBLL bll = new UserMsgBLL();
            List<UserMsgModel> list = bll.GetUserMsgAllList(key, type, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        #endregion


        #region 联系我们


        public ActionResult ContactManager()
        {
            return PartialView();
        }
        public ActionResult ContactList(int? index, string key, int pageSize = 10)
        {
            ProblemBLL bll = new ProblemBLL();
            int sum;
            List<ContactUs> list = bll.GetContactList(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        #endregion



        #region 商品管理

        public ActionResult ProuductManager()
        {
            return PartialView();
        }

        public ActionResult ProuductList(int? categoryid, int? index, int? status, string key, int pageSize = 10)
        {
            ProductsBLL bll = new ProductsBLL();
            // RelexBarBLL.AdsListBLL bll = new RelexBarBLL.AdsListBLL();
            int sum;
            List<ProductListModel> list = bll.GetAllProductLists(categoryid, key, status, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        [ValidateInput(false)]
        public ActionResult EditProduct(Guid? ID)
        {
            ProductList u;
            if (ID != null)
            {
                u = new ProductsBLL().GetProduct(ID.Value);
            }
            else
            {
                u = new ProductList();
            }
            return View(u);
        }

        [Filter.AdminPermission(Common.PermissionName.商品编辑)]
        [ValidateInput(false)]
        public JsonResult DoEditProduct(Guid? ID, string Name, string Title, int CategoryID, string Img, string ImgList, string Descrition, decimal RealPrice, int PriceType, int Type
            , decimal Price, decimal FootQuanPrice, decimal Stock, int OrderID, DateTime? EndTime, DateTime? BeginTime, int CompleteCount, int GoodCount, int ViewCount, int CPoints, string Specification, string specList)
        {

            ProductsBLL bll = new ProductsBLL();
            if (ID == null || ID == Guid.Empty)
            {
                int i = bll.Insert(Guid.Empty, Name, Title, CategoryID, Img, ImgList, Descrition, RealPrice, PriceType, Price, FootQuanPrice, Stock, OrderID, Type, BeginTime, EndTime, CompleteCount, GoodCount, ViewCount, CPoints, Specification, specList);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败：" + (ErrorCode)i);
                }
            }
            else
            {
                ProductList prod = null;
                ProductList p;
                if (prod == null)
                {
                    p = bll.GetProduct(ID.Value);
                }
                else
                {
                    p = prod;
                }
                p.Name = Name;
                p.Title = Title;
                // p.Number = Number;
                p.CategoryID = CategoryID;
                p.Img = Img;
                p.ImgList = ImgList;
                p.Descrition = Descrition;
                p.RealPrice = RealPrice;
                p.PriceType = (int)PriceType;
                p.Price = Price;
                p.FootQuanPrice = FootQuanPrice;
                p.Type = (int)Type;
                p.Stock = Stock;
                p.OrderID = OrderID;
                p.BeginTime = BeginTime;
                p.EndTime = EndTime;
                p.UpdateTime = DateTime.Now;
                p.CompleteCount = CompleteCount;
                p.GoodCount = GoodCount;
                p.ViewCount = ViewCount;
                p.CPoints = CPoints;
                p.FootQuanPrice = FootQuanPrice;
                p.Specification = Specification;
                int i = bll.Update(p, specList);
                if (i > 0)
                {
                    // BaseBll.logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},新增商品成功,商品号{1},商品名称{2},", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Number, Name), enLogType.Admin);

                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }

        }

        [Filter.AdminPermission(Common.PermissionName.商品编辑)]
        public JsonResult CProductStatus(Guid PID, int status)
        {

            ProductsBLL bll = new ProductsBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.UpdateStatus(PID, (int)s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }

        [Filter.AdminPermission(Common.PermissionName.商品删除)]
        public JsonResult DeleteProduct(Guid PID)
        {
            ProductsBLL bll = new ProductsBLL();
            string err;
            int i = bll.DeleteProduct(PID, out err);
            if (i > 0)
                return RJson("1", err);
            return RJson("-1", err);
        }

        #endregion


        #region 商品分类

        public ActionResult CategoryManager()
        {
            return PartialView();
        }


        public ActionResult CategoryList(int? index, string key, int pageSize = 10)
        {

            CategoryBLL bll = new CategoryBLL();
            int sum;
            List<Category> list = bll.GetCategoryList(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        public JsonResult GetCategoryList()
        {

            CategoryBLL bll = new CategoryBLL();
            List<Category> list = bll.GetAllList();

            return RJson("1", list.SerializeObject());

        }

        [Filter.AdminPermission(Common.PermissionName.商品编辑)]
        public JsonResult CCategoryStatus(int ID, int status)
        {
            CategoryBLL bll = new CategoryBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.UpdateStatus(ID, (int)s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }

        public ActionResult EditCategory(int? ID)
        {
            Category u;
            if (ID != null)
            {
                u = new CategoryBLL().GetDetail(ID.Value);
            }
            else
            {
                u = new Category();
            }
            return View(u);
        }

        [Filter.AdminPermission(Common.PermissionName.商品编辑)]
        public JsonResult DoEditCategory(int? ID, string Name, string Title, string Keywords, int HeadID, int? OrderID)
        {

            CategoryBLL bll = new CategoryBLL();
            if (ID == 0)
            {
                int i = bll.Insert(HeadID, Name, Keywords, Title, OrderID);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败：" + (ErrorCode)i);
                }
            }
            else
            {

                int i = bll.Update((int)ID, Name, HeadID, OrderID, Title, Keywords);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }

        }

        public JsonResult GetFamily(string Family)
        {

            CategoryBLL bll = new CategoryBLL();
            var list = bll.CategoryTeam(Family);
            return Json(new { code = 1, list = list });
        }

        #endregion



        #region 用户帮助

        public ActionResult UserHelpManager()
        {
            return PartialView();
        }

        public ActionResult UserHelpList(int? index, string key, int pageSize = 10)
        {

            UserHelpBLL bll = new UserHelpBLL();
            int sum;
            List<UserHelp> list = bll.GetUserHelpSearch(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);

        }

        [ValidateInput(false)]
        public ActionResult EditUserHelp(Guid? ID)
        {
            UserHelp u;
            if (ID != null)
            {
                u = new UserHelpBLL().GetUserHelpById(ID.Value);
            }
            else
            {
                u = new UserHelp();
            }
            return View(u);
        }

        [Filter.AdminPermission(Common.PermissionName.帮助编辑)]
        [ValidateInput(false)]
        public JsonResult DoEditUserHelp(Guid? ID, int Type, string Title, string Img, string Content)
        {

            UserHelpBLL bll = new UserHelpBLL();
            if (ID == null || ID == Guid.Empty)
            {
                int i = bll.Insert(Title, Type, Img, Content);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败：" + (ErrorCode)i);
                }

            }
            else
            {
                UserHelp UH = null;
                UserHelp u;
                if (UH == null)
                {
                    u = bll.GetUserHelpById(ID.Value);
                }
                else
                {
                    u = UH;
                }
                u.Title = Title;
                u.Img = Img;
                u.Type = Type;
                u.Content = Content;
                u.UpdateTime = DateTime.Now;
                int i = bll.UpdateUserHelpById(u);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                return RJson("-2", "修改失败");
            }
        }

        [Filter.AdminPermission(Common.PermissionName.帮助编辑)]
        public JsonResult CUserHelpStatus(Guid ID, int status)
        {
            UserHelpBLL bll = new UserHelpBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeUserHelpStatus(ID, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }


        public JsonResult GetUserHelpList(Guid ID)
        {
            UserHelpBLL bll = new UserHelpBLL();
            List<UserHelp> list = bll.GetUserHelpList(ID);
            return RJson("1", list.SerializeObject());
        }


        #endregion


        #region 福音天地 2018-9-20 17:46:09

        #region 福音类型
        public ActionResult InfomationTypeManager()
        {
            return PartialView();
        }

        public ActionResult InfomationTypeList(int? index, string key, int pageSize = 10)
        {
            InfomationsBLL bll = new InfomationsBLL();
            int sum;
            List<InfomationType> list = bll.GetInfomationTypesSearch(key, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }
        public ActionResult EditInfomationType(int? ID)
        {
            InfomationType u;
            if (ID != null)
            {
                u = new InfomationsBLL().GetInfomationTypeById(ID.Value);
            }
            else
            {
                u = new InfomationType();
            }
            return View(u);
        }
        [Filter.AdminPermission(Common.PermissionName.福音编辑)]
        public JsonResult DoEditInfomationType(int ITID, string Name, string Desr)
        {

            InfomationsBLL bll = new InfomationsBLL();
            if (ITID == 0)
            {
                InfomationType model = new RelexBarDLL.InfomationType();
                // model.ID = Guid.NewGuid();
                model.Name = Name;
                model.Desr = Desr;
                model.Status = 1;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                int i = bll.Add(model);
                if (i > 0)
                {
                    return RJson("1", "添加成功");
                }
                else
                {
                    return RJson("-1", "添加失败");
                }
            }
            else
            {
                int i = bll.Update(ITID, Name, Desr);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                else
                {
                    return RJson("-1", "修改失败");
                }
            }

        }

        [Filter.AdminPermission(Common.PermissionName.福音编辑)]
        public JsonResult CInfomationTypeStatus(int Id, int status)
        {
            InfomationsBLL bll = new InfomationsBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeInfomationTypeStatus(Id, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }

        #endregion

        #region 福音管理
        public ActionResult InfomationsManager()
        {
            ReportBLL report = new ReportBLL();

            ViewData["InfomationsCount"] = report.GetInfomationsTotal();//发布广告总计

            ViewData["PointPaylist"] = report.GetPayListScoreByPayType(enPayType.Point, null).ToString();//福音总计

            ViewData["seePointCount"] = report.GetPayListInfomationsScoreByPayType(enPayType.Point).ToString(); //观看广告福音总计

            return PartialView();
        }


        public ActionResult InfomationsList(int? index, string key, DateTime? beginTime, DateTime? endTime, int pageSize = 10)
        {
            InfomationsBLL bll = new InfomationsBLL();
            int sum;
            List<RelexBarBLL.Models.InfomationsModel> list = bll.GetInfomationsSearch(key, beginTime, endTime, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        [Filter.AdminPermission(Common.PermissionName.福音编辑)]
        public JsonResult CInfomationStatus(Guid Id, int status)
        {
            InfomationsBLL bll = new InfomationsBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeInfomationStatus(Id, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }

        public ActionResult EditInfomations(Guid? ID)
        {
            Infomations u;
            if (ID != null)
            {
                u = new InfomationsBLL().GetInfomationById(ID.Value);
            }
            else
            {
                u = new Infomations();
            }
            return View(u);
        }

        [Filter.AdminPermission(Common.PermissionName.福音编辑)]
        public JsonResult DoEditInfomation(Guid? IID, string title, int Type, string imglist, string LinkTo, int GoodCount, int ViewCount, DateTime? BeginTime, DateTime? EndTime)
        {

            InfomationsBLL bll = new InfomationsBLL();
            if (IID == Guid.Empty)
            {
                Infomations model = new RelexBarDLL.Infomations();
                // model.ID = Guid.NewGuid();
                model.IID = Guid.NewGuid();
                model.UID = Guid.Empty;
                model.Type = Type;
                model.BeginTime = DateTime.Now;
                model.EndTime = DateTime.Now.AddDays(7);//7天后过期时间
                model.title = title;
                model.imglist = imglist;
                model.LinkTo = LinkTo;
                model.GoodCount = GoodCount;
                model.ViewCount = ViewCount;
                model.Status = (int)enStatus.Enabled;
                model.BeginTime = BeginTime;
                model.EndTime = EndTime;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                int i = bll.SystemPublish(model);
                if (i > 0)
                {
                    return RJson("1", "添加成功");
                }
                else
                {
                    return RJson("-1", "添加失败");
                }
            }
            else
            {
                int i = bll.UpdatePublish(IID, title, Type, imglist, LinkTo, GoodCount, ViewCount, BeginTime, EndTime);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                else
                {
                    return RJson("-1", "修改失败");
                }
            }

        }

        public JsonResult GetInfomationsByID(Guid ID)
        {
            InfomationsBLL bll = new InfomationsBLL();
            var list = bll.GetInfomationById(ID);
            return Json(new { code = 1, list = list });
            //  return RJson("1", list.SerializeObject());
        }

        #endregion


        public ActionResult InfomationsCommentManager(Guid id)
        {
            ViewData["IID"] = id;
            return PartialView();
        }
        /// <summary>
        /// 获取所有商圈评论
        /// </summary>
        /// <returns></returns>
        public JsonResult GetComment(Guid IID, int? index, int pageSize = 10)
        {
            InfomationsBLL bll = new InfomationsBLL();
            int DataCount;
            var list = bll.GetComments(IID, pageSize, index == null ? 1 : index.Value, out DataCount);
            ViewData["DataCount"] = DataCount;
            //return RJson("1", list.SerializeObject());
            return Json(new { code = 1, list = list });
        }

        public ActionResult InfomationsCommentList(Guid IID, int? index, string key, int pageSize = 10)
        {
            InfomationsBLL bll = new InfomationsBLL();
            int sum;
            List<InfomationsCommentModel> list = bll.GetCommentslist(IID, pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);
        }

        #endregion


        #region 积分管理 2018-9-30 

        public ActionResult ScoreManager()
        {

            ReportBLL report = new ReportBLL();

            ViewData["FuQuanPaylist"] = report.GetPayListScoreByPayType(enPayType.FuQuan, null).ToString();//福利总计

            ViewData["PointPaylist"] = report.GetPayListScoreByPayType(enPayType.Point, null).ToString();//福音总计

            ViewData["YFuQuanPaylist"] = report.GetPayListScoreByPayType(enPayType.FuQuan, DateTime.Now.AddDays(-1)).ToString();//昨日福利总计

            ViewData["YPointPaylist"] = report.GetPayListScoreByPayType(enPayType.Point, DateTime.Now.AddDays(-1)).ToString();//昨日福音总计

            return PartialView();
        }

        public ActionResult ScoreListManager()
        {
            return PartialView();
        }

        #endregion


        #region 管理员权限 2018-10-17

        public ActionResult AdminPermissionManager()
        {
            return PartialView();
        }

        public ActionResult AdminPermissionList(int? index, int pageSize = 10)
        {

            AdminPermissionBLL bll = new AdminPermissionBLL();
            int sum;
            List<AdminPermission> list = bll.GetAdminPermissionSearch(pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);

        }


        public ActionResult EditAdminPermission(Guid? ID)
        {

            AdminPermission Ad;
            if (ID != null)
            {
                Ad = new AdminPermissionBLL().GetAdminPermissionByID(ID.Value);
            }
            else
            {
                Ad = new AdminPermission();
            }
            return View(Ad);
        }

        [Filter.AdminPermission(Common.PermissionName.权限编辑)]
        public JsonResult DoEditAdminPermission(Guid? ID , Guid? AID, int RoleID,string PermissionID)
        {
            string err;
            AdminPermissionBLL bll = new AdminPermissionBLL();
            if (ID == null || ID == Guid.Empty)
            {
                int i = bll.Insert(AID, RoleID,PermissionID,out err);
                if (i > 0)
                {
                    return RJson("1", err);
                }
                else
                {
                    return RJson("-1", err);
                }

            }
            else
            {
                AdminPermission AD = null;
                AdminPermission u;
                if (AD == null)
                {
                    u = bll.GetAdminPermissionByID(ID.Value);
                }
                else
                {
                    u = AD;
                }
                u.AID = AID.Value;
                u.RoleID = RoleID;
                u.PermissionID = PermissionID;
                u.UpdateTime = DateTime.Now;
                int i = bll.UpdateAdminPermissionById(u,out err);
                if (i > 0)
                {
                    return RJson("1", err);
                }
                return RJson("-2", err);
            }
        }

        [Filter.AdminPermission(Common.PermissionName.权限编辑)]
        public JsonResult CAdminPermissiontatus(Guid ID, int status)
        {
            
            AdminPermissionBLL bll = new AdminPermissionBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeAdminPermissionStatus(ID, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }

        public ActionResult RoleManager()
        {
            return PartialView();
        }

        public ActionResult RoleList(int? index, int pageSize = 10)
        {
            RoleBLL bll = new RoleBLL();
            int sum;
            List<Role> list = bll.GetRoleSearch(pageSize, index == null ? 1 : index.Value, out sum);
            ViewData["sum"] = sum;
            return PartialView(list);

        }

        public ActionResult EditRole(int? ID)
        {

            Role r;
            if(ID!=null)
            {
                r = new RoleBLL().GetRoleByID(ID.Value);
            }
            else
            {
                r = new Role();
            }

            return View(r);
        }

        [Filter.AdminPermission(Common.PermissionName.权限编辑)]
        public JsonResult DoEditRole(int? ID, string Name)
        {          
            RoleBLL bll = new RoleBLL();
            if (ID == 0)
            {
                int i = bll.Insert(Name);
                if (i > 0)
                {
                    return RJson("1", "新增成功");
                }
                else
                {
                    return RJson("-1", "新增失败");
                }
            }
            else
            {
                Role r = null;
                Role ro;
                if (r == null)
                {
                    ro = bll.GetRoleByID(ID.Value);
                }
                else
                {
                    ro = r;
                }
                ro.Name = Name;
                ro.UpdateTime = DateTime.Now;
                int i = bll.UpdateRoleById(ro);
                if (i > 0)
                {
                    return RJson("1", "修改成功");
                }
                else
                {
                    return RJson("-2", "修改失败");
                }

            }

        }

        [Filter.AdminPermission(Common.PermissionName.权限编辑)]
        public JsonResult CRolestatus(int ID, int status)
        {

            RoleBLL bll = new RoleBLL();
            enStatus s = enStatus.Enabled;
            if (status == 0)
            {
                s = enStatus.Unabled;
            }
            int i = bll.ChangeRoleStatus(ID, s);
            if (i > 0)
                return RJson("1", "修改成功");
            return RJson("-1", "修改失败");
        }


        #endregion


        #region  修改密码

        public ActionResult  PwdManager()
        {
            return View(Session["admin"] as AdminUser);
        }

        [Filter.AdminPermission(Common.PermissionName.管理员编辑)]
        public JsonResult UpdateNewAdminPwd(Guid ID, string OldPwd, string NewPwd)
        {
            AdminUserBLL bll = new AdminUserBLL();
            AdminUser a = bll.GetAdminByID(ID);
            if (a == null)
            {
                return RJson("-1", "管理员不存在");
            }
            if (string.IsNullOrEmpty(NewPwd))
            {
                return RJson("-3", "密码不能为空");
            }
            if (a.Psw != CommonClass.EncryptDecrypt.GetMd5Hash(OldPwd + SysConfigBLL.MD5Key))
            {
                return RJson("-5", "旧密码不正确");
            }
            if (a.Psw == CommonClass.EncryptDecrypt.GetMd5Hash(NewPwd + SysConfigBLL.MD5Key))
            {
                return RJson("-4", "新密码与旧密码相同");
            }
            
            a.Psw = CommonClass.EncryptDecrypt.GetMd5Hash(NewPwd + SysConfigBLL.MD5Key);
            int i = bll.UpdateAdminUser(a);
            if (i > 0)
            {
                return RJson("1", "修改成功");
            }
            else
            {
                return RJson("-2", "修改失败");
            }
        }
        #endregion

    }
}