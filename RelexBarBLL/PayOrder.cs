using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    public enum enPayOrderStatus
    {
        Create = 0,
        Payed = 1,
    }
    public class PayOrderBLL : BaseBll
    {
        public PayOrder Get(Guid id)
        {
            using (DBContext)
            {
                return DBContext.PayOrder.FirstOrDefault(m => m.ID == id);
            }
        }

        public int Pay(Guid UID, Guid OID, string payPwd, out Guid orderUID, ref decimal price)
        {
            using (DBContext)
            {
                PayOrder o = DBContext.PayOrder.FirstOrDefault(m => m.ID == OID);
                if (o == null)
                {
                    orderUID = Guid.Empty;
                    price = 0;
                    return (int)ErrorCode.订单不存在;
                }
                orderUID = o.UID;
                if (o.Price > 0)//大于0，表示有金额，否则表示由接收方设置金额
                    price = o.Price;
                else
                    o.Price = price;
                if (o.Status != (int)enPayOrderStatus.Create)
                {
                    return (int)ErrorCode.订单已支付;
                }
                if (UID == o.UID)
                {
                    return (int)ErrorCode.参数错误;
                }
                string md5Paypsw = MD5(payPwd);
                Users u = DBContext.Users.FirstOrDefault(m => m.ID == UID && m.PayPsw == md5Paypsw);//付款人
                if (u == null)
                {
                    return (int)ErrorCode.支付密码错误;
                }
                if (u.Balance < price)
                {
                    return (int)ErrorCode.账户余额不足;
                }
                Users u2 = DBContext.Users.FirstOrDefault(m => m.ID == o.UID);//收款人
                if (u2 == null)
                {
                    return (int)ErrorCode.数据状态异常;
                }
                u.Balance -= price;
                PayList p = new PayList();
                p.CID = o.ID;
                p.UID = u.ID;
                p.InOut = (int)enPayInOutType.Out;
                p.PayType = (int)enPayType.Coin;
                p.FromTo = (int)enPayFrom.OutLinePay;
                p.Val = price;
                p.Remark = string.Format("支付给：{0} 金额：{1}", u.Name, price);
                DBContext.PayList.Add(p);
                u2.Balance += price;
                PayList p2 = new PayList();
                p2.CID = o.ID;
                p2.UID = u2.ID;
                p2.InOut = (int)enPayInOutType.In;
                p2.PayType = (int)enPayType.Coin;
                p2.FromTo = (int)enPayFrom.OutLinePay;
                p.Val = price;
                p.Remark = string.Format("收到：{0}付款，金额：{1}", u2.Name, price);
                DBContext.PayList.Add(p2);
                o.Status = (int)enPayOrderStatus.Payed;
                return DBContext.SaveChanges();

            }
        }

        /// <summary>
        /// 二维码扫码付款生成订单
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public PayOrder CreateDefaultOrder(Guid uid, decimal price)
        {
            using (DBContext)
            {
                PayOrder o = new PayOrder();
                o.ID = Guid.NewGuid();
                o.UID = uid;
                o.Price = price;
                o.Status = (int)enPayOrderStatus.Create;
                o.CreateTime = o.UpdateTime = DateTime.Now;
                DBContext.PayOrder.Add(o);
                int i = DBContext.SaveChanges();
                if (i > 0)
                {
                    return o;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
