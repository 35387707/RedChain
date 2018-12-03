using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    /// <summary>
    /// 报表
    /// </summary>
    public class ReportBLL : BaseBll
    {
        public int GetUserTotal(enUserType? UserType)
        {
            using (DBContext)
            {
                if (UserType == null)
                {
                    return DBContext.Users.Count();
                }
                else if (UserType == enUserType.User)
                {
                    return DBContext.Users.Count(m => m.UserType == (int)enUserType.User);
                }
                else if (UserType == enUserType.Shop)
                {
                    return DBContext.Users.Count(m => m.UserType == (int)enUserType.Shop);
                }
                else if (UserType == enUserType.Agent)
                {
                    return DBContext.Users.Count(m => m.UserType == (int)enUserType.Agent);
                }
                return 0;
            }
        }
        public int GetYesterDayUser(DateTime date)
        {
            using (DBContext)
            {
                date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                DateTime dtend = date.AddDays(1);
                return DBContext.Users.Count(m => m.CreateTime.Value > date && m.CreateTime.Value < dtend);
            }
        }

       

        public int GetUserTypeByDate(DateTime date,enUserType? UserType)
        {
            using (DBContext)
            {
                if (UserType == null)
                {
                    date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                    DateTime dtend = date.AddDays(1);
                    return DBContext.Users.Count(m => m.CreateTime.Value > date && m.CreateTime.Value < dtend);
                }
                else
                {
                    date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                    DateTime dtend = date.AddDays(1);
                    return DBContext.Users.Count(m => m.UserType == (int)UserType && m.CreateTime.Value > date && m.CreateTime.Value < dtend);
                }
            }
        }

        public int GetLoginCount(DateTime date)
        {
            using (DBContext)
            {
                date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                DateTime dtend = date.AddDays(1);
                return DBContext.Logs.Where(m => m.CreateTime.Value > date && m.CreateTime.Value < dtend && m.LogType==(int)enLogType.Login).Select(m=>new {UID=m.UID }).GroupBy(m=>m.UID).Count();
            }
        }
        public List<Report> GetList(enReportType Type,int Top) {
            using (DBContext) {
                return DBContext.Report.OrderByDescending(m => m.CreateTime).Take(Top).ToList();
            }
        }
        public int Insert(decimal value,enReportType Type,DateTime? Date) {
            using (DBContext) {
                Report r = new Report();
                r.Name = Type.ToString();
                r.Value = value;
                r.CountDate = 3;
                r.Type = ((int)Type).ToString();
                r.CreateTime =Date==null?DateTime.Now:Date.Value;
                DBContext.Report.Add(r);
                return DBContext.SaveChanges();
            }
        }
        public int GetPayedOrder()
        {
            using (DBContext)
            {
                return DBContext.OrderList.Count(m => m.Status == (int)enOrderStatus.Payed);
            }
        }

        public decimal TotalPays(enPayFrom? FromTo, enPayInOutType? InOut, enPayType? PayType)
        {
            using (DBContext)
            {
                if (FromTo == enPayFrom.RedPaged)
                {
                    var q = DBContext.PayList.Where(m => m.FromTo == (int)FromTo && m.InOut == (int)InOut && m.PayType == (int)PayType);
                    var others = DBContext.OtherPayServiceLog.Where(m => m.OrderType == (int)FromTo && m.Status ==(int)enOrderStatus.Payed);
                    decimal? total = q.Sum(m => (decimal?)m.Val);
                    decimal? otherprice = others.Sum(m => (decimal?)m.PayPrice);
                    decimal all = 0;
                    if (total.HasValue)
                        all += total.Value;
                    if (otherprice.HasValue)
                        all += otherprice.Value;
                    return all;
                }
                else
                {
                    var q = DBContext.PayList.Where(m => m.FromTo == (int)FromTo && m.InOut == (int)InOut && m.PayType == (int)PayType);
                    decimal? total = q.Sum(m => (decimal?)m.Val);
                    return total.HasValue ? total.Value : 0;
                }
            }
        }

        /// <summary>
        /// 获取福包总数
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public int GetRedpackByRedType(enRedType? Type)
        {
            using (DBContext)
            {
                int? Count = 0;
                int total = 0;
                if(Type==null)
                {
                    Count = DBContext.RedPacket.Where(m => m.RedType != (int)enRedType.Single).Sum(m => (int?)m.OncePacketCount);  //系统福包
                    total = (Count.HasValue ? Count.Value : 0);
                }
                else
                {
                    if(Type==enRedType.Auto_SystemAchieveSend||Type==enRedType.Auto_UserRecRed)
                    {
                        Count = DBContext.RedPacket.Where(m => m.RedType == (int)enRedType.Auto_SystemAchieveSend || m.RedType==(int)enRedType.Auto_UserRecRed || m.RedType == (int)enRedType.Auto_NewAgentSend).Sum(m => (int?)m.OncePacketCount);
                        total = (Count.HasValue ? Count.Value : 0);
                    }
                    else
                    {
                        Count = DBContext.RedPacket.Where(m => m.RedType == (int)Type).Sum(m => (int?)m.OncePacketCount);
                        total = (Count.HasValue ? Count.Value : 0);
                    }
                   
                }
               
                return total;
            }
        }

        public decimal GetRedpackTotalPriceByRedType(enRedType? type)
        {
            using (DBContext)
            {
                decimal? PriceCount;
                decimal totalprice = 0;

                if (type == null)
                {
                    PriceCount = DBContext.RedPacket.Where(m => m.RedType != (int)enRedType.Single).Sum(m => (decimal?)m.TotalPrice); //系统福包
                    totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                }
                else
                {
                    PriceCount = DBContext.RedPacket.Where(m => m.RedType == (int)enRedType.Single).Sum(m => (decimal?)m.TotalPrice); 
                    totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                   
                }
                return totalprice;
            }
        }

        ///获取福包领取或未领取个数 
        public int GetRepackByStatus(enPacketStatus? status,DateTime? date)
        {
            using (DBContext)
            {
                if(date!=null)
                {
                    date = DateTime.Parse(date.Value.ToString("yyyy-MM-dd"));
                    DateTime dtend = date.Value.AddDays(1);
                    return DBContext.RedPacketList.Count(m => m.Status == (int)status && m.CreateTime.Value > date && m.CreateTime.Value < dtend);
              
                }
                else
                {
                    return DBContext.RedPacketList.Count(m => m.Status == (int)status);
                }
               // return 0;
            }
        }

        public decimal GetRepackListMoneyByStatus(enPacketStatus? status, DateTime?date)
        {
            using (DBContext)
            {
                decimal? PriceCount;
                decimal totalprice = 0;
                if (date != null)
                {
                    date = DateTime.Parse(date.Value.ToString("yyyy-MM-dd"));
                    DateTime dtend = date.Value.AddDays(1);
                    PriceCount = DBContext.RedPacketList.Where(m => m.Status == (int)status && m.UpdateTime.Value > date && m.UpdateTime.Value < dtend).Sum(m => (decimal?)m.Money);
                    totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                }
                else
                {
                    PriceCount = DBContext.RedPacketList.Where(m => m.Status == (int)status).Sum(m => (decimal?)m.Money);
                    totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                    
                }
                return totalprice;

            }
               
        }
        
        public int GetPayListCountByFromTo(enPayFrom? FromTo)
        {
            using (DBContext)
            {
                if (FromTo == null)
                {
                    return DBContext.PayList.Count();
                }
             
                return DBContext.PayList.Count(m => m.FromTo == (int)FromTo);

                // return 0;
            }
        }

        public decimal GetPayListValByFromTo(enPayFrom? FromTo)
        {
            using (DBContext)
            {
                decimal? PriceCount;
                decimal totalprice = 0;
                if (FromTo == null)
                {
                    PriceCount = DBContext.PayList.Sum(m => (decimal?)m.Val);
                    totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                }
                else
                {
                    string remark;
                    if (FromTo == enPayFrom.Reward)
                    {
                        remark = "福包分润";
                        PriceCount = DBContext.PayList.Where(m =>(m.FromTo == (int)FromTo && m.Remark.Contains(remark))|| m.Remark.Contains(remark)).Sum(m => (decimal?)m.Val);
                        totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                    }
                    else if(FromTo==enPayFrom.Commission)
                    {
                        remark = "奖励";
                        PriceCount = DBContext.PayList.Where(m => (m.FromTo == (int)FromTo && m.Remark.Contains(remark)) || m.Remark.Contains(remark)).Sum(m => (decimal?)m.Val);
                        totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                    }
                    else
                    {
                        PriceCount = DBContext.PayList.Where(m => m.FromTo == (int)FromTo).Sum(m => (decimal?)m.Val);
                        totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                    }
                   
                }
            
                return totalprice;
            }
        }

        public decimal GetTransferOutValByStatus(enApplyStatus? Status)
        {
            using (DBContext)
            {
                decimal? PriceCount;
                decimal totalprice = 0;
               
                PriceCount = DBContext.TransferOut.Where(m => m.Status == (int)Status).Sum(m => (decimal?)m.Price);
                totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
               
                return totalprice;
            }
        }



        public decimal GetPayListScoreByPayType(enPayType? Type, DateTime? date)
        {
            using (DBContext)
            {
                decimal? PriceCount;
                decimal totalprice = 0;
                if (date != null)
                {
                    date = DateTime.Parse(date.Value.ToString("yyyy-MM-dd"));
                    DateTime dtend = date.Value.AddDays(1);  // && m.FromTo == (int)enPayFrom.RedPaged && m.InOut == (int)enPayInOutType.In
                    PriceCount = DBContext.PayList.Where(m => m.PayType == (int)Type && m.CreateTime.Value > date && m.CreateTime.Value < dtend).Sum(m => (decimal?)m.Val);
                    totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                   
                }
                else
                {
                    PriceCount = DBContext.PayList.Where(m => m.PayType == (int)Type).Sum(m => (decimal?)m.Val);
                    totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);

                  
                }
                return totalprice;
            }
        }

        /// <summary>
        /// 发布广告总计
        /// </summary>
        /// <returns></returns>
        public int GetInfomationsTotal()
        {
            using (DBContext)
            {
                 return DBContext.Infomations.Count();
            }
        }

        public decimal GetPayListInfomationsScoreByPayType(enPayType? Type)
        {
            using (DBContext)
            {
                decimal? PriceCount;
                decimal totalprice = 0;

                PriceCount = DBContext.PayList.Where(m => m.PayType == (int)Type && m.FromTo == (int)enPayFrom.Infomations && m.InOut == (int)enPayInOutType.In).Sum(m => (decimal?)m.Val);
                totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                return totalprice;
            }
        }


        /// <summary>
        /// 获取登录数据
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<Report> GetLoginReport(int day)
        {
            using (DBContext)
            {
                return DBContext.Report.Where(m => m.Name == enReportType.Login.ToString()).OrderByDescending(m => m.CreateTime).Take(day).ToList();
            }
        }


        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<Report> GetReport(int count, enReportType type)
        {
            using (DBContext)
            {
                string typename = type.ToString();
                return DBContext.Report.Where(m => m.Name == typename).OrderByDescending(m => m.CreateTime).Take(count).ToList();
            }
        }


        #region 订单成交额

        //付款订单数
        public int GetOrderPayCount(DateTime date)
        {
            using (DBContext)
            {
                date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                DateTime dtend = date.AddDays(1);
                return DBContext.OrderList.Count(m => m.PayTime.Value > date && m.PayTime.Value < dtend);
            }
        }

        //订单金额
        public decimal GetOrderPriceCount(DateTime date)
        {
            using (DBContext)
            {
                date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                DateTime dtend = date.AddDays(1);
                //  decimal? inmoney = q.Where(m => m.InOut == (int)enPayInOutType.In).Sum(m => (decimal?)m.Val);//入账数
                //   return DBContext.OrderList.Where(m => m.PayTime.Value > date && m.PayTime.Value < dtend).Select(m => new { Price= m.Price }).GroupBy(m => m.Price).Count();
                //  return DBContext.OrderList.Count(m => m.PayTime.Value > date && m.PayTime.Value < dtend);
               // Model[i].Status == -1) ? "取消" : ((Model[i].Status == 0)
                decimal? PriceCount= DBContext.OrderList.Where(m => m.PayTime.Value > date && m.PayTime.Value < dtend).Sum(m => (decimal?)m.Price);
                // decimal PriceCount2=(PriceCount == null) ? "0" : PriceCount;
                decimal totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);

                return totalprice;
            }
        }
        /// <summary>
        /// 付款金额
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public decimal GetOrderPayPriceTotal(DateTime date)
        {
            using (DBContext)
            {
                date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                DateTime dtend = date.AddDays(1);
                decimal? priceCount = DBContext.OrderList.Where(m => m.PayTime.Value > date && m.PayTime < dtend && (m.Status == (int)enOrderStatus.Payed || m.Status == (int)enOrderStatus.Sended || m.Status == (int)enOrderStatus.Recieved || m.Status == (int)enOrderStatus.Completed)).Sum(m => (decimal?)m.Price);
                decimal totalprice = (priceCount.HasValue ? priceCount.Value : 0);
                return totalprice;
            }
        }
        /// <summary>
        /// 下单金额
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public decimal GetOrderPriceTotal(DateTime date)
        {
            using (DBContext)
            {
                date = DateTime.Parse(date.ToString("yyyy-MM-dd"));
                DateTime dtend = date.AddDays(1);
                decimal? PriceCount = DBContext.OrderList.Where(m => m.CreateTime.Value > date && m.CreateTime.Value < dtend).Sum(m => (decimal?)m.Price);

                decimal totalprice = (PriceCount.HasValue ? PriceCount.Value : 0);
                return totalprice;
            }
        }





        #endregion

        /// <summary>
        /// 总收入
        /// </summary>
        /// <returns></returns>
        public decimal OtherPayPrice()
        {
            using (DBContext)
            {
                decimal? PriceCount, PriceCount2;
                decimal Scoretotalprice = 0, OtherTotalprice = 0;

                PriceCount2 = DBContext.OtherPayServiceLog.Where(m => m.Status == (int)enOrderStatus.Payed).Sum(m => (decimal?)m.PayPrice);
                OtherTotalprice = (PriceCount2.HasValue ? PriceCount2.Value : 0);

                return Scoretotalprice + OtherTotalprice;
            }
        }

        /// <summary>
        /// 总收入
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalInComeVal()
        {
            using (DBContext)
            {
                decimal? PriceCount,PriceCount2;
                decimal Scoretotalprice = 0, OtherTotalprice=0;

                PriceCount = DBContext.PayList.Where(m => m.FromTo == (int)enPayFrom.StoredValue && m.InOut==(int)enPayInOutType.In).Sum(m => (decimal?)m.Val);
                Scoretotalprice = (PriceCount.HasValue ? PriceCount.Value : 0);

                PriceCount2 = DBContext.OtherPayServiceLog.Where(m => m.Status==(int)enOrderStatus.Payed).Sum(m => (decimal?)m.PayPrice);
                OtherTotalprice = (PriceCount2.HasValue ? PriceCount2.Value : 0);
              
                return Scoretotalprice+ OtherTotalprice;
            }
        }
        /// <summary>
        /// 总支出
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalOutComeVal()
        {
            using (DBContext)
            {
                decimal? PriceCount, PriceCount2;
                decimal Scoretotalprice = 0, OtherTotalprice = 0;

                PriceCount = DBContext.PayList.Where(m => m.FromTo == (int)enPayFrom.Commission || m.FromTo == (int)enPayFrom.Reward).Sum(m => (decimal?)m.Val);
                Scoretotalprice = (PriceCount.HasValue ? PriceCount.Value : 0);

                PriceCount2 = DBContext.RedPacket.Where(m => m.RedType == (int)enRedType.System || m.RedType == (int)enRedType.Auto_SystemAchieveSend).Sum(m => (decimal?)m.TotalPrice);
                OtherTotalprice = (PriceCount2.HasValue ? PriceCount2.Value : 0);

                //int Count = DBContext.RedPacket.Count(m => m.RedType == (int)enRedType.Auto_SystemAchieveSend);
                //decimal AutoPrice = Count * 60000;

                return Scoretotalprice + OtherTotalprice;
            }
        }
        /// <summary>
        /// 总推荐数
        /// </summary>
        /// <returns></returns>
        public int GetFUIDCount()
        {
            using (DBContext)
            {
                int Count = 0;
                Count = DBContext.Users.Count(m => m.FID != Guid.Empty);
                return Count;
            }
        }
        /// <summary>
        /// 总访问数
        /// </summary>
        /// <returns></returns>
        public int GetLoginCount()
        {
            using (DBContext)
            {
                int Count = 0;
                Count = DBContext.Logs.Count(m => m.LogType==(int)enLogType.Login);
                return Count;
            }
        }
    }
}
