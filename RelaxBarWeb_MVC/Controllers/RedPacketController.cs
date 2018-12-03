using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;
using RelexBarBLL.WeChat;
using RelexBarBLL.WeChat.impl;
using RelexBarBLL.Services;
using RelexBarDLL;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class RedPacketController : BaseController
    {
        /// <summary>
        /// 获取可领红包列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPacketList()
        {
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetPacketList(UserInfo.ID);

            return Json(new { code = 1, list = list });//获取所有红包列表
        }

        /// <summary>
        /// 获取已发出的红包列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSendPacketList(Guid? ID)
        {
            RedPacksBLL bll = new RedPacksBLL();
            if (!ID.HasValue)
            {
                ID = UserInfo.ID;
            }
            var list = bll.GetSendPacketList(ID.Value, PageSize, PageIndex, out DataCount);
            PayListBLL pbll = new PayListBLL();
            decimal SendReds, RecReds;
            SendReds = pbll.TotalPays(ID.Value, null, null, Common.enPayFrom.RedPaged, Common.enPayInOutType.Out, null);
            RecReds = pbll.TotalPays(ID.Value, null, null, Common.enPayFrom.RedPaged, Common.enPayInOutType.In, null);

            return Json(new { code = 1, pagecount = TotalPageCount, RecReds = RecReds, SendReds = SendReds, list = list });//获取所有红包列表
        }

        /// <summary>
        /// 获取已领取红包列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRecPacketList()
        {
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetRecPacketList(UserInfo.ID, PageSize, PageIndex, out DataCount);

            return Json(new { code = 1, pagecount = TotalPageCount, list = list });//获取所有红包列表
        }

        /// <summary>
        /// 领取红包
        /// </summary>
        /// <returns></returns>
        public JsonResult ClickRedPacket(Guid RID, string number)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var result = bll.ClickRedPacket(UserInfo.ID, RID, number, UserInfo.UserType);

            return RJson(result.ToString(), result > 0 ? "" : ((Common.ErrorCode)result).ToString());//获取所有红包列表
        }

        /// <summary>
        /// 获取红包详情
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPacketDetail(Guid RID)
        {
            RedPacksBLL bll = new RedPacksBLL();
            //  Rep model = bll.GetPacketDetail(RID);
            RedPacket model = bll.GetPacketDetail(RID);
            UsersBLL ubll = new UsersBLL();
            var type = ubll.GetUserById(model.UID).UserType;

            return Json(new { code = 1, model = model, type = type });//获取所有红包列表
        }

        /// <summary>
        /// 获取红包详情
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRecPacketDetail(Guid RID, string Number)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var model = bll.GetPacketDetail(UserInfo.ID, RID, Number);
            var hadRecs = bll.GetRecPacketList(RID, Number, PageSize, PageIndex, out DataCount);

            return Json(new { code = 1, model = model, reccount = DataCount, pagecount = TotalPageCount, reclist = hadRecs });//获取所有红包列表
        }

        /// <summary>
        /// 点赞红包
        /// </summary>
        /// <returns></returns>
        public JsonResult GoodForRedPacket(Guid RID)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var result = bll.GoodForRedPacket(UserInfo.ID, RID);
            if (result > 0)
            {
                return Json(new { code = 1, msg = "成功" });
            }
            else
            {
                return Json(new { code = result, msg = ((Common.ErrorCode)result).ToString() });
            }
        }

        /// <summary>
        /// 发红包
        /// </summary>
        /// <param name="redmoney"></param>
        /// <param name="Payment"></param>
        /// <param name="title"></param>
        /// <param name="img"></param>
        /// <param name="linkto"></param>
        /// <param name="areacode"></param>
        /// <param name="arealimit"></param>
        /// <param name="ctype">发红包要获取福券还是积分</param>
        /// <returns></returns>
        public JsonResult SendRedPacket(decimal redmoney, int Payment, string title, string img, string linkto, string areacode, string arealimit, int? ctype, int? sex, string pwd)
        {
            //调用微信支付、支付宝支付
            decimal single = 0;
            if (redmoney <= 10)//10块钱5个红包
            {
                single = redmoney / 10;
            }
            else if (redmoney <= 100 && redmoney > 10)//100块钱50个红包
            {
                single = redmoney / 100;
            }
            else if (redmoney <= 1000 && redmoney > 100)//1000块钱500个红包
            {
                single = redmoney / 1000;
            }
            if (Payment == (int)Common.enPayment.LOCAL)//余额支付
            {
                if (string.IsNullOrEmpty(UserInfo.PayPsw))
                {
                    return RJson("-112", Common.ErrorCode.密码尚未设置.ToString());////余额支付才需要密码
                }
                if (!CheckPayPSW(pwd))
                {
                    return RJson("-113", Common.ErrorCode.支付密码错误.ToString());
                }
                var ui = new UsersBLL().GetUserById(UserInfo.ID);
                if (ui != null && ui.Balance < redmoney)//余额超过红包金额
                {
                    return Json(new { code = Common.ErrorCode.账户余额不足, msg = "用户余额不足" });
                }

                if (new RedPacksBLL().SendRedPacket(ui.ID, Common.enRedType.Single, title, img, linkto, null, null, single, redmoney, areacode, arealimit, sex, ctype.HasValue ? ctype.Value : 1))
                    return Json(new { code = 1, msg = "发送福包成功" });
                else
                    return Json(new { code = -1, msg = "福包发送失败" });
            }
            else
            {
                ThirdServices th = new ThirdServices();
                //   var money = nt == Common.enUserType.Agent ? SysConfigBLL.AgentPrice : SysConfigBLL.ShopPrice;
                var ordernum = Common.GetOrderNumer();
                var log = th.PayOrders("RedPacket/SendRedPacket", (Common.enPayment)Payment, redmoney, ordernum, ((int)(Common.enRedType.Single)).ToString() + "|" + title.Replace("|", "&") + "|" + img.Replace("|", "&") + "|" + linkto.Replace("|", "&") + "|" + single + "|" + areacode.Replace("|", "&") + "|" + arealimit.Replace("|", "&") + "|" + sex + "|" + (ctype.HasValue ? ctype.Value : 1), UserInfo.ID, Guid.Empty, Common.enPayFrom.RedPaged, "");
                if (log != null)
                {
                    if (Payment == (int)Common.enPayment.WX) //微信支付
                    {
                        var wxAPI = new WX_Services.WxPayApi();
                        //先统一下单
                        //var paydata = new WX_Services.WxPayApi().GetUnifiedOrderResult((int)(log.PayPrice * 100), "福包多多", "", log.PayNumber, WX_Services.euTrade_type.MWEB);
                        //if (paydata == null)
                        //{
                        //    return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        //}
                        //string WS_prepay_id = paydata.GetValue("prepay_id").ToString();
                        //if (string.IsNullOrEmpty(WS_prepay_id))//生成订单失败
                        //{
                        //    return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        //}
                        //string result_code = paydata.GetValue("result_code").ToString();
                        //if (result_code != "SUCCESS")
                        //{
                        //    return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        //}
                        //return Json(new { code = "1", num = log.PayNumber, msg = wxAPI.GetPrePayDataByH5("pay/RedictH5Pay?url=" + Server.UrlEncode(paydata.GetValue("mweb_url").ToString())) });//生成成功

                        var paydata = new WX_Services.WxPayApi().GetUnifiedOrderResult((int)(log.PayPrice * 100), "福包多多", "", log.PayNumber, WX_Services.euTrade_type.APP);
                        string WS_prepay_id = paydata.GetValue("prepay_id").ToString();
                        if (string.IsNullOrEmpty(WS_prepay_id))//生成订单失败
                        {
                            return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        }
                        string data = wxAPI.GetPrePayDataByAPP(WS_prepay_id);//预付订单号
                        //Response.Write(data);
                        return Json(new { code = "1", num = log.PayNumber, msg = data });//生成成功
                    }
                    else if (Payment == (int)Common.enPayment.ALI)//是否支付宝？
                    {
                        AliPay paybll = new AliPay();
                        string paydata = paybll.CreatePayDatas(log.PayPrice, "福包多多", log.PayNumber, AliPay.euTrade_type.QUICK_MSECURITY_PAY);

                        return Json(new { code = "1", num = log.PayNumber, msg = paydata });//生成成功
                    }
                    else
                    {
                        return RJson("-1", "支付类型不正确");
                    }
                }
                return RJson("-1", "生成订单失败");//生成支付失败
                                             //string url = "";
                                             //  return Json(new { code = 1, msg = url });//跳转到微信支付}
            }
            return RJson("-1", "支付类型不正确");
        }

        /// <summary>
        /// 评论红包
        /// </summary>
        /// <returns></returns>
        public JsonResult Comment(Guid RID, string Number, string content)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var result = bll.Comment(RID, Number, UserInfo.ID, content);
            return Json(new { code = result, msg = "评论成功" });
        }

        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <returns></returns>
        public JsonResult GetComment(Guid RID, string Number)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetComments(RID, Number, PageSize, PageIndex, out DataCount);

            return Json(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        /// <summary>
        /// 获取发红包的可选金额选项
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRedPrice()
        {
            var pri = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            return Json(new { code = 1, list = pri });
        }

        /// <summary>
        /// 获取红包排行榜
        /// </summary>
        /// <param name="type">排行榜类型     1：今日排行榜，2：当月排行榜，3：总排行榜</param>
        /// <param name="sv">发/抢红包类型   1：发红包，2：抢红包</param>
        /// <returns></returns>
        public JsonResult GetRankList(int type, int sv)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetRankList(type, sv, 100);
            return Json(new { code = 1, list = list });//获取所有红包列表
        }

        /// <summary>
        /// 首页广告领主
        /// </summary>
        /// <param name="type">排行榜类型</param>
        /// <returns></returns>
        public JsonResult GetMainAD(int type)
        {
            return Json(new { code = 1, list = "" });//获取所有红包列表
        }

        /// <summary>
        /// 获取最新抢到红包的人
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNewsRecRedPack()
        {
            RedPacksBLL bll = new RedPacksBLL();
            var list = bll.GetNewsRecRedPack();
            return Json(new { code = 1, list = list });//获取所有红包列表
        }

        /// <summary>
        /// 今日已领红包个数及分享朋友数量
        /// </summary>
        /// <param name="type">排行榜类型</param>
        /// <returns></returns>
        public JsonResult GetReciveRedPackCount()
        {
            RedPacksBLL bll = new RedPacksBLL();
            int maxcount = 0;
            var count = bll.GetUserReciveCount(UserInfo.ID, DateTime.Now, out maxcount);
            return Json(new { code = 1, msg = "", count = count, maxcount = maxcount });//获取所有红包列表
        }

        /// <summary>
        /// 获取最新一条升级自动发红包的记录
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNewRedPacketByUID()
        {
            RedPacksBLL bll = new RedPacksBLL();
            return Json(new { code = 1, list = bll.GetRedPacketNewByUID(UserInfo.ID) });
        }

        //修改一条升级自动发红包的记录
        public JsonResult UpdateRedPacketByRID(Guid RID, string title, string img)
        {
            RedPacksBLL bll = new RedPacksBLL();
            var result = bll.EditRedPacketNewByID(RID, title, img);
            if (result > 0)
                return RJson("1", "修改成功");
            else
                return RJson(result.ToString(), "修改失败：" + ((Common.ErrorCode)result).ToString());
        }
    }
}