using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class BillController : BaseController
    {
        // GET: Bill
        public ActionResult MyBill()
        {
            return View();
        }

        /// <summary>
        /// 账单列表
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public JsonResult List(int type, DateTime? begin, DateTime? end)
        {
            Common.enPayFrom? from = Common.enPayFrom.RedPaged;
            Common.enPayInOutType? inout = Common.enPayInOutType.In;
            Common.enPayType? paytype = Common.enPayType.Coin;
            switch (type)
            {
                case 1: from = Common.enPayFrom.Commission; break;
                case 2: from = Common.enPayFrom.Reward; break;
                case 3: from = Common.enPayFrom.RedPaged; inout = Common.enPayInOutType.Out; break;
                case 4: from = Common.enPayFrom.RedPaged; break;
                case 5: from = null; break;
                case 6: from = null; begin = DateTime.Now.Date; break;
                case 7: from = null; paytype = Common.enPayType.Point; inout = null; break;
                case 8: from = null; paytype = Common.enPayType.FuQuan; inout = null; break;
                case 9: from = null; paytype = Common.enPayType.Coin; inout = null; break;
                case 10: from = Common.enPayFrom.Transfor; paytype = Common.enPayType.Coin; inout = null; break;
                case 11: from = Common.enPayFrom.Exchange; paytype = Common.enPayType.Coin; inout = null; break;
            }
            decimal inVal = 0, outVal = 0, totalVal = 0;
            PayListBLL pbll = new PayListBLL();
            //LogsBLL.InsertAPILog("BillController/list", UserInfo.ID, "pageindex=" + PageIndex + "&type=" + type);
            var list = pbll.GetPayList(UserInfo.ID, begin, end, from, inout, paytype, PageSize, PageIndex, out DataCount, ref inVal, ref outVal, ref totalVal);
            return Json(new { code = 1, pagecount = TotalPageCount, inVal = inVal, outVal = outVal, totalVal = totalVal, list = list });//获取所有红包列表
        }

        /// <summary>
        /// 获取基本钱包信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInfo()
        {
            decimal Commission, Rewards, SendReds, RecReds, FubaoShare = 0, BigFubaoShare = 0;
            PayListBLL pbll = new PayListBLL();
            Commission = pbll.TotalPays(UserInfo.ID, null, null, Common.enPayFrom.Commission, Common.enPayInOutType.In, null);
         //   Rewards = pbll.TotalPays(UserInfo.ID, null, null, Common.enPayFrom.Reward, Common.enPayInOutType.In, null);
            Rewards = pbll.TotalPays(UserInfo.ID, null, null, Common.enPayFrom.Reward, Common.enPayInOutType.In, Common.enPayType.Coin);
            SendReds = pbll.TotalPays(UserInfo.ID, null, null, Common.enPayFrom.RedPaged, Common.enPayInOutType.Out, Common.enPayType.Coin);
            RecReds = pbll.TotalPays(UserInfo.ID, null, null, Common.enPayFrom.RedPaged, Common.enPayInOutType.In, Common.enPayType.Coin);

            RedPacksBLL rbll = new RedPacksBLL();
            pbll.GetFubaoShare(out FubaoShare, out BigFubaoShare);
            var ubll = new UsersBLL();
            UserInfo = ubll.GetUserById(UserInfo.ID);

            return Json(new
            {
                code = 1,
                Balance = UserInfo.Balance,
                Score = UserInfo.Score,
                FootQuan = UserInfo.FootQuan,
                Commission = Commission,
                Rewards = Rewards,
                SendReds = SendReds,
                RecReds = RecReds,
                FubaoShare = FubaoShare,
                BigFubaoShare = BigFubaoShare * 0.02M,
                RedBalance = UserInfo.RedBalance,
            });
        }

        /// <summary>
        /// 账单详情
        /// </summary>
        /// <returns></returns>
        public JsonResult Detail(int ID)
        {
            PayListBLL pbll = new PayListBLL();
            var result = pbll.Detail(ID);
            return Json(new
            {
                code = 1,
                model = result,
            });
        }
    }
}