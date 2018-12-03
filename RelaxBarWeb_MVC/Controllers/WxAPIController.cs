using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;
using RelexBarBLL.WeChat;
using RelexBarBLL.WeChat.impl;
using RelexBarDLL;
using System.Text;
using System.Xml;
using static RelexBarBLL.Common;

namespace RelaxBarWeb_MVC.Controllers
{
    public class WxAPIController : BaseController
    {
        // GET: WxAPI
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult WxPay(string number)
        {
            WeChatAPI wxapi = new WxMiniProgram();
            decimal sum = 0;
            //switch (type)
            //{
            //    case 1:
            //        RedPacksBLL bll = new RedPacksBLL();
            //        List<RedPacket> list = bll.GetRedPacketByNumber(user.ID, number, enPacketStatus.NoActive);
            //        sum = bll.GetRedPacketTotalPrice(Guid.Parse(number));
            //        break;
            //    case 2:
                   OrdersBLL bll2 = new OrdersBLL();
            //        List<OrderList> list2 = bll2.GetByNumber(user.ID, number, RelexBarBLL.Common.enOrderStatus.Order);
                     sum = bll2.GetPayPrice(number);
            //        break;
            //}
            
            try
            {
              
                if (sum < 0)
                {
                    return JsonPro(0); //数据有误
                }
                WxPayData req = new WxPayData();
                req.SetValue("out_trade_no", number);
                req.SetValue("body", "福包多微信支付");
                req.SetValue("total_fee", Convert.ToInt32((sum * 100)));
                req.SetValue("trade_type", "CNY");
                req.SetValue("notify_url", string.Format("{0}/WxAPI/WxPayNotify", SysConfigBLL.DOMAIN.IndexOf("https") != -1 ? SysConfigBLL.DOMAIN.Replace("https", "http") : SysConfigBLL.DOMAIN));
                req.SetValue("trade_type", "MWEB");
                req.SetValue("spbill_create_ip", Request.UserHostAddress);
                // req.SetValue("attach", user.ID.ToString());//额外参数传入用户id
                req.SetValue("scene_info", "{\"h5_info\": {\"type\":\"Wap\",\"wap_url\": \"http://www.fbddd.com\",\"wap_name\": \"福包多微信支付\"}}");
             
                WxPayData wxPayData = wxapi.Unifiedorder(RelexBarBLL.Services.WX_Services.WxPayData.wxAppid, RelexBarBLL.Services.WX_Services.WxPayData.wxMCHID, RelexBarBLL.Services.WX_Services.WxPayData.wxKey, Request.UserHostAddress, req);
              //  log.Debug(wxPayData.ToJson());
                if (wxPayData.GetValue("return_code").ToString() == "SUCCESS")
                {
                    string ts = RelexBarBLL.WeChat.Utils.GenerateTimeStamp();
                    string ns = RelexBarBLL.WeChat.Utils.GenerateNonceStr();
                    string pid = wxPayData.GetValue("prepay_id").ToString();
                    string mweb_url = wxPayData.GetValue("mweb_url").ToString();//mweb_url为拉起微信支付收银台的中间页面，可通过访问该url来拉起微信客户端，完成支付,mweb_url的有效期为5分钟。
                    WxPayData d = new WxPayData();
                    d.SetValue("appId", RelexBarBLL.Services.WX_Services.WxPayData.wxAppid);
                    d.SetValue("nonceStr", ns);
                    d.SetValue("package", string.Format("prepay_id={0}", pid));
                    d.SetValue("signType", "MD5");
                    d.SetValue("timeStamp", ts);

                    return JsonPro(new
                    {
                        codeMsg = string.Format(wxPayData.GetValue("mweb_url") + "&redirect_url=http%3A%2F%2Fwww.fbddd.com/WxAPI/ResultUrl?no=" + number),
                        // codeMsg = wxPayData.GetValue("mweb_url"),
                        timeStamp = ts,
                        nonceStr = ns,
                        package = string.Format("prepay_id={0}", pid),
                        signType = "MD5",
                //        paySign = d.MakeSign(shopWxInfo.MCHKEY)
                    }, "1");
                }
                else
                {
                    return JsonPro(wxPayData.GetValue("return_msg"), "-1");
                }
            }
            catch (Exception e)
            {
              //  log.Error(string.Format("/WxAPI/WxPay异常：{0}", e.ToString()));
                return JsonPro("-1", e.Message);
            }
        }

        //微信支付异步通知
        public void WxPayNotify()
        {
            try
            {
               // OrdersBLL bll = new OrdersBLL();
                ThirdServices bll = new ThirdServices();
                SysConfig SysConfig;
                WxPayData notifyData = GetNotifyData(out SysConfig);
              //  log.Info(string.Format("接口：/WxAPI/WxPayNotify 微信小程序异步通知结果：{0}", notifyData.ToJson()));
                //检查支付结果中transaction_id是否存在
                if (!notifyData.IsSet("transaction_id"))
                {
                    //若transaction_id不存在，则立即返回结果给微信支付后台
                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "支付结果中微信订单号不存在");
                    Log.Error(this.GetType().ToString(), "微信小程序支付结果The Pay result is error : " + res.ToXml());
                    Response.Write(res.ToXml());
                    Response.End();
                }

                string transaction_id = notifyData.GetValue("transaction_id").ToString();//微信支付订单号

                //查询订单，判断订单真实性
                if (!QueryOrder(transaction_id, RelexBarBLL.Services.WX_Services.WxPayData.wxAppid, RelexBarBLL.Services.WX_Services.WxPayData.wxMCHID, RelexBarBLL.Services.WX_Services.WxPayData.wxKey))
                {
                    //若订单查询失败，则立即返回结果给微信支付后台
                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "订单查询失败");
                    Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                    Response.Write(res.ToXml());
                    Response.End();
                }
                //查询订单成功
                else
                {
                    string out_trade_no = notifyData.GetValue("out_trade_no").ToString();


                    if (bll.IsStatusByNumber(out_trade_no, RelexBarBLL.Common.enOrderStatus.Order))
                    {//如果是下单状态，支付
                        object t = notifyData.GetValue("total_fee");
                        string total_fee = t == null ? "0" : t.ToString();
                        decimal price;
                        decimal.TryParse(total_fee, out price);
                        
                        int i = bll.PaySuccessNotify(out_trade_no, transaction_id, price); //（订单通知修改状态）
                        if (!(i > 0))
                        {
                            WxPayData res2 = new WxPayData();
                            res2.SetValue("return_code", "FAIL");
                            res2.SetValue("return_msg", "订单查询失败");
                          //  Log.Error(this.GetType().ToString(), "Order query failure : " + res2.ToXml());
                            Response.Write(res2.ToXml());
                            Response.End();
                        }
                    }

                    WxPayData res = new WxPayData();
                    res.SetValue("return_code", "SUCCESS");
                    res.SetValue("return_msg", "OK");
                    Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());


                    Response.Write(res.ToXml());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
              //  log.Error(string.Format("接口：/WxAPI/WxPayNotify，异常信息：{0}", ex.ToString()));
            }

        }



        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData(out SysConfig SysConfig)
        {
            SysConfig = null;
            //接收从微信后台POST过来的数据
            System.IO.Stream s = Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            Log.Info(this.GetType().ToString(), "Receive data from WeChat : " + builder.ToString());

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                string xml = builder.ToString();
                if (string.IsNullOrEmpty(xml))
                {
                    throw new Exception("将空的xml串转换为WxPayData不合法!");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
                XmlNodeList nodes = xmlNode.ChildNodes;
                foreach (XmlNode xn in nodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    data.SetValue(xe.Name, xe.InnerText);
                }


                //2015-06-29 错误是没有签名
                if (data.GetValue("return_code").ToString() != "SUCCESS")
                {
                    return data;
                }
                //object shopid = data.GetValue("attach");
                //Guid shopId = Guid.Parse(shopid.ToString());
                //IShopWxInfoBLL bll = new ShopWxInfoBLL();
                //var info = bll.GetByShopId(shopId);
                //shopWxInfo = info;
                //data.CheckSign(info.MCHKEY);//验证签名,不通过会抛异常
            }
            catch (Exception ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                Log.Error(this.GetType().ToString(), "Sign check error : " + res.ToXml());
                Response.Write(res.ToXml());
                Response.End();
            }

            Log.Info(this.GetType().ToString(), "Check sign success");
            return data;
        }


        private bool QueryOrder(string transaction_id, string GZHAPPID, string MCHID, string MCHKEY)
        {
            WeChatAPI wxapi = new WeChatGZH();
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = wxapi.OrderQuery(GZHAPPID, MCHID, MCHKEY, Request.UserHostAddress.ToString(), req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public ActionResult ResultUrl()
        {
            if (!string.IsNullOrEmpty(Request["pid"]))
            {
                int pid = Convert.ToInt32(Request["pid"]);
                // ViewBag.Payrecord = cManage.GetModel(pid);
            }
            else
            {
              //  ViewBag.Cored = cManage.GetModel(" and out_trade_no='" + Request["no"] + "'");
            }

            ThirdServices bll = new ThirdServices();
            OtherPayServiceLog number = bll.GetPayOrdersDetails(Request["no"].ToString());
            ViewData["number"] = number.OrderNumber;
            ViewData["OID"] = number.OrderNumber;
            return View();
        }


        public string GetIP()

        {

            HttpRequest request = System.Web.HttpContext.Current.Request;

            string result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))
            {
                result = request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrEmpty(result))
            {
                result = request.UserHostAddress;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = "0.0.0.0";
            }

            return result;

        }
        /// <summary>
        /// 获取终端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp()
        {
            string userIP = "";
            try
            {
                if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";
                string CustomerIP = "";                //CDN加速后取到的IP simone 090805         
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }
                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!String.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }
            return userIP;
        }

    }
}