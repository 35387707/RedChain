using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    public partial class TransferOutBLL : BaseBll
    {
        /// <summary>
        /// 获取个人提现列表
        /// </summary>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public List<TransferOut> GetUserList(Guid Uid)
        {
            using (DBContext)
            {
                return DBContext.TransferOut.Where(m => m.UID == Uid).ToList();
            }
        }

        /// <summary>
        /// 提现详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public TransferOut GetDetail(Guid ID)
        {
            using (DBContext)
            {
                return DBContext.TransferOut.FirstOrDefault(m => m.ID == ID);
            }
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        /// <param name="Uid"></param>
        /// <param name="BankID"></param>
        /// <param name="money"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int ApplyTransferOut(Guid Uid, Guid BankID, decimal money, string reason)
        {
            using (DBContext)
            {
                var bank = DBContext.BankList.FirstOrDefault(m => m.ID == BankID);
                if (bank == null)
                    return (int)ErrorCode.银行卡不存在;
                var user = DBContext.Users.FirstOrDefault(m => m.ID == Uid);
                if (user == null)
                    return (int)ErrorCode.账号不存在;
                if (user.Status != (int)enStatus.Enabled)
                    return (int)ErrorCode.账号不可用;
                if (user.Balance < money)
                    return (int)ErrorCode.账户余额不足;

                user.Balance -= money;

                TransferOut model = new TransferOut();
                model.ID = Guid.NewGuid();
                model.UID = Uid;
                model.Reason = reason;
                model.Price = money;
                model.ComPrice = money * SysConfigBLL.Transout;
                model.BankName = bank.BankName;
                model.BankZhiHang = bank.BankZhiHang;
                model.BankAccount = bank.BankAccount;
                model.BankUser = bank.BankUser;
                model.Status = (int)enApplyStatus.Normal;
                model.CreateTime = model.UpdateTime = DateTime.Now;

                DBContext.TransferOut.Add(model);
                int result = DBContext.SaveChanges();
                if (result > 0)
                {
                    PayListBLL paybll = new PayListBLL();
                    paybll.Insert(model.ID, Uid, enPayInOutType.Out, enPayType.Coin, enPayFrom.Transfor, money, "提现申请");
                }
                return result;
            }
        }

        /// <summary>
        /// 更新提现状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="comprice"></param>
        /// <returns></returns>
        public int UpdateStatus(Guid ID, enApplyStatus status, string remark, decimal comprice)
        {
            using (DBContext)
            {
                var model = DBContext.TransferOut.FirstOrDefault(m => m.ID == ID);
                if (model.Status != (int)enApplyStatus.Normal)//
                {
                    return (int)ErrorCode.状态异常或已处理;
                }
                model.Status = (int)status;
                model.UpdateTime = DateTime.Now;
                if (status == enApplyStatus.Faild)
                {
                    model.ApplyRemark = remark;
                }
                else
                {
                    if (comprice > 0)
                        model.ComPrice = comprice;
                }

                var user = DBContext.Users.FirstOrDefault(m => m.ID == model.UID);
                if (user != null)
                {
                    string msgcontent = string.Empty;
                    if (status == enApplyStatus.Success)
                    {
                        msgcontent = "您的提现申请【金额" + model.Price + "，实际到账" + (model.Price - model.ComPrice) + "】已审批通过,请注意您的银行卡账单信息。";
                    }
                    else
                    {
                        user.Balance += model.Price;
                        user.UpdateTime = DateTime.Now;
                        PayListBLL.Insert(DBContext, Guid.Empty,user.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Transfor,model.Price, "提现未通过，返还余额");
                        msgcontent = "您的提现申请【金额" + model.Price + "】已被拒绝,原因：" + remark;
                        new UserMsgBLL().Insert(user.ID, Guid.Empty, "提现通知", msgcontent, enMessageType.System, "", msgcontent);
                    }

                    Rewards rds = new Rewards();
                    rds.TransforOutRewards(model.ID);
                }
                else {
                    return (int)ErrorCode.账号不存在;
                }

               // return DBContext.SaveChanges();

                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作审核提现成功,用户名称:{1},状态:{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, status), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作审核提现失败,用户名称:{1},状态{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, status), enLogType.Admin);
                    return 0;
                }
            }
        }

        public dynamic GetList(string key, decimal? price, enApplyStatus? Status, DateTime? begin, DateTime? end
            , int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from u1 in DBContext.Users
                        from b in DBContext.TransferOut
                        where u1.ID == b.UID
                        orderby b.CreateTime, b.Status
                        select new
                        {
                            Name = u1.Name,
                            HeadImg=u1.HeadImg1,
                            CardNumber = u1.CardNumber,
                            TrueName = u1.TrueName,
                            Phone = u1.Phone,
                            UserType = u1.UserType,
                            RealCheck = u1.RealCheck,

                            ID = b.ID,
                            BankName = b.BankName,
                            BankZhiHang = b.BankZhiHang,
                            BankAccount = b.BankAccount,
                            BankUser = b.BankUser,
                            Price = b.Price,
                            ComPrice = b.ComPrice,
                            Reason = b.Reason,
                            ApplyRemark = b.ApplyRemark,
                            Status = b.Status,
                            CreateTime = b.CreateTime,
                            UpdateTime = b.UpdateTime,
                        };

                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key)
                      || m.CardNumber == key);
                }
                if (price.HasValue)
                {
                    q = q.Where(m => m.Price > price.Value);
                }
                if (Status.HasValue)
                {
                    q = q.Where(m => m.Status == (int)Status);
                }
                if (begin.HasValue)
                {
                    q = q.Where(m => m.CreateTime > begin);
                }
                if (end.HasValue)
                {
                    DateTime endTime = end.Value.AddDays(1);
                    q = q.Where(m => m.CreateTime < endTime);
                }

                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        public List<Models.TransferOutModel> GetList(string key, enApplyStatus? Status, DateTime? begin, DateTime? end
            , int pagesize, int pageinex, out int count,Guid? UID)
        {
            using (DBContext)
            {
                var q = from u1 in DBContext.Users
                        from b in DBContext.TransferOut
                        where u1.ID == b.UID
                        orderby b.CreateTime, b.Status
                        select new Models.TransferOutModel
                        {
                            UID=b.UID.Value,
                            Name = u1.Name,
                            CardNumber = u1.CardNumber,
                            TrueName = u1.TrueName,
                            Phone = u1.Phone,
                            UserType = u1.UserType,
                            RealCheck = u1.RealCheck,
                            Email=u1.Email,

                            ID = b.ID,
                            BankName = b.BankName,
                            BankZhiHang = b.BankZhiHang,
                            BankAccount = b.BankAccount,
                            BankUser = b.BankUser,
                            Price = b.Price,
                            ComPrice = b.ComPrice,
                            Reason = b.Reason,
                            ApplyRemark = b.ApplyRemark,
                            Status = b.Status,
                            CreateTime = b.CreateTime,
                            UpdateTime = b.UpdateTime,
                        };
                if (UID!=null) {
                    q = q.Where(m => m.UID == UID.Value);
                }
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key)
                      || m.CardNumber == key);
                }
                if (Status.HasValue)
                {
                    q = q.Where(m => m.Status == (int)Status);
                }
                if (begin.HasValue)
                {
                    q = q.Where(m => m.CreateTime > begin);
                }
                if (end.HasValue)
                {
                    q = q.Where(m => m.CreateTime < end);
                }

                return GetPagedList(q.OrderByDescending(m=>m.CreateTime), pagesize, pageinex, out count);
            }
        }

        /// <summary>
        /// 导出所有的提现数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Models.TransferOutModel> GetAllTansferOutList(string key, DateTime? begin, DateTime? end)
        {
            using (DBContext)
            {
                var q = from u1 in DBContext.Users
                        from b in DBContext.TransferOut
                        where u1.ID == b.UID
                        orderby b.CreateTime, b.Status
                        select new Models.TransferOutModel
                        {
                            UID = b.UID.Value,
                            Name = u1.Name,
                            CardNumber = u1.CardNumber,
                            TrueName = u1.TrueName,
                            Phone = u1.Phone,
                            UserType = u1.UserType,
                            RealCheck = u1.RealCheck,
                            Email = u1.Email,

                            ID = b.ID,
                            BankName = b.BankName,
                            BankZhiHang = b.BankZhiHang,
                            BankAccount = b.BankAccount,
                            BankUser = b.BankUser,
                            Price = b.Price,
                            ComPrice = b.ComPrice,
                            Reason = b.Reason,
                            ApplyRemark = b.ApplyRemark,
                            Status = b.Status,
                            CreateTime = b.CreateTime,
                            UpdateTime = b.UpdateTime,
                        };
                //if (UID != null)
                //{
                //    q = q.Where(m => m.UID == UID.Value);
                //}
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key)
                      || m.CardNumber == key);
                }
                if (begin.HasValue)
                {
                    q = q.Where(m => m.CreateTime > begin);
                }
                if (end.HasValue)
                {
                    q = q.Where(m => m.CreateTime < end);
                }
                return q.OrderByDescending(m => m.CreateTime).ToList();
            }
        }
    }
}
