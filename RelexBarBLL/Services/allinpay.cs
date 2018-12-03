using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Services
{
    public class AliPay
    {
        #region DEBUG#
        public static string app_id = "2018091461340517";
        // 支付宝网关
        public static string gatewayUrl = "https://openapi.alipay.com/gateway.do";
        // 商户私钥，您的原始格式RSA私钥
        public static string private_key = "MIIEowIBAAKCAQEAokdzoLdaKwaN3o8FTlJpDWhPqoZmMIuT661iX0XMa5UwS5+CwQvqpM5bq92tdcoZDJsVWKqSw4FIGE4IokrVI/flxXUKbZJ696806eMu3Im9OCH+uaSvFRBPAUZJsaU9kL+pvOV35Nq1xLi6+MOTb1q9KeEYKgmWfO2zEFh+Md7OxjfdZH3Bn1c9TooQbHKPikoWb1iuwnYiQhTHqngfwlGPPtwsAF2kuYLnzJPmQetLwt0ecUC9Xe9waPYr3D3DeiGCgSEnWKFgp1mDwXppFxcXGids13xfBsu/0/JPjf52MKkV6iZuHe9Li8i60qnLJeacBSndspsgO/aVdaJtgwIDAQABAoIBABfQyKVjgzYhRhzNfIfcDFLNZjOuebzSHylNTsyQn8mCIY5XoIbYSt1Y85crd1N8D2UMNXvHvLY4eO4oNZgrx0/6B5yM6rTd9HH5IpjYoH35MO3V/14bTx6WQCH9wL7zcy+VEUIJoyjQ+DAgtD/h4/dJp81HnZY9xfoboFXbxtlmj6NnUH8vxaTk2u+XKcqNgBQEZx+cKh1yZtIBg/YZyBfNQ42CDLiw+Pkl+eOtDQGr+v536FEeqfUiNeJwlRkd0I8JwGCAc8ZuYV/dtuIHYHC/d2UTyWjIcxHLfUjsGSxDhARLWny6M0Wf6fWZbBrvP/Zk9ihbyQCxSrhC8w6g2KECgYEA12By+MS2cWyCFKeyvSyGDLSVmicDCsmKGknBIsj8qStw6efD/iTuzyvyHVd/Ky4H2voFWM94M13rtP07h20JwjwOSAoNnGfGiNB0e+PWLt0JozWc4FD1cdZOCqXxFiFdfbJ85Ncx36vn3gv/MfoF3MMvYs90rVHoP+fhqq5Z9kkCgYEAwOMpdC59kA+hywCJdgTM36I3JLaL/89+GMfts+chwnVN5fH2GoN/0xndJ0Cslnyt0RJw2ce2dtdklwuGD1XW9hgO4PkM7zWTP8xFrtAdAHcPhmdN9Mt33YhCHFXXAfIDf5rGmfEiOxYdzABqd6RveOigbMIIaPPGbbqDBawwlWsCgYBvX77UidTWfogd6kJxtO5074VVO8tE7sdtpKotNMYDLSWsr18YszyTAWSoa7ClQZ5qTFQrgs3jsjPLWhBIjcg4Gxo/goNbWAbAhRtXXZBKl57+OSEwlmz2Ox4MP5eWHBbeg0g2hlmSErFKj+WdkG8ro/uDG0h4wBxbv6tgudxnkQKBgQCztHAwoAe3qfiw6uUmoT5GtBjwCWrRf/0ZFVtums4nH6bdrtn8xbrXcGdMZMLmyW4fNTESlTB39CDxYHkb3HuTd8KU8zOLwbxYWfYp8hazEjnSnHhSsyYPrRz6zTatx05fGlIhpjigF/DW2SRYJ0j7uq+L5BJ1xXh/Tp83L79r1QKBgAkDIe4Fvycinz+pkZLolsCjkUf0CZo3SPlgsZBQtOn1iZIycioWhVHloQIS0lMUTGSAh/Jccnc5GxmC8sOvSuNHXVeTW1ekaMCCEzFAgbJLn1RfuTCeQbx3mYhook8tI5fChhfIORTBbuUDjbqiKVCq2yU7zU/+gcHHjIlcrTWY";
        // 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
        public static string alipay_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAo9BdrOlPxTrt1GnaEezzlySEqx7xug0mzoJeMOwmGSjqChTgUYeHX72d9Fuig+PSSanc+O+9Ib+5aSR/X+V67jHgAWaxkZXN5Q3ydX/h9/PvchLXLg7s/MrrHL4McKVpqd0gj/BKHjlnSGasWq3C7cdj4W0ho5E95IzkPOEUIPEzjqsg2tdR276SxLqrJS0ZtoEkDZ7lNPC7bogXviLLMJX9/gCKOVhmM5G24aN5AOQxrYGv3EjKkr2oQ/tNQqScR7HWtw+Z5WbJ8wGEAC7k2gi6+8nDY0MEqY2RAbM/sJHL9MbaMS/Beui/TH7INB6+VYpszxlY9ZxBYsgp4GexwQIDAQAB";
        // 签名方式
        public static string sign_type = "RSA2";
        // 编码格式
        public static string charset = "UTF-8";

        #endregion

        public static string AliPayUrl = "https://openapi.alipay.com/gateway.do";
        public static string APPID = "";
        public static string APP_PRIVATE_KEY = "";
        public static string ALIPAY_PUBLIC_KEY = "";

        /// <summary>
        /// 支付完成异步通知页面
        /// </summary>
        public static string aliPayNotifyURL = string.Empty;

        /// <summary>
        /// 交易类型
        /// </summary>
        public enum euTrade_type
        {
            /// <summary>
            /// app支付
            /// </summary>
            QUICK_MSECURITY_PAY = 1,
            /// <summary>
            /// H5支付
            /// </summary>
            QUICK_WAP_WAY = 2,
        }

        /// <summary>
        /// 生成支付订单
        /// </summary>
        /// <returns></returns>
        public string CreatePayDatas(decimal total_fee, string order_desc, string orderid, euTrade_type type)
        {
            LogsBLL.InsertAPILog("CreatePayDatas:PriveKEY", Guid.Empty, APP_PRIVATE_KEY);
            LogsBLL.InsertAPILog("CreatePayDatas:PublicKEY", Guid.Empty, ALIPAY_PUBLIC_KEY);

            ////IAopClient client = new DefaultAopClient(AliPayUrl, APPID, APP_PRIVATE_KEY, "json", "1.0", "RSA2", ALIPAY_PUBLIC_KEY, "GBK", false);
            //IAopClient client = new DefaultAopClient(AliPayUrl, APPID, System.Web.HttpContext.Current.Server.MapPath("~/Content/alipaykey/rsa_private_key.pem")
            //    , "json", "1.0", "RSA2", System.Web.HttpContext.Current.Server.MapPath("~/Content/alipaykey/rsa_public_key.pem"), "utf-8", true);
            //AlipayTradeAppPayRequest request = new AlipayTradeAppPayRequest();
            //AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
            ////model.Body = order_desc;
            ////model.Subject = order_desc;
            //model.Body = "fbddd";
            //model.Subject = "fbddd";
            //model.TotalAmount = total_fee.ToString();//小数
            //model.ProductCode = type.ToString();
            //model.OutTradeNo = orderid;
            //model.TimeoutExpress = "30m";
            //request.SetBizModel(model);
            //request.SetNotifyUrl(aliPayNotifyURL);
            ////这里和普通的接口调用不同，使用的是sdkExecute
            //try
            //{
            //    if (type == euTrade_type.QUICK_MSECURITY_PAY)
            //    {
            //        //string forms = string.Empty;
            //        //var dis = request.GetParameters();
            //        //foreach (string k in dis.Keys)
            //        //{
            //        //    forms += k + "=" + dis[k] + "&";
            //        //}
            //        //LogsBLL.InsertAPILog("CreatePayDatas:request", Guid.Empty, forms);

            //        //AlipayTradeAppPayResponse response = client.SdkExecute(request);

            //        AlipayTradeAppPayResponse response = client.pageExecute(request);

            //        LogsBLL.InsertAPILog("CreatePayDatas:response", Guid.Empty, response.Msg + ";" + response.Body);

            //        return response.Body;
            //    }
            //    else
            //    {
            //        var response = client.pageExecute(request, null, "post");
            //        return response.Body;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogsBLL.InsertAPILog("CreatePayDatas", Guid.Empty, ex.ToString());
            //    return string.Empty;
            //}

            //DEBUG
            type = euTrade_type.QUICK_WAP_WAY;

            DefaultAopClient client = new DefaultAopClient(gatewayUrl, app_id, private_key, "json", "1.0", sign_type, alipay_public_key, charset, false);
            // 组装业务参数model
            AlipayTradeWapPayModel model = new AlipayTradeWapPayModel();
            model.Body = order_desc;
            model.Subject = order_desc;
            model.TotalAmount = total_fee.ToString();
            model.OutTradeNo = orderid;
            model.ProductCode = type.ToString();
            model.QuitUrl = aliPayNotifyURL;

            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            // 设置支付完成同步回调地址
            // request.SetReturnUrl("");
            // 设置支付完成异步通知接收地址
            request.SetNotifyUrl(aliPayNotifyURL);
            request.SetBizModel(model);

            AlipayTradeWapPayResponse response = null;
            try
            {
                response = client.pageExecute(request, null, "post");
                return response.Body;
            }
            catch (Exception ex)
            {
                LogsBLL.InsertAPILog("CreatePayDatas", Guid.Empty, ex.ToString());
                return string.Empty;
            }
        }

        public string CreatePayUrl(int total_fee, string order_desc, string orderid)
        {
            DefaultAopClient client = new DefaultAopClient(AliPayUrl, APPID, APP_PRIVATE_KEY, "json", "1.0", "RSA2", ALIPAY_PUBLIC_KEY, "GBK", false);

            // 外部订单号，商户网站订单系统中唯一的订单号
            string out_trade_no = orderid.Trim();
            // 订单名称
            string subject = order_desc.Trim();
            // 付款金额
            string total_amout = total_fee.ToString();
            // 商品描述
            string body = order_desc.Trim();
            // 支付中途退出返回商户网站地址
            string quit_url = "";

            // 组装业务参数model
            AlipayTradeWapPayModel model = new AlipayTradeWapPayModel();
            model.Body = body;
            model.Subject = subject;
            model.TotalAmount = total_amout;
            model.OutTradeNo = out_trade_no;
            model.ProductCode = "QUICK_WAP_WAY";
            model.QuitUrl = quit_url;

            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            // 设置支付完成同步回调地址
            // request.SetReturnUrl("");
            // 设置支付完成异步通知接收地址
            request.SetNotifyUrl(aliPayNotifyURL);
            // 将业务model载入到request
            request.SetBizModel(model);

            AlipayTradeWapPayResponse response = null;
            try
            {
                response = client.pageExecute(request, null, "post");
                return (response.Body);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public bool CheckRSA(Dictionary<string, string> datas)
        {
            //切记alipaypublickey是支付宝的公钥，请去open.alipay.com对应应用下查看。
            //bool RSACheckV1(IDictionary<string, string> parameters, string alipaypublicKey, string charset, string signType, bool keyFromFile)
            bool flag = AlipaySignature.RSACheckV1(datas, ALIPAY_PUBLIC_KEY, "GBK", "RSA2", false);

            return flag;
        }

    }
}
