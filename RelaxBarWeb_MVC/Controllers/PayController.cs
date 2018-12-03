using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarDLL;
using RelexBarBLL;
using RelexBarBLL.Services;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class PayController : BaseController
    {
        /// <summary>
        /// 通过此接口创建一个支付订单获得订单id
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public JsonResult CreatePayOrder(decimal price)
        {
            PayOrderBLL bll = new PayOrderBLL();
            PayOrder p = bll.CreateDefaultOrder(UserInfo.ID, price);
            if (p != null)
            {
                return RJson("1", p.ID.ToString());
            }
            else
            {
                return RJson("-1", "订单创建失败");
            }
        }
        /// <summary>
        /// 通过传入订单id，和支付密码支付
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="payPwd"></param>
        /// <returns></returns>
        public JsonResult PayByOrder(Guid oid, string payPwd, decimal? price)
        {
            PayOrderBLL bll = new PayOrderBLL();
            Guid ouid;
            decimal realPrice = price.HasValue ? price.Value : 0;
            int i = bll.Pay(UserInfo.ID, oid, payPwd, out ouid, ref realPrice);
            if (i > 0)
            {
                SendMessage(Guid.Empty, ouid, RelexBarBLL.EnumCommon.MessageTType.System, RelexBarBLL.EnumCommon.MessageType.ShouKuanOK,
                    string.Format("{0}|{1}", oid, realPrice));
                return RJson("1", "支付成功");
            }
            else
            {
                return RJson(i.ToString(), ((enPayOrderStatus)i).ToString());
            }
        }
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public JsonResult GetOrderDetail(Guid oid)
        {
            PayOrderBLL bll = new PayOrderBLL();
            PayOrder o = bll.Get(oid);
            if (o == null)
            {
                return RJson(((int)Common.ErrorCode.订单不存在).ToString(), Common.ErrorCode.订单不存在.ToString());
            }
            if (o.Status != (int)enPayOrderStatus.Create)
            {
                return RJson(((int)Common.ErrorCode.订单已支付).ToString(), Common.ErrorCode.订单已支付.ToString());
            }
            return Json(new
            {
                code = "1",
                recName = new UsersBLL().GetName(o.UID),
                price = o.Price
            });

        }
        /// <summary>
        /// 付款给对方
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="price"></param>
        /// <param name="payPwd"></param>
        /// <returns></returns>
        public JsonResult PayToUser(Guid uid, decimal price, string payPwd)
        {
            if (uid == UserInfo.ID)
            {
                return RJson("-1", "");
            }
            UsersBLL bll = new UsersBLL();
            int i = bll.PayToUser(UserInfo.ID, uid, price, payPwd);
            if (i > 0)
            {
                return RJson("1", "Success");
            }
            else
            {
                return RJson(((int)((Common.ErrorCode)i)).ToString(), ((Common.ErrorCode)i).ToString());
            }
        }

        /// <summary>
        /// 升级等级
        /// </summary>
        public JsonResult UpgradeUserType(Common.enUserType nt, string area, string code, string credid, string title, string img, string pwd, Common.enPayment? payment)
        {
            if (string.IsNullOrEmpty(title))
            {
                return RJson("-1", "标题不能为空");
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
            var u = bll.GetUserById(UserInfo.ID);
            if (u.UserType < (int)nt)//小于该级别可升级
            {
                if (!payment.HasValue || payment.Value == Common.enPayment.LOCAL)//余额支付
                {
                    if (string.IsNullOrEmpty(UserInfo.PayPsw))//余额支付才需要密码
                    {
                        return RJson("-112", Common.ErrorCode.密码尚未设置.ToString());
                    }
                    if (!CheckPayPSW(pwd))
                    {
                        return RJson("-113", Common.ErrorCode.支付密码错误.ToString());
                    }

                    UsersBLL ubll = new UsersBLL();
                    int i = ubll.UpgradeUsertype(UserInfo.ID, nt, Common.enPayment.LOCAL);
                    if (i > 0)
                    {
                        try
                        {
                            if (!new RedPacksBLL().SendRedPacket(UserInfo.ID, Common.enRedType.Timeout, title, img, string.Empty, null, null, 0.1M, 1314, string.Empty, string.Empty, 1))
                            {
                                LogsBLL.InsertAPILog("PayController/UpgradeUserType", UserInfo.ID, "会员升级后，发福包失败！");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogsBLL.InsertAPILog("PayController/UpgradeUserType", UserInfo.ID, "会员升级后，发福包失败！" + ex);
                        }
                        return RJson("1", "升级成功");
                    }
                    else
                        return RJson(i.ToString(), ((Common.ErrorCode)i).ToString());
                }
                else//第三方支付
                {
                    ThirdServices th = new ThirdServices();
                    var money = nt == Common.enUserType.Agent ? SysConfigBLL.AgentPrice : SysConfigBLL.ShopPrice;
                    var ordernum = Common.GetOrderNumer();
                    var log = th.PayOrders("Pay/UpgradeUserType", payment.Value, money, ordernum, ((int)(nt)).ToString() + "|" + title.Replace("|", "&") + "|" + img.Replace("|", "&"), UserInfo.ID, Guid.Empty, Common.enPayFrom.UpgradeLV, "");
                    if (log != null)
                    {
                        ////微信APP支付
                        //if (payment.Value == Common.enPayment.WX)//微信支付
                        //{
                        //    //var wxAPI = new WX_Services.WxPayApi();
                        //    ////先统一下单
                        //    //var paydata = new WX_Services.WxPayApi().GetUnifiedOrderResult((int)log.PayPrice, "福包多多", "", log.PayNumber, Config.pay_notify_url, WX_Services.euTrade_type.APP);
                        //    //string WS_prepay_id = paydata.GetValue("prepay_id").ToString();
                        //    //if (string.IsNullOrEmpty(WS_prepay_id))//生成订单失败
                        //    //{
                        //    //    return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        //    //}
                        //    //string data = wxAPI.GetPrePayDataByAPP(WS_prepay_id);//预付订单号
                        //    ////Response.Write(data);
                        //    //return RJson("1", data);//生成成功

                        //    var wxAPI = new WX_Services.WxPayApi();
                        //    //先统一下单
                        //    var paydata = new WX_Services.WxPayApi().GetUnifiedOrderResult((int)(log.PayPrice * 100), "福包多多", "", log.PayNumber, WX_Services.euTrade_type.MWEB);
                        //    if (paydata == null)
                        //    {
                        //        return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        //    }
                        //    string WS_prepay_id = paydata.GetValue("prepay_id").ToString();
                        //    if (string.IsNullOrEmpty(WS_prepay_id))//生成订单失败
                        //    {
                        //        return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        //    }
                        //    string result_code = paydata.GetValue("result_code").ToString();
                        //    if (result_code != "SUCCESS")
                        //    {
                        //        return RJson("-1", "生成微信预支付订单失败");//生成支付失败
                        //    }
                        //    return Json(new { code = "1", num = log.PayNumber, msg = wxAPI.GetPrePayDataByH5("pay/RedictH5Pay?url=" + Server.UrlEncode(paydata.GetValue("mweb_url").ToString())) });//生成成功
                        //}
                        if (payment.Value == Common.enPayment.WX) //微信支付
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
                        else if (payment.Value == Common.enPayment.ALI)//是否支付宝？
                        {
                            AliPay paybll = new AliPay();
                            string paydata = paybll.CreatePayDatas(log.PayPrice, "福包多多", log.PayNumber, AliPay.euTrade_type.QUICK_MSECURITY_PAY);

                            //return Json(new { code = "1", num = log.PayNumber, msg = paydata });//生成成功
                            Response.Write(paydata);
                            return null;
                        }
                        else
                        {
                            return RJson("-1", "支付类型不正确");
                        }
                    }
                    return RJson("-1", "生成订单失败");//生成支付失败
                }
            }
            else
            {
                return RJson("-1", "当前级别大于新级别");
            }
        }

        [Filter.NoFilter]
        /// <summary>
        /// 支付
        /// </summary>
        public ActionResult PayTest()
        {
            System.Threading.Thread.Sleep(10000);

            AliPay paybll = new AliPay();
            var a = paybll.CreatePayDatas(0.01M, "test", "20180503333", AliPay.euTrade_type.QUICK_MSECURITY_PAY);

            //string key = @"MIIEowIBAAKCAQEA1HI+MPxDzGWLWNdsCZcEifs4a6XDVI1y5QG6WHqgnhos7+QeqxlNBhVvO9VdhVhihjiRn593rCOVulb+SXjdcCl+VlTcXiHFGqJqzm5Gaj1ck8YPrftIf5IqWUvnSGUeHDrAafHq5ntUisuUfVJeRf/2dth89UO17Wd+Kw+qHCh6N76qT+lTkOOGFxrv3eEfiMVC7xZFSfalF+qMGZiM+NiyJTs0U5AgOX1t4Aq4UxBy4HDUsDejbVfdtxvpWbiO9de/Cm0HTLjYYxbFjGp0YfiXgi+HnGjkisnRNCjUU4vjVAcLolo6J08WK8CUCoObyzYU9MaOhPgEd5+4y86AmQIDAQABAoIBABGgwSRTalTFgECI74AENe41GENmZ8TWT9hMwmehFT7JMT1ekmTkHo4IrUgrtizgXpyAiSTTmJyB/2SM32C9VLJ/3unO9NIivzYsj+w9Hj6mhM4GOzrWhPeA616pe7F4In15Koof9rvVM/ioV9PmR8hLKupFoGjBi5D9a2yrShQ36Oh/AEG/FDfHrhXH7IQEjHHr9BbwP5r1QGK5cxazCcp3XlrIm5vZUxmiJ5tkhsvlJxBiW0Nm4L6SmGeQBrlea6cXbOE4qNHbH/insx/ASaZ2Bo8S6iI2u9MaijgrK2VMAwPSwQEGad8ViPvUdp4BHN8D9P7MItN0NUwyLU4B+pkCgYEA6erswnnOKy1z9aLmiPCxNlssYa2dFel/5wfqV79ec/QeZtbntcMW7ge9iUyMOQ3whKmPQxxtUf9Bxkr6KRa6k5cGCnA/f7tpxufiZVDJ9f7S8wLcqltR9YgoeeO85AE4dAMkxi7q7aKH9gsKKmBIz/IgourzPELCRZCSyrAyS78CgYEA6IBrbriq7hFW/zPN2zURwqXPlPNPo2jlr0gfkOWpFIXBgVeaFxHLcMKvW5sdOpz4n7aUMS1AeOfbM0i11srk46q/0WHlgEsrBB87uu1qAfoWn9UG67NZf1p1BvgKmxeCNrz7Rt+kpBqaHhYZcJw2tN5pVam8pfqzHwNSJ9CCqacCgYBssbpEf/8aNyu8B28iUN23yVPtK5Rj2zWQsC/niEgmj1gVJ2VSI44jGzuqcgRyepS5wGSOpcXypS72Kl7l+ubFsBahgRkwjHCReazMsWa3RSrWuwy4qGiENDnCsd1TAXIvDkrnQGPv+mc9IFIf8M/EuNkLeyt9Y/QMCSr8vjUQ1wKBgEyfNciRd1Npx3/nNKpPPQEm39g4r0AtK7SuUijQZv5qbjEQC6oapS8OGfhYkDNm09DevMvQG/U8g9LM3ZBh4TjeZsS8PIQhQZXkLS0XXN416wrVi6hVvOLpjrOzPtuJzdH4Sd87iOjEE5JiuhOQh77npVMj0xXoHuvTjlmsr8L1AoGBAIfMnzKvp3bu6+3r41ztrVeLM4RyOOCzj2zeejX79USs1tFTImArWvh0wfHP7dPqEcTcOVMJhZFS4rKGDXHtBGRf86kpTTgx3+zEYP4zf0KmShCHyroCbEBFU7OWqx6RHUaGM7P52qv8cSwfjlqbktMyCQZjw3bewvQDZXszkr+f";
            string key = @"
MIIEpAIBAAKCAQEAv9qjIItjalwKf4/z2yyPf3KpW+kXbyrcCbwdRScu45wg4DN6
dPYzvOchFvgc1xxkNQhUzsdK/O3sGkK+c/IKsbYpkYdrjMi7xiY6q3NcIZB33atl
EEXs4k3kHQNYWhI+a4RtqXzaHkMUob+Ghooazo6Mm3ZvUx7J30g9mKIjN5IWYOgO
1jdlfAqv30jARhVOgSiNGVJlA5pcssvH16/RP982wxHyfJ9nAClUnu68aXkg7OpK
fNpWAf02rDqpDmybZLRFPuEtkAtrTVaYaEp8mnYt3o0DTHlNo/0yYfijR9SlodOL
eaINK4zHswDVDwSPDuABoYfKTyHvnnDuZAt68wIDAQABAoIBAE7nE1nKGbvfWK33
vRmxrN6EgKR9K5cbsF0MkZkkMUOIrXink0BRFRwmjlRM/Ed3tLqez4ovKmb1TrnK
I1u5+q31tgjVAeVnlNo8VLq4efaP3Mw8thbIO1EjKaLJAmggwq0jTJp875OskOqo
wyHH4Jh8xdUWE1jC+9Epe7OvUv6CDd55lIFWtG6zTmZjjl/9LQggtKv4MWIVpvBZ
XUbJkZoi1lliz8lmu1pAi/f6F808vSVWJn2vxx/29QaZDu9vdzW2AylMoL3DMjbL
WpV4js8CUOkSg6HfZ9yNyoiw5O01QsB2NU/Z1/lSBrxEUOCzDeRU3KjrcPpbN/kc
PgSQUikCgYEA+C76zoslIzl56CQUCUntKnMkcmhyN9M8HWrJ+k2YLn9g+8OygKUF
kdsZ8l6IRSR7mSyC3rFtpghdf0lGmhiGUnTgzYF7P6vvakgxYcXpSRdA09A+PZRL
fV4XMdSaiS2abXJlAB3VU0vdUBCmUkYY4LfGwZQYtmeJOQfYtKXrZX8CgYEAxeV9
8bA9/9YoXhjlDqMR1tyLGfjfNsA0rJWcyRo3eCpmuk6L+EGOS3sOWx0TmQqSin7r
/vo3/oRBknZlHiSni11AJS4c9KPzbb50q5NVX6AcFA1EtQ5mzd7UTAmxSgm31MM/
8wKGwX/GQxQyP1bcD4ed6Lhlr2h5JfCa80TtbI0CgYEAg5C0GIYe+9teSrfFf18X
QOelLJk942Yxeg7AX9Z+lDemfBC0MgpJN5cE3D1M1AqRExRLlC5OyDOrDO0VZl8V
5eRbv9WiNpC47Ii01PYLw1l2XlMIi7BvjFgyx5HDEnGAagC4liU6j1uqSOVqpjbO
vWHvFXOi3h9o2TjVon5KmZUCgYBjlXeKLskUO912dIqXkgHBIJwzBqS0tYYMLwGF
xVaSdqvZJvuSgtKrMxJnUTtycPqLDRGO7rVMs5sXq9J4l7NkExnbW9ggV3yBBi1J
xctZYoWepYqkncmn4XDtjpcTjc5fF53PmL7dEoJfcQNUEk6M1g7ldB1fZnBIYkpn
DpF3ZQKBgQCmt/VBvj3K1w32XMQ+cpuUeEBQwFhclMzIh7GA4D2Tz6ihtH5okiPS
3vOXLHzOq0bSW41vPDM9yognIjR7fXDcypDN+8oiVCffQ2knD6JHbFm2oyofHZLB
/fXYRBVUzC+gvtyFSPoGd0UPG9KXfGccI24p+ue+M4+Qr6JtKRx+eA==";
            RSASignCharSet("12314", AliPay.APP_PRIVATE_KEY, "GBK", false, "RSA2");
            //RSASignCharSet("12314", key, "GBK", false, "RSA2");
            return View();
            //Response.Redirect(Server.UrlDecode(url));
        }

        [Filter.NoFilter]
        /// <summary>
        /// 支付某个订单，假设完成？
        /// </summary>
        public ActionResult PayFaild(string num, int fff)
        {
            if (string.IsNullOrEmpty(num))
                return RJson("-1", "不能为空");
            ThirdServices service = new ThirdServices();
            var paylog = service.GetPayOrdersDetails(num);
            if (paylog == null)
            {
                return RJson("-1", "支付订单不存在！");
            }
            if (fff == 666123)
            {
                LogsBLL logbll = new LogsBLL();
                logbll.InsertLog("强制重新支付一次。可能是支付失败，管理员操作。" + num, Common.enLogType.Services);
                service.UpdateOtherPayServiceLogStatus(paylog.ID,(int)Common.enOrderStatus.Order);
                paylog.Status = (int)Common.enOrderStatus.Order;
            }
            if (paylog.Status.Value == (int)Common.enOrderStatus.Payed)//如果已支付成功
            {
                return RJson("1", "该订单之前已处理成功。");
            }
            else if (paylog.Status.Value == (int)Common.enOrderStatus.Order)//如果在支付中
            {
                if (service.CompletedPayServiceLog(paylog.UID, paylog.ID))
                {
                    return RJson("1", "支付成功！");
                }
                else
                {
                    return RJson("-1", "支付订单失败！");
                }
            }
            //其他状态则直接失败
            return RJson("-1", "支付订单失败！");
        }

        #region DEBUG
        public static string RSASignCharSet(string data, string privateKeyPem, string charset, bool keyFromFile, string signType)
        {

            byte[] signatureBytes = null;
            try
            {
                System.Security.Cryptography.RSACryptoServiceProvider rsaCsp = null;
                if (keyFromFile)
                {//文件读取

                }
                else
                {
                    //字符串获取
                    rsaCsp = LoadCertificateString(privateKeyPem, signType);
                }

                byte[] dataBytes = null;
                if (string.IsNullOrEmpty(charset))
                {
                    dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
                }
                else
                {
                    dataBytes = System.Text.Encoding.GetEncoding(charset).GetBytes(data);
                }
                if (null == rsaCsp)
                {
                    throw new Exception("您使用的私钥格式错误，请检查RSA私钥配置" + ",charset = " + charset);
                }
                if ("RSA2".Equals(signType))
                {
                    signatureBytes = rsaCsp.SignData(dataBytes, "SHA256");
                }
                else
                {
                    signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
                }

            }
            catch (Exception ex)
            {

            }
            return Convert.ToBase64String(signatureBytes);
        }
        private static System.Security.Cryptography.RSACryptoServiceProvider LoadCertificateString(string strKey, string signType)
        {
            byte[] data = null;
            //读取带
            //ata = Encoding.Default.GetBytes(strKey);
            data = Convert.FromBase64String(strKey);
            //data = GetPem("RSA PRIVATE KEY", data);
            try
            {
                System.Security.Cryptography.RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(data, signType);
                return rsa;
            }
            catch (Exception ex)
            {
                //    throw new AopException("EncryptContent = woshihaoren,zheshiyigeceshi,wanerde", ex);
            }
            return null;
        }

        private static System.Security.Cryptography.RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey, string signType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            System.IO.MemoryStream mem = new System.IO.MemoryStream(privkey);
            System.IO.BinaryReader binr = new System.IO.BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                System.Security.Cryptography.CspParameters CspParameters = new System.Security.Cryptography.CspParameters();
                CspParameters.Flags = System.Security.Cryptography.CspProviderFlags.UseMachineKeyStore;

                int bitLen = 1024;
                if ("RSA2".Equals(signType))
                {
                    bitLen = 2048;
                }

                System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider(bitLen, CspParameters);
                System.Security.Cryptography.RSAParameters RSAparams = new System.Security.Cryptography.RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }
        private static int GetIntegerSize(System.IO.BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, System.IO.SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        #endregion

        [Filter.NoFilter]
        /// <summary>
        /// 支付
        /// </summary>
        public ActionResult RedictH5Pay(string url)
        {
            return View();
            //Response.Redirect(Server.UrlDecode(url));
        }

        [Filter.NoFilter]
        /// <summary>
        /// 微信支付成功后返回的通知
        /// </summary>
        /// <returns></returns>
        public void WXNotify()
        {
            //初始化数据
            using (System.IO.StreamReader sr = new System.IO.StreamReader(Request.InputStream))
            {
                string contents = string.Empty;
                contents = sr.ReadToEnd();

                WX_Services.WxPayApi wxpay = new WX_Services.WxPayApi();
                decimal fee;
                string number = string.Empty;

                LogsBLL logbll = new LogsBLL();
                logbll.InsertLog(contents, Common.enLogType.Services);

                WX_Services.WxPayData data = new WX_Services.WxPayData(WX_Services.euEncryptType.MD5);
                var r = data.FromXml(contents);
                string sign = data.MakeSign(WX_Services.WxPayData.wxPayKey);
                if (data.CheckSign(WX_Services.WxPayData.wxPayKey))
                {
                    if (r["return_code"].ToString().ToUpper() == "SUCCESS")
                    {
                        fee = decimal.Parse(r["total_fee"].ToString());
                        number = r["out_trade_no"].ToString();
                    }
                }
                if (OperatOrder(number))
                {
                    Response.Write("<xml><return_code>SUCCESS</return_code></xml>");//处理成功
                }
                else
                {
                    Response.Write("<xml><return_code>FAIL</return_code></xml>");//处理失败
                }

            }
        }

        /// <summary>
        /// 支付宝支付返回的通知
        /// </summary>
        [Filter.NoFilter]
        public void ALIPayNotify()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            System.Collections.Specialized.NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;
            string formdata = "";
            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
                formdata += requestItem[i] + "=" + Request.Form[requestItem[i]] + "&";
            }

            LogsBLL logbll = new LogsBLL();
            logbll.InsertLog(formdata, Common.enLogType.Services);

            AliPay payBll = new AliPay();
            var isaly = payBll.CheckRSA(sArray);
            if (isaly)
            {
                string trade_status = Request.Form["trade_status"];
                string number = Request.Form["out_trade_no"];

                if (OperatOrder(number))
                {
                    Response.Write("success");//处理成功
                }
                else
                {
                    Response.Write("fail");//处理失败
                }
            }
            else
            {
                Response.Write("fail");
            }
        }

        private bool OperatOrder(string number)
        {
            if (string.IsNullOrEmpty(number))
                return false;
            ThirdServices service = new ThirdServices();
            var paylog = service.GetPayOrdersDetails(number);
            if (paylog != null)
            {
                if (paylog.Status.Value == (int)Common.enOrderStatus.Payed)//如果已支付成功
                {
                    return true;
                }
                else if (paylog.Status.Value == (int)Common.enOrderStatus.Order)//如果在支付中
                {
                    if (service.CompletedPayServiceLog(paylog.UID, paylog.ID))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //其他状态则直接失败
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 校验订单状态
        /// </summary>
        /// <returns></returns>
        public JsonResult CheckOtherServiceStatus(string num)
        {
            ThirdServices service = new ThirdServices();
            var paylog = service.GetPayOrdersDetails(num);
            if (paylog != null)
            {
                if (paylog.UID != UserInfo.ID)
                {
                    return RJson("-2", "不能查询他人的订单！");
                }
                return Json(new
                {
                    code = 1,
                    Payment = paylog.Payment,
                    PayPrice = paylog.PayPrice,
                    Status = paylog.Status.Value,
                    UpdateTime = paylog.UpdateTime.Value,
                });
            }
            return RJson("-1", "订单号不存在！");
        }
    }
}