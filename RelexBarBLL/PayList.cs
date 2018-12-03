using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Data.SqlClient;
using RelexBarBLL.Models;
using System.Web;

namespace RelexBarBLL
{
    public partial class PayListBLL : BaseBll
    {

        public List<PayList> GetPayList(int index, int pageSize, DateTime? time, Guid uid)
        {
            using (DBContext)
            {
                var q = DBContext.PayList.Where(m => m.UID == uid);
                if (time != null)
                {
                    q.Where(m => m.CreateTime < time);
                }
                return q.OrderByDescending(m => m.CreateTime).Skip((index - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取账单
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="FromTo"></param>
        /// <param name="InOut"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<AdminPayListModel> GetPayList(string phone, enPayFrom? FromTo, enPayInOutType? InOut, enPayType? PayType, DateTime? beginTime, DateTime? endTime, string remark, int index, int pageSize, out int sum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from(select  ROW_NUMBER() over(order by p.createtime desc) row_number,p.UID,u.Phone,u.Email,u.HeadImg1 HeadImg,u.CardNumber,p.CID OUID,o.Phone OPhone,o.Name OName,u.Name,p.InOut,p.FromTo,p.Val,p.Remark,p.CreateTime,p.PayType ");
            sql.Append("from PayList p left join Users u on p.UID=u.ID ");
            sql.Append("left join Users o on p.CID=o.ID ");
            StringBuilder tj = new StringBuilder();
            sql.Append("where 1=1 ");
            if (FromTo != null)
            {
                if (FromTo == enPayFrom.Reward)
                {
                    // string remark;
                    remark = "福包分润";
                    tj.Append("and p.Remark like @remark or (p.Remark like @remark and p.FromTo=" + (int)FromTo.Value + ")");
                }
                else if (FromTo == enPayFrom.Commission)
                {
                    remark = "奖励";
                    tj.Append("and p.Remark like @remark or (p.Remark like @remark and p.FromTo=" + (int)FromTo.Value + ")");
                    // tj.Append("and p.FromTo=" + (int)FromTo.Value);
                }
                else
                {
                    tj.Append("and p.FromTo=" + (int)FromTo.Value);
                }
            }
            if (InOut != null)
            {
                tj.Append("and p.InOut=" + (int)InOut.Value);
            }
            if (PayType != null)
            {
                tj.Append("and p.PayType=" + (int)PayType.Value);
            }
            if (string.IsNullOrEmpty(phone))
            {
                tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and u.Phone like @phone ");
            }
            if (beginTime != null)
            {
                tj.Append(" and p.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            }
            if (endTime != null)
            {
                tj.Append(" and p.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            }

            string sqlend = " ) as t where t.row_number > @min and t.row_number <= @max";
            using (DBContext)
            {
                sum = DBContext.Database.SqlQuery<int>("select count(p.UID) from PayList p left join Users u on p.UID=u.ID where 1=1 " + tj.ToString(), new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%"),
                    new SqlParameter("@remark","%"+remark+"%")
                }).FirstOrDefault();
                return DBContext.Database.SqlQuery<AdminPayListModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%"),
                    new SqlParameter("@remark","%"+remark+"%"),
                    new SqlParameter("@min",(index-1)*pageSize),
                    new SqlParameter("@max",index*pageSize)
                }).ToList();
            }
        }

        public dynamic GetPayList(Guid userid, DateTime? begin, DateTime? end, enPayFrom? FromTo, enPayInOutType? InOut, enPayType? PayType
            , int pagesize, int pageinex, out int count, ref decimal inVal, ref decimal outVal, ref decimal totalVal)
        {
            using (DBContext)
            {
                var q = DBContext.PayList.Where(m => m.UID == userid);
                if (FromTo.HasValue)
                {
                    q = q.Where(m => m.FromTo == (int)FromTo);
                }
                if (begin.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value >= begin);
                }
                if (end.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value <= end);
                }
                if (InOut.HasValue)
                {
                    q = q.Where(m => m.InOut == (int)InOut);
                }
                if (PayType.HasValue)
                {
                    q = q.Where(m => m.PayType == (int)PayType);
                }
                var ttval = q.Sum(m => (decimal?)m.Val);
                if (ttval.HasValue)
                {
                    totalVal = ttval.Value;
                    var inval = q.Where(m => m.InOut == (int)enPayInOutType.In).Sum(m => (decimal?)m.Val);
                    var outval = q.Where(m => m.InOut == (int)enPayInOutType.Out).Sum(m => (decimal?)m.Val);
                    if (inval.HasValue)
                        inVal = inval.Value;
                    if (outval.HasValue)
                        outVal = outval.Value;
                }
                return GetPagedList(q.Select(m => new { m.ID, m.InOut, m.Val, m.Remark, m.CreateTime }).OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }
        /// <summary>
        /// 今日收入
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="FromTo"></param>
        /// <param name="InOut"></param>
        /// <param name="PayType"></param>
        /// <returns></returns>
        public decimal TotalPays(Guid userid, DateTime? begin, DateTime? end, enPayFrom? FromTo, enPayInOutType? InOut, enPayType? PayType)
        {
            using (DBContext)
            {
                //收入只算领取红包，奖励，佣金
                var q = DBContext.PayList.Where(m => m.UID == userid &&
                                                    (m.FromTo == (int)enPayFrom.Commission
                                                        || m.FromTo == (int)enPayFrom.Reward
                                                        || m.FromTo == (int)enPayFrom.RedPaged));
                if (FromTo.HasValue)
                {
                    q = q.Where(m => m.FromTo == (int)FromTo);
                }
                if (begin.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value >= begin);
                }
                if (end.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value <= end);
                }
                if (InOut.HasValue)
                {
                    q = q.Where(m => m.InOut == (int)InOut);
                }
                if (PayType.HasValue)
                {
                    q = q.Where(m => m.PayType == (int)PayType);
                }
                decimal? total = q.Sum(m => (decimal?)m.Val);

                return total.HasValue ? total.Value : 0;
            }
        }

        /// <summary>
        /// 获取今日收入
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="FromTo"></param>
        /// <param name="InOut"></param>
        /// <param name="PayType"></param>
        /// <returns></returns>
        public decimal TotalPaysOutExchange(Guid userid, DateTime? begin, DateTime? end, enPayFrom? FromTo, enPayInOutType? InOut, enPayType? PayType)
        {
            using (DBContext)
            {
                //收入只算领取红包，奖励，佣金
                var q = DBContext.PayList.Where(m => m.UID == userid &&
                                                    (m.FromTo == (int)enPayFrom.Commission
                                                        || m.FromTo == (int)enPayFrom.Reward
                                                        || m.FromTo == (int)enPayFrom.RedPaged));
                //if (FromTo.HasValue)
                //{
                //    q = q.Where(m => m.FromTo == (int)FromTo);
                //}
                if (begin.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value >= begin);
                }
                if (end.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value <= end);
                }
                if (InOut.HasValue)
                {
                    q = q.Where(m => m.InOut == (int)InOut);
                }
                if (PayType.HasValue)
                {
                    q = q.Where(m => m.PayType == (int)PayType);
                }

                decimal? total = q.Sum(m => (decimal?)m.Val);

                return total.HasValue ? total.Value : 0;
            }
        }

        public PayList Details(int? ID, Guid? UID, Guid? CID)
        {
            using (DBContext)
            {
                var q = DBContext.PayList.AsEnumerable();
                if (ID.HasValue)
                {
                    q = q.Where(m => m.ID == ID.Value);
                }
                if (UID.HasValue)
                {
                    q = q.Where(m => m.UID == UID.Value);
                }
                if (CID.HasValue)
                {
                    q = q.Where(m => m.CID == CID.Value);
                }
                return q.FirstOrDefault();
            }
        }

        /// <summary>
        /// 详情，包括关联熟悉
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public dynamic Detail(int ID)
        {
            using (DBContext)
            {
                var model = DBContext.PayList.FirstOrDefault(m => m.ID == ID);
                if (model == null)
                    return null;
                switch ((enPayFrom)model.PayType)
                {
                    case enPayFrom.Exchange://转账
                        #region Exchange
                        return new
                        {
                            model.ID,
                            model.PayType,
                            model.Val,
                            model.UpdateTime,
                            model.Remark,
                        };
                    #endregion
                    case enPayFrom.Transfor://提现
                        var trans = DBContext.TransferOut.FirstOrDefault(m => m.ID == model.CID);
                        #region Exchange
                        return new
                        {
                            model.ID,
                            model.PayType,
                            model.UpdateTime,

                            Remark = trans.Reason,
                            Status = trans.Status,
                            Val = trans.Price,
                            trans.ComPrice,
                            trans.BankAccount,
                            trans.BankName,
                            trans.BankUser,
                            trans.BankZhiHang,
                            trans.ApplyRemark,
                        };
                        #endregion
                }

                return null;
            }
        }

        /// <summary>
        /// 线下支付消费（没有订单，直接给商家增加金额）
        /// </summary>
        /// <returns></returns>
        public int PayForOutline(Guid UID, Guid? ShopID, Guid CID, enPayment Payment, decimal Val)
        {
            using (DBContext)
            {
                var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (user == null)
                {
                    return (int)ErrorCode.账号不存在;
                }
                if (user.UserType != (int)enUserType.User)
                {
                    return (int)ErrorCode.账号类型不正确;
                }
                if (user.Status == (int)enStatus.Unabled)
                {
                    return (int)ErrorCode.账号不可用;
                }
                //支付金额到谁手里，有可能到平台上
                Users shoper = null;
                if (ShopID.HasValue)
                {
                    shoper = DBContext.Users.FirstOrDefault(m => m.ID == ShopID);
                }

                if (shoper != null)
                {
                    shoper.Balance += Val;
                }

                PayList model = new PayList();
                model.CID = CID;
                model.UID = UID;
                model.InOut = (int)enPayInOutType.Out;
                model.PayType = (int)enPayType.Coin;
                model.FromTo = (int)enPayFrom.OutLinePay;
                model.Val = Val;
                model.Remark = "微信消费金额：" + Val;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                DBContext.PayList.Add(model);

                if (shoper != null)
                {
                    PayList model2 = new PayList();
                    model2.CID = CID;
                    model2.UID = ShopID;
                    model2.InOut = (int)enPayInOutType.In;
                    model2.PayType = (int)enPayType.Coin;
                    model2.FromTo = (int)enPayFrom.OutLinePay;
                    model2.Val = Val;
                    model2.Remark = "微信收款：" + Val + "--" + user.Phone;
                    model2.Status = (int)enStatus.Enabled;
                    model2.CreateTime = model2.UpdateTime = DateTime.Now;
                    DBContext.PayList.Add(model2);
                }

                return DBContext.SaveChanges() > 0 ? 1 : (int)ErrorCode.数据库操作失败;
            }
        }

        /// <summary>
        /// 本地支付消费（没有订单，直接消费扣除金额）
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="PayType"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public int PayForLocal(Guid UID, Guid? ShopID, Guid CID, enPayType PayType, decimal Val)
        {
            using (DBContext)
            {
                var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (user == null)
                {
                    return (int)ErrorCode.账号不存在;
                }
                if (user.UserType != (int)enUserType.User)
                {
                    return (int)ErrorCode.账号类型不正确;
                }
                if (user.Status == (int)enStatus.Unabled)
                {
                    return (int)ErrorCode.账号不可用;
                }
                //支付金额到谁手里，有可能到平台上
                Users shoper = null;
                if (ShopID.HasValue)
                {
                    shoper = DBContext.Users.FirstOrDefault(m => m.ID == ShopID);
                }

                if (shoper != null)
                {
                    if (PayType == enPayType.Coin)
                    {
                        if (user.Balance < Val)
                        {
                            return (int)ErrorCode.账户余额不足;
                        }
                        user.Balance -= Val;
                        shoper.Balance += Val;
                    }
                    else
                    {
                        if (user.Score < Val)
                        {
                            return (int)ErrorCode.账户积分不足;
                        }
                        user.Score -= Val;
                        shoper.Score += Val;
                        shoper.TotalScore += Val;
                    }
                }
                else
                {

                    if (PayType == enPayType.Coin)
                    {
                        if (user.Balance < Val)
                        {
                            return (int)ErrorCode.账户余额不足;
                        }
                        user.Balance -= Val;
                    }
                    else
                    {
                        if (user.Score < Val)
                        {
                            return (int)ErrorCode.账户积分不足;
                        }
                        user.Score -= Val;
                    }
                }

                PayList model = new PayList();
                model.CID = CID;
                model.UID = UID;
                model.InOut = (int)enPayInOutType.Out;
                model.PayType = (int)PayType;
                model.FromTo = (int)enPayFrom.OutLinePay;
                model.Val = Val;
                model.Remark = "线下消费金额：" + Val;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                DBContext.PayList.Add(model);

                if (shoper != null)
                {
                    PayList model2 = new PayList();
                    model2.CID = CID;
                    model2.UID = ShopID;
                    model2.InOut = (int)enPayInOutType.In;
                    model2.PayType = (int)PayType;
                    model2.FromTo = (int)enPayFrom.OutLinePay;
                    model2.Val = Val;
                    model2.Remark = "线下收款：" + Val + "--" + user.Phone;
                    model2.Status = (int)enStatus.Enabled;
                    model2.CreateTime = model2.UpdateTime = DateTime.Now;
                    DBContext.PayList.Add(model2);
                }

                return DBContext.SaveChanges() > 0 ? 1 : (int)ErrorCode.数据库操作失败;
            }
        }

        public int Insert(Guid? CID, Guid? UID, enPayInOutType InOut, enPayType PayType, enPayFrom FromTo, decimal Val, string Remark)
        {
            using (DBContext)
            {
                PayList model = new PayList();
                model.CID = CID;
                model.UID = UID;
                model.InOut = (int)InOut;
                model.PayType = (int)PayType;
                model.FromTo = (int)FromTo;
                model.Val = Val;
                model.Remark = Remark;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;

                DBContext.PayList.Add(model);
                return DBContext.SaveChanges();
            }
        }

        public static void Insert(RelexBarEntities entity, Guid? CID, Guid? UID, enPayInOutType InOut, enPayType PayType, enPayFrom FromTo, decimal Val, string Remark)
        {
            PayList model = new PayList();
            model.CID = CID;
            model.UID = UID;
            model.InOut = (int)InOut;
            model.PayType = (int)PayType;
            model.FromTo = (int)FromTo;
            model.Val = Val;
            model.Remark = Remark;
            model.Status = (int)enStatus.Enabled;
            model.CreateTime = model.UpdateTime = DateTime.Now;

            entity.PayList.Add(model);
        }

        public int Delete(int id)
        {
            using (DBContext)
            {
                var pay = DBContext.PayList.FirstOrDefault(m => m.ID == id);
                if (pay != null)
                {
                    pay.Status = (int)enStatus.Unabled;
                    return DBContext.SaveChanges();
                }
                return 0;
            }
        }

        public dynamic GetPayList(string key, DateTime? bdate, DateTime? edate, enUserType? UserType, enPayFrom? FromTo,
            enPayInOutType? InOut, enPayType? PayType, int pagesize, int pageinex, out int count, out decimal totalprice)
        {
            using (DBContext)
            {
                var q = from a in DBContext.Users
                        from b in DBContext.PayList
                        where a.ID == b.UID
                        select new
                        {
                            UID = a.ID,
                            Phone = a.Phone,
                            CardNumber = a.CardNumber,
                            TrueName = a.TrueName,
                            Score = a.Score,
                            Balance = a.Balance,
                            Status = a.Status,
                            UserType = a.UserType,
                            LV = a.LV,
                            ID = b.ID,
                            CID = b.CID,
                            InOut = b.InOut,
                            PayType = b.PayType,
                            FromTo = b.FromTo,
                            Val = b.Val,
                            Remark = b.Remark,
                            CreateTime = b.CreateTime,
                            UpdateTime = b.UpdateTime,
                        };

                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.TrueName.Contains(key) || m.Phone == key || m.CardNumber.Contains(key) || m.Remark.Contains(key));
                }
                if (FromTo.HasValue)
                {
                    q = q.Where(m => m.FromTo == (int)FromTo);
                }
                if (bdate.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value > bdate.Value);
                }
                if (edate.HasValue)
                {
                    q = q.Where(m => m.CreateTime.Value < edate.Value);
                }
                if (InOut.HasValue)
                {
                    q = q.Where(m => m.InOut == (int)InOut);
                }
                if (PayType.HasValue)
                {
                    q = q.Where(m => m.PayType == (int)PayType);
                }
                if (UserType.HasValue)
                {
                    q = q.Where(m => m.UserType == (int)UserType);
                }
                totalprice = 0;
                decimal? inmoney = q.Where(m => m.InOut == (int)enPayInOutType.In).Sum(m => (decimal?)m.Val);//入账数
                decimal? outmoney = q.Where(m => m.InOut == (int)enPayInOutType.Out).Sum(m => (decimal?)m.Val);//出账数
                totalprice = (inmoney.HasValue ? inmoney.Value : 0) - (outmoney.HasValue ? outmoney.Value : 0);

                return GetPagedList(q.OrderByDescending(m => m.ID), pagesize, pageinex, out count);
            }
        }
        /// <summary>
        /// 获得月份的收入和支出
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="sr"></param>
        /// <param name="zc"></param>
        /// <param name="type">交易类型</param>
        public void GetBillSZ(Guid uid, int year, int month, out decimal sr, out decimal zc, enPayFrom? type)
        {
            using (DBContext)
            {
                StringBuilder tsql = new StringBuilder();
                tsql.Append("select SUM(Val) from PayList where UID=@uid");
                if (type != null)
                {
                    tsql.Append(" and FromTo=" + (int)type);
                }
                tsql.Append(" and CreateTime>=CAST(@begin as datetime) and CreateTime<CAST(@end as datetime) and InOut=" + (int)enPayInOutType.In + " union all");
                tsql.Append(" select SUM(Val) from PayList where UID=@uid");
                if (type != null)
                {
                    tsql.Append(" and FromTo=" + (int)type);
                }
                tsql.Append(" and CreateTime >= CAST(@begin as datetime) and CreateTime< CAST(@end as datetime) and InOut =" + (int)enPayInOutType.Out);

                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@uid",uid),
                    new SqlParameter("@begin",year+"-"+month+"-1"),
                     new SqlParameter("@end",month+1>12?(year+1)+"-1-1":year+"-"+(month+1)+"-1")
                    //new SqlParameter("@begin1",year+"-"+month+"-1"),
                    //new SqlParameter("@end1",year+"-"+(month+1)+"-1")
                };
                List<decimal?> result = DBContext.Database.SqlQuery<decimal?>(tsql.ToString(), param).ToList();
                sr = result[0] == null ? 0 : result[0].Value;
                zc = result[1] == null ? 0 : result[1].Value;
            }
        }
        /// <summary>
        /// 获得账单
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="type">交易类型</param>
        /// <returns></returns>
        public List<BillModel> GetBill(Guid uid, int year, int month, int index, int pageSize, enPayFrom? type)
        {
            using (DBContext)
            {
                StringBuilder tsql = new StringBuilder();
                tsql.Append("select * from (select ROW_NUMBER() over(order by p.createtime desc) row_number,p.Remark,p.Val,p.InOut,p.FromTo,p.CreateTime,u.HeadImg1 headimg,u.Name PayName from");
                tsql.Append(" PayList p left join Users u on p.PayUID=u.ID");
                tsql.Append(" where p.UID=@uid");
                if (type != null)
                {
                    tsql.Append(" and p.FromTo=" + (int)type);
                }
                tsql.Append(" and p.CreateTime >= CAST(@begin as datetime) and p.CreateTime < CAST(@end as datetime)) as t where t.row_number > @min and t.row_number <= @max");

                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@uid",uid),
                    new SqlParameter("@begin",year+"-"+month+"-1"),
                    new SqlParameter("@end",month+1>12?(year+1)+"-1-1":year+"-"+(month+1)+"-1"),
                    new SqlParameter("@min",(index-1)*pageSize),
                    new SqlParameter("@max",index*pageSize)
                };

                return DBContext.Database.SqlQuery<BillModel>(tsql.ToString(), param).ToList();
            }
        }

        public int Recharge(Guid UID, decimal Price, string remark)
        {
            using (DBContext)
            {
                Users user = DBContext.Users.Where(m => m.ID == UID).FirstOrDefault();
                if (user == null)
                {
                    return -1;
                }
                user.Balance += Price;
                user.UpdateTime = DateTime.Now;
                PayList p = new PayList();
                p.CID = null;
                p.UID = UID;
                p.InOut = (int)enPayInOutType.In;
                p.PayType = (int)enPayType.Coin;
                p.FromTo = (int)enPayFrom.Recharge;
                p.Val = Price;
                p.Remark = remark;
                p.Status = (int)enStatus.Enabled;
                p.CreateTime = DateTime.Now;
                p.UpdateTime = DateTime.Now;
                DBContext.PayList.Add(p);
                int i = DBContext.SaveChanges();
                return i;
            }
        }

        public int StoredValue(Guid UID, decimal Price, string remark, int type)
        {
            using (DBContext)
            {
                Users user = DBContext.Users.Where(m => m.ID == UID).FirstOrDefault();
                if (user == null)
                {
                    return -1;
                }
                if (type == 1)
                {
                    user.Balance += Price;
                }
                else if (type == 2)
                {
                    user.Balance -= Price;
                }
                user.UpdateTime = DateTime.Now;
                PayList p = new PayList();
                p.CID = null;
                p.UID = UID;
                if (type == 1)
                {
                    p.InOut = (int)enPayInOutType.In;
                }
                else if (type == 2)
                {
                    p.InOut = (int)enPayInOutType.Out;
                }
                p.PayType = (int)enPayType.Coin;
                p.FromTo = (int)enPayFrom.StoredValue;
                p.Val = Price;
                p.Remark = remark;
                p.Status = (int)enStatus.Enabled;
                p.CreateTime = DateTime.Now;
                p.UpdateTime = DateTime.Now;
                DBContext.PayList.Add(p);
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作{1}金额用户成功,用户名称:{2},{1}金额:{3}", (HttpContext.Current.Session["admin"] as AdminUser).Name, type == 1 ? "储值" : "扣除", user.Name + "--" + user.Phone, Price), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {

                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作{1}金额用户失败,用户名称:{2},{1}金额:{3}", (HttpContext.Current.Session["admin"] as AdminUser).Name, type == 1 ? "储值" : "扣除", user.Name + "--" + user.Phone, Price), enLogType.Admin);
                    return 0;
                }


            }
        }

        public Dictionary<String, decimal> GetTotalInOut(DateTime? beginTime, DateTime? endTime)
        {
            using (DBContext)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select ISNULL(SUM(Val),0) from PayList where InOut =1");
                if (beginTime != null)
                {
                    sql.Append(string.Format(" and CreateTime>CONVERT(datetime,'{0}')", beginTime.Value.ToString("yyyy-MM-dd")));
                }
                if (endTime != null)
                {
                    sql.Append(string.Format(" and CreateTime<CONVERT(datetime,'{0}')", beginTime.Value.AddDays(1).ToString("yyyy-MM-dd")));
                }
                sql.Append(" union all select ISNULL(SUM(Val),0) from PayList where InOut =0");
                if (beginTime != null)
                {
                    sql.Append(string.Format(" and CreateTime>CONVERT(datetime,'{0}')", beginTime.Value.ToString("yyyy-MM-dd")));
                }
                if (endTime != null)
                {
                    sql.Append(string.Format(" and CreateTime<CONVERT(datetime,'{0}')", beginTime.Value.AddDays(1).ToString("yyyy-MM-dd")));
                }

                sql.Append(" union all select ISNULL(SUM(Val),0) from PayList where InOut =1 and datediff(week,CreateTime,getdate())=0"); // 7天的收入
                sql.Append(" union all select ISNULL(SUM(Val),0) from PayList where InOut =0 and datediff(month,CreateTime,getdate())=0"); // 本月的支出
                sql.Append(" union all select ISNULL(SUM(Val),0) from PayList where InOut =1 and datediff(month,CreateTime,getdate())=0"); //  本月的收入

                sql.Append(" union all select ISNULL(SUM(TotalPrice),0) from RedPacket");  //发布红包总额
                sql.Append(" union all select ISNULL(SUM(Money),0) from RedPacketList where Status=3");   //抢红包总额

                List<decimal> list = DBContext.Database.SqlQuery<decimal>(sql.ToString()).ToList();
                Dictionary<string, decimal> map = new Dictionary<string, decimal>();
                map.Add("in", list[0]);
                map.Add("out", list[1]);

                map.Add("weekin", list[2]);
                map.Add("monthout", list[3]);
                map.Add("monthin", list[4]);

                map.Add("redout", list[5]);
                map.Add("redin", list[6]);
                return map;
            }
        }

        public void GetFubaoShare(out decimal p, out decimal p2)
        {
            using (DBContext)
            {
                p = DBContext.Users.Where(m => m.Status == (int)enStatus.Enabled).Sum(m => m.RedBalance);
                p = p % RedPacksBLL.UserRedBalance;

                p2 = SysConfigBLL.SystemMoney % SysConfigBLL.SystemAchieveSend;
            }
        }

        /// <summary>
        /// 收支
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<AdminPayListModel> GetPayInComeSearch(string phone, int index, int pageSize, out int sum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from(select  ROW_NUMBER() over(order by p.createtime desc) row_number,p.UID,u.Phone,u.Email,p.CID OUID,o.Phone OPhone,o.Name OName,u.Name,p.InOut,p.FromTo,p.Val,p.Remark,p.CreateTime,p.PayType ");
            sql.Append("from PayList p left join Users u on p.UID=u.ID ");
            sql.Append("left join Users o on p.CID=o.ID ");
            StringBuilder tj = new StringBuilder();
            sql.Append("where p.InOut=" + (int)enPayInOutType.In);
            //if (FromTo != null)
            //{
            //    tj.Append("and p.FromTo=" + (int)FromTo.Value);
            //}
            //if (InOut != null)
            //{
            //    tj.Append("and p.InOut=" + (int)InOut.Value);
            //}
            if (string.IsNullOrEmpty(phone))
            {
                tj.Append(" and ( p.InOut=" + (int)enPayInOutType.In + " or u.Email like @phone) ");
                //tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and u.Email like @phone ");
                //tj.Append(" and u.Phone like @phone ");
            }
            //if (beginTime != null)
            //{
            //    tj.Append(" and p.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            //}
            //if (endTime != null)
            //{
            //    tj.Append(" and p.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            //}

            string sqlend = " ) as t where t.row_number > @min and t.row_number <= @max";
            using (DBContext)
            {
                sum = DBContext.Database.SqlQuery<int>("select count(p.UID) from PayList p left join Users u on p.UID=u.ID where  p.InOut=" + (int)enPayInOutType.In + tj.ToString(), new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%")
                }).FirstOrDefault();
                return DBContext.Database.SqlQuery<AdminPayListModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%"),
                    new SqlParameter("@min",(index-1)*pageSize),
                    new SqlParameter("@max",index*pageSize)
                }).ToList();
            }
        }

        public List<AdminPayListModel> GetPayRewarsSearch(string phone, int index, int pageSize, out int sum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from(select  ROW_NUMBER() over(order by p.createtime desc) row_number,p.UID,u.Phone,u.Email,p.CID OUID,o.Phone OPhone,o.Name OName,u.Name,p.InOut,p.FromTo,p.Val,p.Remark,p.CreateTime,p.PayType ");
            sql.Append("from PayList p left join Users u on p.UID=u.ID ");
            sql.Append("left join Users o on p.CID=o.ID ");
            StringBuilder tj = new StringBuilder();
            sql.Append("where p.FromTo=" + (int)enPayFrom.Commission);
            //if (FromTo != null)
            //{
            //    tj.Append("and p.FromTo=" + (int)FromTo.Value);
            //}
            //if (InOut != null)
            //{
            //    tj.Append("and p.InOut=" + (int)InOut.Value);
            //}
            if (string.IsNullOrEmpty(phone))
            {
                tj.Append(" and ( p.FromTo=" + (int)enPayFrom.Commission + " or u.Email like @phone) ");
                //tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and (u.Phone like @phone or u.Name like @phone) ");
                //tj.Append(" and u.Phone like @phone ");
            }
            //if (beginTime != null)
            //{
            //    tj.Append(" and p.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            //}
            //if (endTime != null)
            //{
            //    tj.Append(" and p.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            //}

            string sqlend = " ) as t where t.row_number > @min and t.row_number <= @max";
            using (DBContext)
            {
                sum = DBContext.Database.SqlQuery<int>("select count(p.UID) from PayList p left join Users u on p.UID=u.ID where  p.FromTo=" + (int)enPayFrom.Commission + tj.ToString(), new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%")
                }).FirstOrDefault();
                return DBContext.Database.SqlQuery<AdminPayListModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%"),
                    new SqlParameter("@min",(index-1)*pageSize),
                    new SqlParameter("@max",index*pageSize)
                }).ToList();
            }
        }

        /// <summary>
        /// 导出所有佣金记录
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public List<AdminPayListModel> GetAllPayRewarsList(string phone, DateTime? beginTime, DateTime? endTime)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from(select  ROW_NUMBER() over(order by p.createtime desc) row_number,p.UID,u.Phone,u.Email,p.CID OUID,o.Phone OPhone,o.Name OName,u.Name,p.InOut,p.FromTo,p.Val,p.Remark,p.CreateTime,p.PayType ");
            sql.Append("from PayList p left join Users u on p.UID=u.ID ");
            sql.Append("left join Users o on p.CID=o.ID ");
            StringBuilder tj = new StringBuilder();
            sql.Append("where p.FromTo=" + (int)enPayFrom.Commission);

            if (string.IsNullOrEmpty(phone))
            {
                tj.Append(" and ( p.FromTo=" + (int)enPayFrom.Commission + " or u.Email like @phone) ");
                //tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and (u.Phone like @phone or u.Name like @phone) ");
                //tj.Append(" and u.Phone like @phone ");
            }
            if (beginTime != null)
            {
                tj.Append(" and p.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            }
            if (endTime != null)
            {
                tj.Append(" and p.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            }
            string sqlend = " ) as t";
            using (DBContext)
            {
                return DBContext.Database.SqlQuery<AdminPayListModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%")
                }).ToList();
            }
        }

        public List<AdminPayListModel> GetAllPayList(string phone, enPayFrom? FromTo, enPayInOutType? InOut, enPayType? PayType, DateTime? beginTime, DateTime? endTime)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from(select  ROW_NUMBER() over(order by p.createtime desc) row_number,p.UID,u.Phone,u.Email,p.CID OUID,o.Phone OPhone,o.Name OName,u.Name,p.InOut,p.FromTo,p.Val,p.Remark,p.CreateTime,p.PayType ");
            sql.Append("from PayList p left join Users u on p.UID=u.ID ");
            sql.Append("left join Users o on p.CID=o.ID ");
            StringBuilder tj = new StringBuilder();
            sql.Append("where 1=1 ");
            if (FromTo != null)
            {
                tj.Append("and p.FromTo=" + (int)FromTo.Value);
            }
            if (InOut != null)
            {
                tj.Append("and p.InOut=" + (int)InOut.Value);
            }
            if (PayType != null)
            {
                tj.Append("and p.PayType=" + (int)PayType.Value);
            }
            if (string.IsNullOrEmpty(phone))
            {
                tj.Append(" and (1=1 or u.Phone like @phone) ");
                //tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and u.Phone like @phone ");
                //tj.Append(" and u.Phone like @phone ");
            }
            if (beginTime != null)
            {
                tj.Append(" and p.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            }
            if (endTime != null)
            {
                tj.Append(" and p.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            }

            string sqlend = " ) as t";
            using (DBContext)
            {

                return DBContext.Database.SqlQuery<AdminPayListModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%")
                }).ToList();
            }
        }

        /// <summary>
        /// 获取充值记录
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        //public dynamic GetRechargeList(Guid uid, int pageindex, int pageSize, out int count)
        //{
        //    using (DBContext)
        //    {
        //        var q = from a in DBContext.PayList
        //                from b in DBContext.Users
        //                where a.UID == b.ID &&  a.FromTo==(int)enPayFrom.StoredValue && a.Status == (int)enStatus.Enabled
        //                orderby a.CreateTime descending
        //                select new
        //                {
        //                    a.ID,
        //                    a.UpdateTime,
        //                    a.Val,
        //                    a.PayType,
        //                    a.CreateTime,
        //                    UID = b.ID,
        //                    b.TrueName,
        //                    b.HeadImg1,
        //                    b.Phone,
        //                    b.UserType,
        //                    b.Descrition,
        //                };

        //        return GetPagedList(q, pageSize, pageindex, out count);
        //    }
        //}

        //充值
        public List<PayList> GetRechargeList(Guid UID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.PayList.Where(m => m.UID == UID && m.Status == (int)enStatus.Enabled && m.FromTo == (int)enPayFrom.Recharge).OrderByDescending(m => m.CreateTime);
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }
        //储值
        public List<PayList> GetStoredList(Guid UID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.PayList.Where(m => m.UID == UID && m.Status == (int)enStatus.Enabled && m.FromTo == (int)enPayFrom.StoredValue).OrderByDescending(m => m.CreateTime);
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        //转账
        public List<PayList> GetExchangeList(Guid UID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.PayList.Where(m => m.UID == UID && m.Status == (int)enStatus.Enabled && m.FromTo == (int)enPayFrom.Exchange).OrderByDescending(m => m.CreateTime);
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

    }
}
