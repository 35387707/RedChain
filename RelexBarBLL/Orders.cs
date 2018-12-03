using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Data.SqlClient;

namespace RelexBarBLL
{
    public partial class OrdersBLL : BaseBll
    {
        public List<OrderList> GetOrderList(Guid userid, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                return GetPagedList(DBContext.OrderList.Where(m => m.UID == userid), pagesize, pageinex, out count);
            }
        }

        public List<vw_Orders> GetOrderList(Guid? userid, string key, enOrderStatus? status, enOrderType? type, DateTime? beginTime, DateTime? endtime
            , enPayment? Payment, enPayType? PriceType, decimal? Price
            , int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.vw_Orders.Where(m => m.CategoryID != 1);

                if (userid.HasValue)
                {
                    q = q.Where(m => m.UID == userid);
                }
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Number == key || m.proName.Contains(key) || m.proNumber == key || m.Uphone == key || m.UTrueName.Contains(key)
                     || m.Remark.Contains(key) || m.RecName.Contains(key) || m.RecPhone == key);
                }
                if (Price.HasValue)
                {
                    q = q.Where(m => m.Price >= Price.Value);
                }
                if (status.HasValue)
                {
                    q = q.Where(m => m.Status == (int)status.Value);
                }
                if (type.HasValue)
                {
                    q = q.Where(m => m.OrderType == (int)type.Value);
                }
                if (beginTime.HasValue)
                {
                    q = q.Where(m => m.CreateTime >= beginTime.Value);
                }
                if (endtime.HasValue)
                {
                    q = q.Where(m => m.CreateTime <= endtime.Value);
                }
                if (Payment.HasValue)
                {
                    q = q.Where(m => m.Payment == Payment.Value.ToString());
                }
                if (PriceType.HasValue)
                {
                    q = q.Where(m => m.PriceType == (int)PriceType.Value);
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }

        public List<OrderList> GetAllList()
        {
            using (DBContext)
            {
                return DBContext.OrderList.ToList();
            }
        }

        public vw_Orders GetDetail(Guid ID)
        {
            using (DBContext)
            {
                return DBContext.vw_Orders.FirstOrDefault(m => m.ID == ID);
            }
        }

        public OrderList GetDetail(string orderNumber)
        {
            using (DBContext)
            {
                return DBContext.OrderList.FirstOrDefault(m => m.Number == orderNumber);
            }
        }

        public OrderList Insert(Guid? ShopID, Guid UID, Guid addressID, Common.enPayment? Payment, Common.enPayType? PriceType,
            decimal Fee, enOrderType OrderType, string Remark, Guid ProID, int count, Guid? productSpecification, out int error)
        {
            using (DBContext)
            {
                //添加订单里的商品
                ProductList pro = new ProductsBLL().GetProduct(ProID);
                if (pro == null || pro.Stock < count)//商品不存在，或者库存不足
                {
                    error = (int)ErrorCode.商品数量不足;
                    return null;
                }
                //if (pro.PriceType == (int)Common.enPayType.All && (!PriceType.HasValue || PriceType.Value == (int)enPayType.All))
                //{
                //    error = (int)ErrorCode.商品不可购买;
                //    return null;
                //}

                //if (productSpecification == null)
                //{
                //    int c = DBContext.ProductSpecification.Count(m => m.ProductID == ProID);
                //    if (c > 0)
                //    {
                //        return null;
                //    }
                //}

                var user = new UsersBLL().GetUserById(UID);
                if (user == null)//人员不存在
                {
                    error = (int)ErrorCode.账号不存在;
                    return null;
                }
                var address = new RecAddressBLL().GetAddressDetail(addressID);
                if (address == null)
                {
                    error = (int)ErrorCode.收货地址不存在;
                    return null;
                }
                if (!Payment.HasValue)
                {
                    Payment = enPayment.LOCAL;
                }
                if (!PriceType.HasValue)
                {
                    PriceType = (Common.enPayType)pro.PriceType;
                }

                pro.Stock -= count;//库存减掉

                //添加订单
                OrderList model = new OrderList();
                model.ID = Guid.NewGuid();
                model.Number = Common.GetOrderNumer();
                model.ShopID = ShopID;
                model.UID = UID;
                model.RecName = address.TrueName;
                model.RecAreaID = string.IsNullOrEmpty(address.AreaID) ? "" : address.AreaID;
                model.RecAddress = string.IsNullOrEmpty(address.Address) ? "" : address.Address;
                model.RecPhone = address.Phone;
                model.RecAreaCode = address.AreaCode;
                model.RecEmail = address.Email;
                model.RecSex = address.Sex;
                model.Payment = Payment.ToString();
                model.PriceType = (int)PriceType;
                model.Price = (pro.Price * count) + Fee;
                model.Fee = Fee;
                model.OrderType = (int)OrderType;
                model.Remark = Remark;
                model.Status = (int)enOrderStatus.Order;
                model.CreateTime = model.UpdateTime = DateTime.Now;

                OrderProductList orderPro = new OrderProductList();
                orderPro.ID = Guid.NewGuid();
                orderPro.OrderID = model.ID;
                orderPro.OrderNumber = model.Number;
                orderPro.ProductID = pro.ID;
                orderPro.Number = pro.Number;
                orderPro.Name = pro.Name;
                orderPro.CategoryID = pro.CategoryID;
                orderPro.Img = pro.Img;
                orderPro.PriceType = pro.PriceType;
                orderPro.Price = pro.Price;
                orderPro.FootQuanPrice = pro.FootQuanPrice;
                orderPro.Count = count;
                orderPro.Type = pro.Type;
                orderPro.BeginTime = pro.BeginTime;
                orderPro.EndTime = pro.EndTime;
                orderPro.Status = pro.Status;
                orderPro.CreateTime = orderPro.UpdateTime = model.CreateTime;

                //if (productSpecification != null)  //暂时写法(商品规格ID,商品ID)
                //{
                //    ProductSpecification spec = DBContext.ProductSpecification.FirstOrDefault(m => m.ID == productSpecification.Value && m.ProductID == ProID);
                //    if (spec == null)   //商品规格错误
                //    {
                //        error = (int)ErrorCode.商品规格不正确;
                //        return null;  //
                //    }
                //    if (spec.Stock < count)  //商品数量不足
                //    {
                //        error = (int)ErrorCode.商品数量不足;
                //        return null;  //
                //    }
                //    spec.SellCount += count;
                //    spec.Stock -= count;
                //    orderPro.ProductSpecification = productSpecification.Value;
                //    orderPro.ProductSpecificationStr = spec.SpecificationAttr;
                //}

                DBContext.OrderList.Add(model);
                DBContext.OrderProductList.Add(orderPro);
                try
                {
                    if (DBContext.SaveChanges() > 0)
                    {
                        error = 1;
                        return model;
                    }
                    else
                    {
                        error = (int)ErrorCode.未知异常;
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    logBll.InsertLog(UID, ex, enLogType.Error);
                    error = (int)ErrorCode.未知异常;
                    return null;
                }
            }
        }

        public List<OrderProductList> GetOrderProList(Guid OrderID)
        {
            using (DBContext)
            {
                return DBContext.OrderProductList.Where(m => m.OrderID == OrderID).ToList();
            }
        }

        public int UpdateStatus(Guid ID, enOrderStatus OrderStatus)
        {
            using (DBContext)
            {
                OrderList model = DBContext.OrderList.FirstOrDefault(m => m.ID == ID);
                if (model != null)
                {
                    if (model.Status.Value == (int)enOrderStatus.Cancel)//已被取消的订单
                    {
                        return (int)ErrorCode.状态异常或已处理;
                    }

                    if (OrderStatus == enOrderStatus.Cancel)//如果是要取消订单
                    {
                        if (model.Status != (int)enOrderStatus.Order)//只有下单状态才能取消
                        {
                            return (int)ErrorCode.状态异常或已处理;
                        }
                    }
                    else
                    {
                        if (model.Status >= (int)OrderStatus)
                            return (int)ErrorCode.状态异常或已处理;
                    }

                    switch (OrderStatus)
                    {
                        case enOrderStatus.Payed:
                            //插入消费记录
                            PayListBLL paybll = new PayListBLL();
                            var user = DBContext.Users.FirstOrDefault(m => m.ID == model.UID);
                            if (model.Payment == enPayment.LOCAL.ToString())//如果是本地支付，则判断金额/积分是否足够
                            {
                                if (model.PriceType == (int)enPayType.Coin)
                                {
                                    if (user.Balance < model.Price)
                                    {
                                        return (int)ErrorCode.账户余额不足;
                                    }
                                    user.Balance -= model.Price;
                                }
                                else if (model.PriceType == (int)enPayType.Point)
                                {
                                    if (user.Score < model.Price)
                                    {
                                        return (int)ErrorCode.账户积分不足;
                                    }
                                    user.Score -= model.Price;
                                }
                                else if (model.PriceType == (int)enPayType.FuQuan)
                                {
                                    if (user.FootQuan < model.Price)
                                    {
                                        return (int)ErrorCode.账户福券不足;
                                    }
                                    user.FootQuan -= model.Price;
                                }
                                else
                                {
                                    return (int)ErrorCode.未知异常;
                                }

                                user.Balance -= model.Price;
                            }

                            var ov = DBContext.vw_Orders.FirstOrDefault(m => m.ID == model.ID);

                            var fqjf = ov.proFootQuanPrice.Value * ov.Count.Value;
                            if (fqjf > 0)
                            {
                                if (user.FootQuan < fqjf)//福券积分多少
                                {
                                    return (int)ErrorCode.账户福券不足;
                                }
                                user.FootQuan -= ov.proFootQuanPrice.Value * ov.Count.Value;

                                PayListBLL.Insert(DBContext, model.ID, model.UID, enPayInOutType.Out, enPayType.FuQuan, enPayFrom.OnLinePay, fqjf, "购物支出");
                            }

                            var pro = DBContext.ProductList.FirstOrDefault(m => m.ID == ov.ProductID);
                            pro.CompleteCount += 1;

                            model.PayTime = DateTime.Now;
                            PayListBLL.Insert(DBContext, model.ID, model.UID, enPayInOutType.Out, (enPayType)model.PriceType, enPayFrom.OnLinePay, model.Price, "购物支出");

                            break;
                        case enOrderStatus.Sended:
                            model.SendTime = DateTime.Now;
                            break;
                        case enOrderStatus.Recieved:
                            model.RecTime = DateTime.Now;
                            break;
                        case enOrderStatus.Completed:
                            model.FinishTime = DateTime.Now;
                            break;
                        case enOrderStatus.Cancel://取消订单，返回库存
                            var ov1 = DBContext.vw_Orders.FirstOrDefault(m => m.ID == model.ID);
                            var pro1 = DBContext.ProductList.FirstOrDefault(m => m.ID == ov1.ProductID);
                            if (pro1 != null)
                            {
                                pro1.Stock += ov1.Count.Value;//加回库存
                            }
                            break;
                    }
                    model.Status = (int)OrderStatus;
                    model.UpdateTime = DateTime.Now;

                    return DBContext.SaveChanges();
                }
                else
                    return 0;
            }
        }

        public List<RelexBarBLL.Models.OrderListModel> GetOrderList(string key, enOrderStatus? Status, DateTime? beginTime, DateTime? endTime, int index, int pageSize, out int sum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from (select  ROW_NUMBER() over(order by o.CreateTime desc) row_number,u.Name,u.TrueName,u.Phone,u.Email,op.Count OCount,op.Price OPrice,op.Name OName,op.ProductSpecificationStr,o.ID,o.UID,o.RecName,o.RecAddress,o.RecPhone, o.Number,o.Price,o.PayTime,o.SendTime,o.RecTime,o.CreateTime,o.Status,o.FinishTime ");
            sql.Append("from OrderList o left join Users u on o.UID=u.ID ");
            sql.Append("left join OrderProductList op on op.OrderID=o.ID ");
            StringBuilder tj = new StringBuilder();
            sql.Append("where 1=1 ");
            if (Status != null)
            {
                tj.Append(" and o.Status=" + (int)Status.Value);
            }

            if (string.IsNullOrEmpty(key))
            {
                tj.Append(" and (1=1 or u.Email like @key) ");
                //tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and op.Name like @key or o.Number like @key");
                //tj.Append(" and u.Phone like @phone ")
            }
            if (beginTime != null)
            {
                tj.Append(" and o.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            }
            if (endTime != null)
            {
                tj.Append(" and o.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            }

            string sqlend = " ) as t where t.row_number > @min and t.row_number <= @max";
            using (DBContext)
            {
                sum = DBContext.Database.SqlQuery<int>("select count(o.UID) from OrderList o left join Users u on o.UID=u.ID left join OrderProductList op on op.OrderID=o.ID where 1=1 " + tj.ToString(), new SqlParameter[] {
                    new SqlParameter("@key","%"+key+"%")
                }).FirstOrDefault();
                return DBContext.Database.SqlQuery<RelexBarBLL.Models.OrderListModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@key","%"+key+"%"),
                    new SqlParameter("@min",(index-1)*pageSize),
                    new SqlParameter("@max",index*pageSize)
                }).ToList();
            }
        }

        public List<Models.OrderListModel> GetOrderListExcel(string key, enOrderStatus? Status, DateTime? beginTime, DateTime? endTime)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from (select  ROW_NUMBER() over(order by o.CreateTime desc) row_number,u.Name,u.TrueName,u.Phone,u.Email,op.Count OCount,op.Price OPrice,op.Name OName,o.ID,o.UID,o.RecName,o.RecAddress,o.RecPhone,o.Number,o.Price,o.PayTime,o.SendTime,o.RecTime,o.CreateTime,o.Status,o.FinishTime ");
            sql.Append("from OrderList o left join Users u on o.UID=u.ID ");
            sql.Append("left join OrderProductList op on op.OrderID=o.ID ");
            StringBuilder tj = new StringBuilder();
            sql.Append("where 1=1 ");
            if (Status != null)
            {
                tj.Append(" and o.Status=" + (int)Status.Value);
            }

            if (string.IsNullOrEmpty(key))
            {
                tj.Append(" and (1=1 or u.Email like @key) ");
                //tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and op.Name like @key or o.Number like @key");
                //tj.Append(" and u.Phone like @phone ")
            }
            if (beginTime != null)
            {
                tj.Append(" and o.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            }
            if (endTime != null)
            {
                tj.Append(" and o.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            }

            string sqlend = " ) as t";
            using (DBContext)
            {

                return DBContext.Database.SqlQuery<RelexBarBLL.Models.OrderListModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@key","%"+key+"%")
                }).ToList();
            }
        }

        public Dictionary<String, decimal> GetPriceTotalOrder(DateTime? beginTime, DateTime? endTime)
        {
            using (DBContext)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(*) from OrderList where datediff(day,CreateTime,getdate())=0"); //今日订单数
                                                                                                         //if (beginTime != null)
                                                                                                         //{
                                                                                                         //    sql.Append(string.Format(" and CreateTime>CONVERT(datetime,'{0}')", beginTime.Value.ToString("yyyy-MM-dd")));
                                                                                                         //}
                                                                                                         //if (endTime != null)
                                                                                                         //{
                                                                                                         //    sql.Append(string.Format(" and CreateTime<CONVERT(datetime,'{0}')", beginTime.Value.AddDays(1).ToString("yyyy-MM-dd")));
                                                                                                         //}
                                                                                                         //  sql.Append(" union all select ISNULL(SUM(Val),0) from PayList where InOut =0");
                                                                                                         //if (beginTime != null)
                                                                                                         //{
                                                                                                         //    sql.Append(string.Format(" and CreateTime>CONVERT(datetime,'{0}')", beginTime.Value.ToString("yyyy-MM-dd")));
                                                                                                         //}
                                                                                                         //if (endTime != null)
                                                                                                         //{
                                                                                                         //    sql.Append(string.Format(" and CreateTime<CONVERT(datetime,'{0}')", beginTime.Value.AddDays(1).ToString("yyyy-MM-dd")));
                                                                                                         //}

                sql.Append(" union all select ISNULL(SUM(Price),0) from OrderList where (Status =1  or Status=2 or Status=3 or Status=4) and datediff(day,CreateTime,getdate())=0"); // 今天收入
                sql.Append(" union all select ISNULL(SUM(Price),0) from OrderList where (Status =1  or Status=2 or Status=3 or Status=4) and datediff(day,CreateTime,getdate())=1"); // 昨天收入
                sql.Append(" union all select ISNULL(SUM(Price),0) from OrderList where (Status =1  or Status=2 or Status=3 or Status=4) and datediff(month,CreateTime,getdate())=0"); //  本月的收入

                sql.Append(" union all  select count(distinct UID) from OrderList"); //下单人数

                sql.Append(" union all select count(*) from OrderList");    //下单笔数

                sql.Append(" union all select ISNULL(SUM(Price),0) from OrderList"); //下单金额

                sql.Append(" union all  select count(distinct UID) from OrderList where Status=1 or Status=2 or Status=3 or Status=4"); //付款人数

                sql.Append(" union all select count(*) from OrderList where Status=1 or Status=2 or Status=3 or Status=4");    //付款笔数

                sql.Append(" union all select ISNULL(SUM(Price),0) from OrderList where Status=1 or Status=2 or Status=3 or Status=4"); //付款金额

                sql.Append(" union all select count(*) from OrderList where datediff(day,CreateTime,getdate())=1"); //昨日订单数

                sql.Append(" union all select count(*) from OrderList where Status=0"); //待付款

                sql.Append(" union all select count(*) from OrderList where Status=1"); //待发货

                List<decimal> list = DBContext.Database.SqlQuery<decimal>(sql.ToString()).ToList();
                Dictionary<string, decimal> map = new Dictionary<string, decimal>();
                map.Add("ordercount", list[0]);
                //  map.Add("out", list[1]);

                map.Add("todayin", list[1]);
                map.Add("yesterdayin", list[2]);
                map.Add("monthin", list[3]);

                map.Add("OrderPeople", list[4]); //下单人数
                map.Add("OrderTotal", list[5]); //下单笔数
                map.Add("OrderPrice", list[6]); //下单金额
                map.Add("PayPeople", list[7]);//付款人数
                map.Add("PayTotal", list[8]);//付款笔数
                map.Add("PayPrice", list[9]); //付款金额

                map.Add("yOrderCount", list[10]); //昨日订单数
                map.Add("DOrderPrice", list[11]); //待付款
                map.Add("DPayPrice", list[12]); //待发货

                return map;
            }
        }

        /// <summary>
        /// 更改他的支付类型
        /// </summary>
        /// <param name="OID"></param>
        /// <param name="newPayment"></param>
        /// <returns></returns>
        public int UpdatePayment(Guid OID, enPayment newPayment)
        {
            using (DBContext)
            {
                OrderList model = DBContext.OrderList.FirstOrDefault(m => m.ID == OID);
                if (model == null)
                    return (int)ErrorCode.订单不存在;
                if (model.Status.Value == (int)enOrderStatus.Cancel)//已被取消的订单
                {
                    return (int)ErrorCode.状态异常或已处理;
                }
                model.Payment = newPayment.ToString();
                model.UpdateTime = DateTime.Now;

                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 已购买订单，通过商品查询
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public dynamic GetOrdersByPro(Guid ProID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.vw_Orders
                        from b in DBContext.Users
                        where a.ProductID == ProID && a.UID == b.ID && a.Status >= (int)enOrderStatus.Payed
                        select new
                        {
                            a.UTrueName,
                            a.Uphone,
                            a.UpdateTime,
                            b.HeadImg1
                        };

                return GetPagedList(q.OrderByDescending(m => m.UpdateTime), pagesize, pageinex, out count);
            }
        }

        /// <summary>
        /// 修改订单支付后发货状态
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int ChangeOrderStatus(Guid Id, enOrderStatus status)
        {
            using (DBContext)
            {
                OrderList order = DBContext.OrderList.Where(m => m.ID == Id).FirstOrDefault();
                if (order != null)
                {
                    order.Status = (int)status;
                    order.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 验证订单号为传入状态
        /// </summary>
        /// <param name="number"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool IsStstusByNumber(string number, enOrderStatus status)
        {
            using (DBContext)
            {
                return DBContext.OrderList.Count(m => m.Status == (int)status) > 0;
            }
        }

        public List<OrderList> GetByNumber(Guid uid, string number, enOrderStatus? status)
        {
            using (DBContext)
            {
                var q = DBContext.OrderList.Where(m => m.UID == uid && m.Number == number);
                if (status != null)
                {
                    q = q.Where(m => m.Status == (int)status.Value);
                }
                return q.ToList();
            }
        }

        public decimal GetPayPrice(string number)
        {
            using (DBContext)
            {
                List<OrderList> list = DBContext.OrderList.Where(m => m.Number == number).ToList();
                decimal sum = list.Sum(m => m.Price);
                //     decimal amount = list.GroupBy(m => new { discountId = m.DiscountCouponID, amount = m.DiscountAmount }).Sum(m => m.Key.amount);
                //      return sum - amount;
                return sum;
            }
        }
    }
}
