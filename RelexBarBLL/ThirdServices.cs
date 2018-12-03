using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Net;
using System.IO;

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.MNS.Model;
using Aliyun.MNS;
using Aliyun.Acs.Dysmsapi.Model.V20170525;

namespace RelexBarBLL
{
    public class ThirdServices : Common
    {
        LogsBLL logBll = new LogsBLL();

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="recPhone"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendSms(string recPhone, string before, string msg)
        {
            try
            {
                //System.Net.WebClient wc = new System.Net.WebClient();
                //string result = wc.DownloadString(string.Format(SysConfigBLL.SMSUrl, SysConfigBLL.SMSUser, SysConfigBLL.SMSPsw, recPhone,
                //    System.Web.HttpUtility.UrlEncode(msg)));
                //wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                //string data = @"appid={0}&to={1}&content={2}&signature={3}";
                //string result = Encoding.UTF8.GetString(wc.UploadData(SysConfigBLL.SMSUrl, Encoding.UTF8.GetBytes(string.Format(data
                //    , SysConfigBLL.SMSUser, recPhone, msg, SysConfigBLL.SMSPsw))));
                //SysConfigBLL bll = new SysConfigBLL();
                //string url = string.Empty;
                //string to = string.Empty;
                //if (before == "+86")
                //{
                //    to = recPhone;
                //    url =bll.Get("SMSURL");
                //}
                //else {
                //    to = before + recPhone;
                //    url = bll.Get("INTERNATIONALSMSURL");
                //}
                //string appid = bll.Get("SMSUSER");
                //string content = msg;

                //string signature = bll.Get("SMSPSW");

                //string result= Post("appid="+appid+ "&to="+to+ "&content="+content+ "&signature="+signature, url,10);

                //if (result.ToLower().Contains("\"success\""))//成功
                //    result = "1";
                //else
                //{
                //    logBll.InsertLog(Guid.Empty, "发送短信失败:" + result, enLogType.Error);
                //    result = "未知错误，请联系管理员";
                //}

                String product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
                String domain = SysConfigBLL.SMSUrl;//短信API产品域名（接口地址固定，无需修改）
                String accessKeyId = SysConfigBLL.SMSUser;
                String accessKeySecret = SysConfigBLL.SMSPsw;
                IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
                //初始化ascClient,暂时不支持多region（请勿修改）
                DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
                IAcsClient acsClient = new DefaultAcsClient(profile);
                SendSmsRequest request = new SendSmsRequest();
                //发送国际/港澳台消息时，接收号码格式为00+国际区号+号码，如“0085200000000”
                if (string.IsNullOrEmpty(before) || before == "86")//国内短信
                {
                    request.PhoneNumbers = recPhone;
                    //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
                    request.TemplateCode = SysConfigBLL.SMSTemplateCode;
                }
                else//国际短信
                {
                    request.PhoneNumbers = "00" + before + recPhone;
                    //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
                    request.TemplateCode = SysConfigBLL.SMSTemplateCode2;
                }
                //必填:短信签名-可在短信控制台中找到
                request.SignName = SysConfigBLL.SMSSignName;
                request.TemplateParam = "{\"code\":\"" + msg + "\"}";
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                if (sendSmsResponse.Message == "OK")//发送成功
                {
                }
                else
                {
                    logBll.InsertLog(Guid.Empty, recPhone + "发送短信失败:" + sendSmsResponse.Message, enLogType.Error);
                }
                return true;
            }
            catch (Exception ex)
            {
                logBll.InsertLog(Guid.Empty, recPhone + "发送短信失败:" + ex.ToString(), enLogType.Error);
                return false;
            }
        }
        public static string Post(string param, string url, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;


                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = timeout * 1000;

                //设置POST的数据类型和长度
                request.ContentType = "application/json;charset=utf-8";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(param);
                request.ContentLength = data.Length;

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recEmail"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool SendEmail(string recEmail, string subject, string content)
        {
            try
            {
                CommonClass.SendMail sm = new CommonClass.SendMail(SysConfigBLL.EmailUser, SysConfigBLL.EmailPsw,
                    SysConfigBLL.EmailUser, recEmail, SysConfigBLL.EmailServer);
                sm.Subject = subject;
                sm.Body = content;
                sm.NewSend();

                //logBll.InsertLog(Guid.Empty, string.Format("收件{0}；主题：{1}；内容：{2}"), enLogType.Email,subject,content);

                return true;
            }
            catch (Exception ex)
            {
                logBll.InsertLog(Guid.Empty, "发送邮件出错：" + ex.ToString(), enLogType.Error);
                return false;
            }
        }

        /// <summary>
        /// 插入支付接口日志
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Payment"></param>
        /// <param name="money"></param>
        /// <param name="ordernum"></param>
        /// <param name="remark">标注</param>
        /// <param name="openid">微信/支付等第三方平台用户id</param>
        /// <returns></returns>
        public OtherPayServiceLog PayOrders(string page, enPayment Payment, decimal money
            , string ordernum, string remark, Guid UID, Guid ToUid, enPayFrom PayFrom, string openid)
        {
            try
            {
                OtherPayServiceLog paylog = new OtherPayServiceLog();
                paylog.ID = Guid.NewGuid();
                paylog.Page = page;
                paylog.Payment = Payment.ToString();
                paylog.PayPrice = money;
                paylog.PayNumber = GetServiceNumer();
                if (string.IsNullOrEmpty(ordernum))
                {
                    paylog.OrderNumber = GetOrderNumer();
                }
                else
                {
                    paylog.OrderNumber = ordernum;
                }
                paylog.ReqStr = "";
                paylog.Status = (int)enOrderStatus.Order;
                paylog.Remark = remark;
                paylog.UID = UID;
                paylog.TOID = ToUid;
                paylog.OrderType = (int)PayFrom;
                paylog.CreateTime = paylog.UpdateTime = DateTime.Now;

                using (RelexBarEntities DBContext = new RelexBarEntities())
                {
                    DBContext.OtherPayServiceLog.Add(paylog);
                    if (DBContext.SaveChanges() > 0)
                    {
                        return paylog;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logBll.InsertLog(Guid.Empty, "支付失败:" + ex.ToString(), enLogType.Error);
                return null;
            }
        }

        public OtherPayServiceLog GetPayOrdersDetails(string paynumber)
        {
            try
            {
                using (RelexBarEntities DBContext = new RelexBarEntities())
                {
                    var paylog = DBContext.OtherPayServiceLog.FirstOrDefault(m => m.PayNumber == paynumber);

                    return paylog;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 完成订单操作
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool CompletedPayServiceLog(Guid? UID, Guid ID)
        {
            try
            {
                using (RelexBarEntities DBContext = new RelexBarEntities())
                {
                    var paylog = DBContext.OtherPayServiceLog.FirstOrDefault(m => m.UID == UID && m.ID == ID);
                    if (paylog != null)
                    {
                        if (paylog.Status == (int)enOrderStatus.Order)//当前状态为下单状态
                        {
                            if (paylog.OrderType == (int)enPayFrom.Recharge)//直接充值
                            {
                                RechargeBLL recharBll = new RechargeBLL();
                                if (recharBll.InsertBalance(paylog.UID.Value, paylog.PayPrice) <= 0)//充值结果
                                {
                                    return false;
                                }
                            }
                            if (paylog.OrderType == (int)enPayFrom.UpgradeLV)//如果是升级订单
                            {
                                UsersBLL ubll = new UsersBLL();
                                var lls = paylog.Remark.Split('|');
                                if (lls.Count() < 3)//不符合条件
                                {
                                    return false;
                                }
                                int i = ubll.UpgradeUsertype(paylog.UID.Value, (Common.enUserType)int.Parse(lls[0]), (Common.enPayment)Enum.Parse(typeof(Common.enPayment), paylog.Payment));
                                if (i > 0)
                                {
                                    try
                                    {
                                        if (!new RedPacksBLL().SendRedPacket(paylog.UID.Value, Common.enRedType.Timeout, lls[1], lls[2], string.Empty, null, null, 0.1M, 1314, string.Empty, string.Empty, 1))
                                        {
                                            LogsBLL.InsertAPILog("ThirdServices/CompletedPayServiceLog", paylog.UID.Value, "会员升级后，发福包失败！");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogsBLL.InsertAPILog("ThirdServices/CompletedPayServiceLog", paylog.UID.Value, "会员升级后，发福包失败！" + ex);
                                    }
                                }
                                else
                                    return false;
                            }
                            if (paylog.OrderType == (int)enPayFrom.RedPaged)//如果是发红包订单
                            {
                                //TO DO
                                var lls = paylog.Remark.Split('|');
                                try
                                {
                                    int? sex = null;
                                    if (!string.IsNullOrEmpty(lls[7]))
                                    {
                                        sex = int.Parse(lls[7]);
                                    }
                                    if (!new RedPacksBLL().SendRedPacket(paylog.UID.Value, Common.enRedType.Single_OtherPay, lls[1], lls[2], lls[3], null, null, decimal.Parse(lls[4]), paylog.PayPrice, lls[5], lls[6], sex, int.Parse(lls[8])))
                                    {
                                        LogsBLL.InsertAPILog("ThirdServices/CompletedPayServiceLog", paylog.UID.Value, "用户/商家发福包失败！");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogsBLL.InsertAPILog("ThirdServices/CompletedPayServiceLog", paylog.UID.Value, "用户/商家发福包失败！" + ex);
                                    return false;
                                }
                            }
                            if (paylog.OrderType == (int)enPayFrom.OutLinePay)//如果是线下支付金额
                            {
                                Common.enPayment payment;
                                if (!Enum.TryParse(paylog.Payment, out payment))
                                {
                                    payment = enPayment.WFT;
                                }
                                PayListBLL paybll = new PayListBLL();
                                if (paybll.PayForOutline(paylog.UID.Value, paylog.TOID, paylog.ID, payment, paylog.PayPrice) <= 0)//充值结果
                                {
                                    return false;
                                }
                            }
                            else//购买产品
                            {
                                OrdersBLL orderbll = new OrdersBLL();
                                var order = orderbll.GetDetail(paylog.OrderNumber);
                                if (order != null && order.Status == (int)enOrderStatus.Order)//订单还是下单状态，未支付成功
                                {
                                    orderbll.UpdatePayment(order.ID, enPayment.WX);
                                    if (orderbll.UpdateStatus(order.ID, enOrderStatus.Payed) <= 0)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        paylog.Status = (int)enOrderStatus.Payed;
                        paylog.UpdateTime = DateTime.Now;

                        DBContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logBll.InsertLog(Guid.Empty, "完成订单失败：" + ex, enLogType.Services);
                return false;
            }
        }

        public List<OtherPayServiceLog> GetByNumber(Guid UID, string number, enOrderStatus? status)
        {
            using (RelexBarEntities DBContext = new RelexBarEntities())
            {
                var q = DBContext.OtherPayServiceLog.Where(m => m.UID == UID && m.OrderNumber == number);
                if (status != null)
                {
                    q = q.Where(m => m.Status == (int)status.Value);
                }
                return q.ToList();
            }
        }

        public decimal GetPayPrice(string number)
        {
            using (RelexBarEntities DBContext = new RelexBarEntities())
            {
                List<OtherPayServiceLog> list = DBContext.OtherPayServiceLog.Where(m => m.OrderNumber == number).ToList();
                decimal sum = list.Sum(m => m.PayPrice);
                //     decimal amount = list.GroupBy(m => new { discountId = m.DiscountCouponID, amount = m.DiscountAmount }).Sum(m => m.Key.amount);
                //      return sum - amount;
                return sum;
            }
        }

        public bool IsStatusByNumber(string number, enOrderStatus status)
        {
            using (RelexBarEntities DBContext = new RelexBarEntities())
            {
                return DBContext.OtherPayServiceLog.Count(m => m.Status == (int)status && m.OrderNumber == number) > 0;
            }
        }

        public int UpdateOtherPayServiceLogStatus(Guid ID, int status)
        {
            using (RelexBarEntities dbcontext = new RelexBarEntities())
            {
                dbcontext.OtherPayServiceLog.FirstOrDefault(m => m.ID == ID).Status = status;
                return dbcontext.SaveChanges();

            }
        }

        public int PaySuccessNotify(string number, string otherNumber, decimal payPrice)
        {
            using (RelexBarEntities dbcontext = new RelexBarEntities())
            {
                var paylog = dbcontext.OtherPayServiceLog.FirstOrDefault(m => m.OrderNumber == otherNumber && m.Status == (int)enOrderStatus.Order);
                if (paylog != null)
                {
                    if (paylog.OrderType == (int)enPayFrom.RedPaged)//发红包
                    {
                        WeChat.Log.Info("修改日志状态", UpdateOtherPayServiceLogStatus(paylog.ID, (int)enOrderStatus.Payed).ToString()); //更改日志状态

                        RedPacksBLL bll = new RedPacksBLL();

                        WeChat.Log.Info("修改发红包激活状态", bll.UpdateRedPacketStatus(paylog.TOID).ToString()); //更改发红包激活状态

                        WeChat.Log.Info("修改发红包领取激活状态", bll.UpdateRedPacketListStatus(paylog.TOID).ToString()); //更改发红包激活状态

                        return 1;
                    }
                }
                return -1;
            }
        }

    }
}
