using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;
using RelexBarBLL.Services;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class OrdersController : BaseController
    {
        /// <summary>
        /// 下单并付款
        /// </summary>
        /// <returns></returns>
        public JsonResult OrderAndPay(Guid? ShopID, Guid addressID, Common.enPayment? Payment, Common.enPayType? PriceType,
            decimal? Fee, string Remark, Guid ProID, int count, string pwd, Guid? productSpecification)
        {
            if (!Payment.HasValue || Payment == (int)Common.enPayment.LOCAL) //本地余额支付
            {
                if (string.IsNullOrEmpty(UserInfo.PayPsw))
                {
                    return RJson("-112", Common.ErrorCode.密码尚未设置.ToString());
                }
                if (!CheckPayPSW(pwd))
                {
                    return RJson("-113", Common.ErrorCode.支付密码不正确.ToString());
                }
            }
            OrdersBLL bll = new OrdersBLL();
            int code;
            var order = bll.Insert(ShopID, UserInfo.ID, addressID, Payment, PriceType, Fee.HasValue ? Fee.Value : 0, Common.enOrderType.OnLine,
                Remark, ProID, count, productSpecification, out code);
            if (order == null)
            {
                return RJson(code.ToString(), ((Common.ErrorCode)code).ToString());
            }
            return PayOrder(order.ID, pwd, Payment);
        }

        /// <summary>
        /// 下单
        /// </summary>
        /// <returns></returns>
        public JsonResult AddOrder(Guid? ShopID, Guid addressID, Common.enPayment? Payment, Common.enPayType? PriceType,
            decimal? Fee, string Remark, Guid ProID, int count, Guid? productSpecification)
        {
            int code;
            OrdersBLL bll = new OrdersBLL();
            var order = bll.Insert(ShopID, UserInfo.ID, addressID, Payment, PriceType, Fee.HasValue ? Fee.Value : 0,
                Common.enOrderType.OnLine, Remark, ProID, count, productSpecification, out code);
            if (order == null)
            {
                return RJson(code.ToString(), ((Common.ErrorCode)code).ToString());
            }
            return RJson("1", "下单成功");
        }

        /// <summary>
        /// 付款
        /// </summary>
        /// <returns></returns>
        public JsonResult PayOrder(Guid OID, string pwd, Common.enPayment? payment)
        {
            OrdersBLL bll = new OrdersBLL();
            var o = bll.GetDetail(OID);
            if (o == null)
            {
                return RJson("-90009", Common.ErrorCode.订单不存在.ToString());
            }
            if (o.UID != UserInfo.ID)
            {
                return RJson("-90012", Common.ErrorCode.您不能操作他人订单.ToString());
            }
            if (o.Status != (int)Common.enOrderStatus.Order)
            {
                return RJson(((int)Common.ErrorCode.订单异常).ToString(), Common.ErrorCode.订单异常.ToString());
            }

            var fqjf = o.proFootQuanPrice.Value * o.Count.Value;
            if (fqjf > 0)
            {
                if (UserInfo.FootQuan < fqjf)//福券积分够不够？
                {
                    return RJson(((int)Common.ErrorCode.账户福券不足).ToString(), Common.ErrorCode.账户福券不足.ToString());
                }
            }

            if (!payment.HasValue || payment.Value == Common.enPayment.LOCAL)//余额支付
            {
                if (string.IsNullOrEmpty(UserInfo.PayPsw))
                {
                    return RJson("-112", Common.ErrorCode.密码尚未设置.ToString());
                }
                if (!CheckPayPSW(pwd))
                {
                    return RJson("-113", Common.ErrorCode.支付密码不正确.ToString());
                }

                bll.UpdatePayment(OID, Common.enPayment.LOCAL);//更新支付类型为本地支付
                var ecode = bll.UpdateStatus(OID, Common.enOrderStatus.Payed);
                if (ecode > 0)
                {
                    return RJson("1", "订单支付成功");
                }
                else
                {
                    return RJson(ecode.ToString(), ((Common.ErrorCode)ecode).ToString());
                }
            }
            else
            {
                //微信APP支付
                ThirdServices th = new ThirdServices();
                var log = th.PayOrders("Order/PayOrder", Common.enPayment.WX, o.Price, o.Number, "", UserInfo.ID, Guid.Empty, Common.enPayFrom.OnLinePay, "");
                if (log != null)
                {
                    if (payment.Value == Common.enPayment.WX)//微信支付
                    {
                        var wxAPI = new WX_Services.WxPayApi();
                        var paydata = new WX_Services.WxPayApi().GetUnifiedOrderResult((int)(log.PayPrice * 100), "福包多多", "", log.PayNumber, WX_Services.euTrade_type.APP);
                        string WS_prepay_id = paydata.GetValue("prepay_id").ToString();
                        if (string.IsNullOrEmpty(WS_prepay_id))//生成订单失败
                        {
                            return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        }
                        string data = wxAPI.GetPrePayDataByAPP(WS_prepay_id);//预付订单号
                        //return RJson("1", data);//生成成功
                        return Json(new { code = "1", num = log.PayNumber, msg = data });//生成成功
                    }
                }
                return RJson("-1", "生成订单失败");//生成支付失败
            }

            return RJson("-1", "支付失败");//支付失败
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult Detail(Guid OID)
        {
            OrdersBLL bll = new OrdersBLL();
            var o = bll.GetDetail(OID);
            if (o == null)
            {
                return RJson("-90009", Common.ErrorCode.订单不存在.ToString());
            }
            if (o.UID != UserInfo.ID)
            {
                return RJson("-90012", Common.ErrorCode.您不能操作他人订单.ToString());
            }
            // var Balance = UserInfo.Balance; //用户余额
            return Json(new { code = 1, model = o });//订单详情
        }

        /// <summary>
        /// 个人订单列表
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public JsonResult List(Common.enOrderStatus? status)
        {
            OrdersBLL bll = new OrdersBLL();
            var list = bll.GetOrderList(UserInfo.ID, "", status, null, null, null, null, null, null, PageSize, PageIndex, out DataCount);

            return Json(new { code = 1, pagecount = TotalPageCount, list = list });//订单列表
        }

        /// <summary>
        /// 更改订单状态，已收货，已完成
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateStatus(Guid OID, int status)
        {
            OrdersBLL bll = new OrdersBLL();
            var o = bll.GetDetail(OID);
            if (o == null)
            {
                return RJson("-90009", Common.ErrorCode.订单不存在.ToString());
            }
            if (o.UID != UserInfo.ID)
            {
                return RJson("-90012", Common.ErrorCode.您不能操作他人订单.ToString());
            }
            if (status != (int)Common.enOrderStatus.Cancel)
            {
                if (o.Status < (int)Common.enOrderStatus.Recieved)
                {
                    return RJson("-90004", Common.ErrorCode.数据状态异常.ToString());
                }
            }
            var ecode = bll.UpdateStatus(OID, (Common.enOrderStatus)status);
            if (ecode > 0)
            {
                return RJson("1", "订单状态更改成功");
            }
            else
            {
                return RJson(ecode.ToString(), ((Common.ErrorCode)ecode).ToString());
            }
        }

    }
}