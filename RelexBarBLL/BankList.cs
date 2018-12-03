using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    public partial class BankListBLL : BaseBll
    {
        public List<BankList> GetUserBankList(Guid Uid)
        {
            using (DBContext)
            {
                return DBContext.BankList.Where(m => m.UID == Uid && m.Status == (int)enStatus.Enabled).ToList();
            }
        }

        public List<BankList> GetUsersBankList(Guid UID, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.BankList.Where(m => m.ID != Guid.Empty && m.UID == UID && m.Status == (int)enStatus.Enabled);
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }
        /// <summary>
        /// 获得最新添加的银行卡信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public BankList GetNewBankList(Guid uid)
        {
            using (DBContext)
            {
                return DBContext.BankList.Where(m => m.UID == uid && m.Status == (int)enStatus.Enabled).Take(1).FirstOrDefault();
            }
        }
        public BankList GetDetail(Guid ID)
        {
            using (DBContext)
            {
                return DBContext.BankList.FirstOrDefault(m => m.ID == ID);
            }
        }

        public Guid Insert(Guid Uid, string BankName, string BankZhiHang, string BankAccount, string BankUser, bool isDefault)
        {
            using (DBContext)
            {
                BankList model = new BankList();
                model.ID = Guid.NewGuid();
                model.UID = Uid;
                model.BankName = BankName;
                model.BankZhiHang = BankZhiHang;
                model.BankAccount = BankAccount;
                model.BankUser = BankUser;
                model.isDefault = isDefault;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;

                DBContext.BankList.Add(model);
                if (DBContext.SaveChanges() > 0)
                {
                    if (isDefault)//如果是默认，则把其他的设置为非默认
                    {
                        ExceSql("update BankList set isDefault=0 where uid={0} and id <>{1}", Uid, model.ID);
                    }
                    return model.ID;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
        public int Delete(Guid ID)
        {
            using (DBContext)
            {
                var model = DBContext.BankList.FirstOrDefault(m => m.ID == ID);
                if (model != null)
                {
                    model.Status = (int)enStatus.Unabled;
                    model.UpdateTime = DateTime.Now;

                    //DBContext.BankList.Remove(model);
                    return DBContext.SaveChanges();
                }
                return 0;
            }
        }

        /// <summary>
        /// 修改默认银行卡
        /// </summary>
        /// <returns></returns>
        public int EditDefault(Guid id, Guid UID, bool isDefault)
        {
            using (DBContext)
            {
                BankList model = DBContext.BankList.FirstOrDefault(m => m.ID == id && m.UID == UID);
                if (model == null)
                    return 0;

                model.isDefault = isDefault;

                if (DBContext.SaveChanges() > 0)
                {
                    if (isDefault)//如果是默认，则把其他的设置为非默认
                    {
                        ExceSql("update BankList set isDefault=0 where uid={0} and id <>{1}", UID, model.ID);
                    }
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 获取默认银行卡
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public BankList GetDefault(Guid UID)
        {
            using (DBContext)
            {
                return DBContext.BankList.FirstOrDefault(m => m.UID == UID && m.isDefault);
            }
        }
    }
}
