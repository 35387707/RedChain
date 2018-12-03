using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    //status 0：创建，1：支付成功，2通知成功
    public class ServiceOrderBLL:BaseBll
    {
        public int NotifySuccess(Guid id) {
            using (DBContext) {
                ServiceOrder o = DBContext.ServiceOrder.FirstOrDefault(m => m.ID == id);
                if (o != null)
                {
                    o.Status = 2;
                    return DBContext.SaveChanges();
                }
                else {
                    return 1;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ServiceOrder.id</param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public ServiceOrder Pay(Guid id,Guid uid,string pwd,out ErrorCode err) {
            using (DBContext) {
                ServiceOrder o = DBContext.ServiceOrder.Where(m => m.ID == id && m.UID == uid).FirstOrDefault();
                if (o==null) {
                    err = ErrorCode.订单不存在;
                    return null;
                }
                Users u = DBContext.Users.FirstOrDefault(m => m.ID == o.UID);
                if (u.PayPsw != MD5(pwd))
                {
                    err = ErrorCode.支付密码错误;
                    return null;
                }
                if (o.Status==1||o.Status==2) {//支付成功或者通知成功
                    err = ErrorCode.没有错误;
                    return o;
                }
                
                if (u==null) {
                    err = ErrorCode.账号不存在;
                    return null;
                }
                if (u.Balance<o.Price) {
                    err = ErrorCode.账户余额不足;
                    return null;
                }
                u.Balance -= o.Price;
                u.UpdateTime = DateTime.Now;
                o.Status = 1;

                PayListBLL.Insert(DBContext, o.ID, o.UID, enPayInOutType.Out, enPayType.Coin, enPayFrom.OtherPay, o.Price, o.Title);
                int i= DBContext.SaveChanges();
                if (i > 0)
                {
                    err = ErrorCode.没有错误;
                    return o;
                }
                else {
                    err = ErrorCode.充值失败;
                    return null;
                }
            }
        }
        public ServiceOrder CreatePayOrder(string token,string AppID, string orderNumber, string title, string body, decimal price, string notify_url,
            string return_url, string sign,out ErrorCode err,out Users user) {
            
            err = ErrorCode.没有错误;
            user = null;
            //通过appid获取key
            ServiceList server = DBContext.ServiceList.Where(m => m.AppID == AppID).FirstOrDefault();
            if (string.IsNullOrEmpty(server.AppSecret))
            {
                err = ErrorCode.签名验证失败;
                return null;
            }
            Utils.GCData d = new Utils.GCData();
            d.SetValue("AppID", AppID);
            d.SetValue("orderNumber", orderNumber);
            d.SetValue("title", title);
            d.SetValue("body", body);
            d.SetValue("price", price);
            d.SetValue("token",token);
            d.SetValue("notify_url", notify_url);
            d.SetValue("return_url", return_url);
            if (sign != d.MakeSign(server.AppSecret))
            {
                err = ErrorCode.签名验证失败;
                return null;
            }

            UsersBLL ubll = new UsersBLL();
            OtherLoginToken t = DBContext.OtherLoginToken.FirstOrDefault(m => m.Token == token);
            if (t == null)
            {
                err = ErrorCode.Token无效;
                return null;
            }
            if (t.ExpiredTime < DateTime.Now)
            {
                err = ErrorCode.Token无效;
                return null;
            }
            user = DBContext.Users.FirstOrDefault(m => m.ID == t.UID);
            if (user == null)
            {
                return null;
            }


            ServiceOrder o= DBContext.ServiceOrder.Where(m => m.ServiceID == server.ID && m.OrderNumber == orderNumber).FirstOrDefault();
            if (o!=null) {
                    return o;
            }

            o = new ServiceOrder();
            o.ID = Guid.NewGuid();
            o.ServiceID = server.ID;
            o.OrderNumber = orderNumber;
            o.Title = title;
            o.Body = body;
            o.Price = price;
            o.Remark = string.Empty;
            o.Status = 0;
            o.UID = user.ID;
            o.Notify_url = notify_url;
            o.Return_url = return_url;
            o.CreateTime = o.UpdateTime = DateTime.Now;
            DBContext.ServiceOrder.Add(o);
            int i = DBContext.SaveChanges();
            if (i > 0)
            {
                return o;
            }
            else {
                err = ErrorCode.订单创建失败;
                return null;
            }
        }
    }
}
