using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RelexBarBLL.WeChat.impl
{
    /// <summary>
    /// 微信小程序实现类
    /// </summary>
    public class WxMiniProgram : WeChatAPI
    {
        public WxUserInfo GetUserInfo(string code, string session_key)
        {
            try
            {
                string url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type={3}",
                    WxConfig.AppID, WxConfig.AppSecret,code, "authorization_code");
                string result = RelexBarBLL.Utils.HttpService.Post("",url,20);
               
                JObject jo=Json.JSONHelper.GetJObject(result);
                string encryptedData = jo["encryptedData"].ToString();
                string iv = jo["iv"].ToString();
                result = Encrypt.Util.UnAesStr(result, session_key, iv);
                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Login(string appid,string appSecret,string code)
        {
            try
            {
                string url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type={3}"
                    ,appid,appSecret,code, "authorization_code");
                return RelexBarBLL.Utils.HttpService.Post("",url,20);
            }
            catch (Exception e)
            {

                throw new Exception(string.Format("微信接口调用失败：{0}",e.ToString()));
            }
        }

        public WxPayData Unifiedorder(string appid,string MCHID, string MCHIDKEY, string requestIP, WxPayData inputObj)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new Exception("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new Exception("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new Exception("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new Exception("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new Exception("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new Exception("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WxPayConfig.NOTIFY_URL);//异步通知url
            }

            inputObj.SetValue("appid", appid);//公众账号ID
            inputObj.SetValue("mch_id", MCHID);//商户号
            inputObj.SetValue("spbill_create_ip", requestIP);//终端ip	  	    
            inputObj.SetValue("nonce_str",Utils.GenerateNonceStr());//随机字符串

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign(MCHIDKEY));
            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            Log.Debug("WxMiniProgram", "UnfiedOrder request : " + xml);
            string response = HttpService.Post(xml, url, false, 20);
            Log.Debug("WxMiniProgram", "UnfiedOrder response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response, MCHIDKEY);

            ReportCostTime(MCHIDKEY,url, timeCost, result);//测速上报

            return result;
        }
        /**
       * 
       * 测速上报
       * @param string interface_url 接口URL
       * @param int timeCost 接口耗时
       * @param WxPayData inputObj参数数组
       */
        private static void ReportCostTime(string MCHIDKEY,string interface_url, int timeCost, WxPayData inputObj)
        {
            //如果不需要进行上报
            if (WxPayConfig.REPORT_LEVENL == 0)
            {
                return;
            }

            //如果仅失败上报
            if (WxPayConfig.REPORT_LEVENL == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" &&
             inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS")
            {
                return;
            }

            //上报逻辑
            WxPayData data = new WxPayData();
            data.SetValue("interface_url", interface_url);
            data.SetValue("execute_time_", timeCost);
            //返回状态码
            if (inputObj.IsSet("return_code"))
            {
                data.SetValue("return_code", inputObj.GetValue("return_code"));
            }
            //返回信息
            if (inputObj.IsSet("return_msg"))
            {
                data.SetValue("return_msg", inputObj.GetValue("return_msg"));
            }
            //业务结果
            if (inputObj.IsSet("result_code"))
            {
                data.SetValue("result_code", inputObj.GetValue("result_code"));
            }
            //错误代码
            if (inputObj.IsSet("err_code"))
            {
                data.SetValue("err_code", inputObj.GetValue("err_code"));
            }
            //错误代码描述
            if (inputObj.IsSet("err_code_des"))
            {
                data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
            }
            //商户订单号
            if (inputObj.IsSet("out_trade_no"))
            {
                data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
            }
            //设备号
            if (inputObj.IsSet("device_info"))
            {
                data.SetValue("device_info", inputObj.GetValue("device_info"));
            }

            try
            {
                Report(MCHIDKEY,data);
            }
            catch (Exception ex)
            {
                //不做任何处理
            }
        }
        /**
	    * 
	    * 测速上报接口实现
	    * @param WxPayData inputObj 提交给测速上报接口的参数
	    * @param int timeOut 测速上报接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回测速上报接口返回的结果，其他抛异常
	    */
        public static WxPayData Report(string MCHIDKEY,WxPayData inputObj, int timeOut = 1)
        {
            string url = "https://api.mch.weixin.qq.com/payitil/report";
            //检测必填参数
            if (!inputObj.IsSet("interface_url"))
            {
                throw new Exception("接口URL，缺少必填参数interface_url！");
            }
            if (!inputObj.IsSet("return_code"))
            {
                throw new Exception("返回状态码，缺少必填参数return_code！");
            }
            if (!inputObj.IsSet("result_code"))
            {
                throw new Exception("业务结果，缺少必填参数result_code！");
            }
            if (!inputObj.IsSet("user_ip"))
            {
                throw new Exception("访问接口IP，缺少必填参数user_ip！");
            }
            if (!inputObj.IsSet("execute_time_"))
            {
                throw new Exception("接口耗时，缺少必填参数execute_time_！");
            }

            inputObj.SetValue("appid", WxPayConfig.APPID);//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.MCHID);//商户号
            inputObj.SetValue("user_ip", WxPayConfig.IP);//终端ip
            inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
            inputObj.SetValue("nonce_str",Utils.GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(MCHIDKEY));//签名
            string xml = inputObj.ToXml();

            Log.Info("WxMiniProgram", "Report request : " + xml);

            string response = HttpService.Post(xml, url, false, timeOut);

            Log.Info("WxMiniProgram", "Report response : " + response);

            WxPayData result = new WxPayData();
            result.FromXml(response, MCHIDKEY);
            return result;
        }

        public WxPayData OrderQuery(string gzhAppid,string MCHID,string MCHIDKEY,string ip, WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new Exception("订单查询接口中，out_trade_no、transaction_id至少填一个！");
            }

            inputObj.SetValue("appid", gzhAppid);//公众账号ID
            inputObj.SetValue("mch_id", MCHID);//商户号
            inputObj.SetValue("nonce_str", Utils.GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(MCHIDKEY));//签名

            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            Log.Debug("WxMiniProgram", "OrderQuery request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口提交数据
            Log.Debug("WxMiniProgram", "OrderQuery response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的数据转化为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response, MCHIDKEY);

            ReportCostTime(MCHIDKEY,url, timeCost, result);//测速上报

            return result;
        }
        
    }
}
