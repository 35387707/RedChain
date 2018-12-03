using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    public partial class RechargeBLL : BaseBll
    {
        /// <summary>
        /// 账号充值
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public int InsertBalance(Guid UID, decimal money)
        {
            using (DBContext)
            {
                var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (user == null)
                    return (int)ErrorCode.账号不存在;
                user.Balance += money;
                int result = DBContext.SaveChanges();
                if (result > 0)
                {
                    PayListBLL paybll = new PayListBLL();
                    paybll.Insert(null, UID, enPayInOutType.In, enPayType.Coin, enPayFrom.Recharge, money, "充值");
                }
                return result;
            }
        }

        /// <summary>
        /// 转入积分
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public int ExchangeToScore(Guid UID, decimal money)
        {
            using (DBContext)
            {
                var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (user == null)
                    return (int)ErrorCode.账号不存在;
                if (user.Balance < money)
                {
                    return (int)ErrorCode.账户余额不足;
                }
                user.Balance -= money;
                user.Score += money;
                user.TotalScore += money;
                int result = DBContext.SaveChanges();
                if (result > 0)
                {
                    PayListBLL paybll = new PayListBLL();
                    paybll.Insert(null, UID, enPayInOutType.Out, enPayType.Coin, enPayFrom.Exchange, money, "转出金额");
                    paybll.Insert(null, UID, enPayInOutType.In, enPayType.Point, enPayFrom.Exchange, money, "转入积分");
                }
                return result;
            }
        }

        /// <summary>
        /// 转账给他人
        /// </summary>
        /// <returns></returns>
        public int ExchangeToOther(Guid FromUID, Guid ToUID, decimal money, enPayType PayType)
        {
            using (DBContext)
            {
                var touser = DBContext.Users.FirstOrDefault(m => m.ID == ToUID);
                if (touser == null)
                    return (int)ErrorCode.账号不存在;
                if (touser.Status == (int)enStatus.Unabled)
                    return (int)ErrorCode.账号不可用;

                var user = DBContext.Users.FirstOrDefault(m => m.ID == FromUID);
                if (user == null)
                    return (int)ErrorCode.账号不存在;

                decimal outMoney, getMoney;//支付金额，获得金额
                //decimal sxf = money * SysConfigBLL.Poundage;
                //sxf = sxf > 2 ? sxf : 2;//转账他人手续费

                outMoney = money;
                getMoney = money;

                if (PayType == enPayType.Coin)
                {
                    if (user.Balance < outMoney)
                        return (int)ErrorCode.账户余额不足;

                    user.Balance -= outMoney;
                    touser.Balance += getMoney;
                }
                if (PayType == enPayType.Point)
                {
                    if (user.Score < outMoney)
                        return (int)ErrorCode.账户积分不足;

                    user.Score -= outMoney;
                    touser.Score += getMoney;
                    touser.TotalScore += getMoney;
                }

                int result = DBContext.SaveChanges();
                if (result > 0)
                {
                    string paytype = (PayType == enPayType.Coin ? "金额" : "积分");
                    PayListBLL paybll = new PayListBLL();
                    UserMsgBLL msgbll = new UserMsgBLL();
                    paybll.Insert(null, FromUID, enPayInOutType.Out, PayType, enPayFrom.Exchange, outMoney, "转出" + paytype + "到" + GetUserShowName(touser));
                    paybll.Insert(null, ToUID, enPayInOutType.In, PayType, enPayFrom.Exchange, getMoney, GetUserShowName(user) + "转入" + paytype);

                    string msgcontent = "您已成功转出" + paytype + outMoney + "给" + GetUserShowName(touser) + ",实际到账" + getMoney + "。";
                    msgbll.Insert(user.ID, Guid.Empty, "转账成功", msgcontent, enMessageType.System, "", msgcontent);
                    msgcontent = GetUserShowName(user) + "转出了" + paytype + outMoney + "给您" + ",实际到账" + getMoney + ")。";
                    msgbll.Insert(touser.ID, Guid.Empty, "转账成功", msgcontent, enMessageType.System, "", msgcontent);
                }
                return result;
            }
        }

        /// <summary>
        /// 用戶儲值
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, decimal> GetRechargeTotal()
        {

            using (DBContext)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(*) from Users where Balance>0");  //储值会员数

                sql.Append(" union all select ISNULL(SUM(Balance),0) from Users");  //储值金额

                sql.Append(" union all  select count(distinct UID) from PayList where FromTo = 0"); // 储值支付会员数

                sql.Append(" union all select ISNULL(SUM(Val),0) from PayList where FromTo = 0");  // 充值金额

                sql.Append(" union all select ISNULL(SUM(Val),0) from PayList where FromTo = 16");//后台储值金额数


                List<decimal> list = DBContext.Database.SqlQuery<decimal>(sql.ToString()).ToList();
                Dictionary<string, decimal> map = new Dictionary<string, decimal>();
                map.Add("BalanceCount", list[0]);
                map.Add("BalanceTotal", list[1]);
                map.Add("BalancePayCount", list[2]);
                map.Add("BalanceRechargeVal", list[3]);
                map.Add("BalanceStoredVal", list[4]);
                return map;
            }
        }



    }
}
