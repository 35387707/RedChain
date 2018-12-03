using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RelexBarBLL.Models;
using System.Web;

namespace RelexBarBLL
{
    public partial class UsersBLL : BaseBll
    {
        public int PayToUser(Guid uid, Guid toUid, decimal price, string payPwd)
        {
            using (DBContext)
            {

                Users u = DBContext.Users.FirstOrDefault(m => m.ID == uid);
                if (u.ID == null)
                {
                    return (int)ErrorCode.账号不存在;
                }
                if (u.PayPsw != MD5(payPwd))
                {
                    return (int)ErrorCode.支付密码不正确;
                }
                if (u.Balance < price)
                {
                    return (int)ErrorCode.账户余额不足;
                }
                Users tu = DBContext.Users.FirstOrDefault(m => m.ID == toUid);
                if (tu == null)
                {
                    return (int)ErrorCode.账号不存在;
                }
                tu.Balance += price;
                u.Balance -= price;
                tu.UpdateTime = u.UpdateTime = DateTime.Now;
                PayList p = new PayList();
                p.UID = u.ID;
                p.PayUID = tu.ID;
                p.InOut = (int)enPayInOutType.Out;
                p.PayType = (int)enPayType.Coin;
                p.FromTo = (int)enPayFrom.Exchange;
                p.Val = -price;
                p.Remark = "转账付款";
                p.Status = 1;
                p.CreateTime = p.UpdateTime = DateTime.Now;
                PayList p2 = new PayList();
                p2.UID = tu.ID;
                p2.PayUID = u.ID;
                p2.InOut = (int)enPayInOutType.In;
                p2.PayType = (int)enPayType.Coin;
                p2.FromTo = (int)enPayFrom.Exchange;
                p.Val = price;
                p.Remark = "转账收款";
                p.Status = 1;
                p.CreateTime = p.UpdateTime = DateTime.Now;
                DBContext.PayList.Add(p);
                DBContext.PayList.Add(p2);
                return DBContext.SaveChanges();
            }
        }
        public string GetName(Guid id)
        {
            using (DBContext)
            {
                return DBContext.Users.Where(m => m.ID == id).Select(m => m.Name).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取所有项目筹码，要求所有项目在account控制器有GetChip(Guid id)方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public decimal GetAllChip(Guid id)
        {
            decimal sum = 0;
            using (DBContext)
            {
                List<ServiceList> list = DBContext.ServiceList.ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    decimal chip = 0;
                    try
                    {
                        string result = RelexBarBLL.Utils.HttpService.Post(id.ToString(), string.Format("{0}/Account/GetChip/{1}", list[i].SafeUrl, id), 20);
                        JObject j = JsonConvert.DeserializeObject(result) as JObject;
                        chip = decimal.Parse(j["msg"].ToString());
                    }
                    catch (Exception)
                    {

                    }
                    sum += chip;
                }
            }
            return sum;
        }
        public bool Exist(string email)
        {
            using (DBContext)
            {
                return DBContext.Users.Where(m => m.Email == email).Count() > 0;
            }
        }
        public bool Exist(string areaCode, string phone)
        {
            using (DBContext)
            {
                return DBContext.Users.Where(m => m.AreaCode == areaCode && m.Phone == phone).Count() > 0;
            }
        }
        public Users GetUserByEmail(string email)
        {
            using (DBContext)
            {
                return DBContext.Users.FirstOrDefault(m => m.Email == email);
            }
        }
        public Users GetUserByCardNumber(string cardNumber)
        {
            using (DBContext)
            {
                return DBContext.Users.FirstOrDefault(m => m.CardNumber == cardNumber);
            }
        }

        public Users GetUserByUserPhone(string Phone)
        {
            using (DBContext)
            {
                return DBContext.Users.FirstOrDefault(m => m.Phone == Phone);
            }
        }

        /// <summary>
        /// 获取用户信息通过token
        /// </summary>
        /// <param name="AppID"></param>
        /// <param name="sign"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Users GetUserInfoByToken(string AppID, string sign, string token, out ErrorCode err)
        {
            using (DBContext)
            {
                err = ErrorCode.没有错误;
                //通过appid获取key
                string key = DBContext.ServiceList.Where(m => m.AppID == AppID).Select(m => m.AppSecret).FirstOrDefault();
                if (string.IsNullOrEmpty(key))
                {
                    err = ErrorCode.签名验证失败;
                    return null;
                }
                Utils.GCData d = new Utils.GCData();
                d.SetValue("AppID", AppID);
                d.SetValue("token", token);
                if (sign != d.MakeSign(key))
                {
                    err = ErrorCode.签名验证失败;
                    return null;
                }
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
                Users u = DBContext.Users.FirstOrDefault(m => m.ID == t.UID);
                return u;
            }
        }
        public string GetNewTokenByOldToken(string oldtoken)
        {
            using (DBContext)
            {
                OtherLoginToken t = DBContext.OtherLoginToken.FirstOrDefault(m => m.Token == oldtoken);
                if (t == null)
                {
                    return null;
                }
                Users u = DBContext.Users.FirstOrDefault(m => m.ID == t.UID);
                if (u == null)
                {
                    return null;
                }
                OtherLoginToken nt = new OtherLoginToken();
                nt.ID = Guid.NewGuid();
                nt.UID = u.ID;
                nt.CreateTime = nt.UpdateTime = DateTime.Now;
                nt.ExpiredTime = nt.CreateTime.AddDays(7);
                nt.Token = MD5(u.ID.ToString() + nt.ID.ToString());
                DBContext.OtherLoginToken.Add(nt);
                int i = DBContext.SaveChanges();
                if (i > 0)
                {
                    return nt.Token;
                }
                return null;
            }
        }
        public string GetNewToken(Guid id)
        {
            using (DBContext)
            {
                Users u = DBContext.Users.FirstOrDefault(m => m.ID == id);
                if (u != null)
                {
                    OtherLoginToken t = new OtherLoginToken();
                    t.ID = Guid.NewGuid();
                    t.UID = u.ID;
                    t.CreateTime = t.UpdateTime = DateTime.Now;
                    t.ExpiredTime = t.CreateTime.AddDays(7);
                    t.Token = MD5(u.ID.ToString() + t.ID.ToString());
                    DBContext.OtherLoginToken.Add(t);
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        return t.Token;
                    }
                }
                return string.Empty;
            }
        }
        public Users OtherLogin(string appid, string email, string psw, out string token, out ErrorCode err)
        {
            using (DBContext)
            {
                int j = DBContext.ServiceList.Where(m => m.AppID == appid).Count();
                if (j < 1)
                {
                    token = "";
                    err = ErrorCode.AppID不正确;
                    return null;
                }
                psw = MD5(psw);
                var q = DBContext.Users.Where(m => m.Email == email && m.Psw == psw);
                var m2 = q.FirstOrDefault();
                if (m2 != null)
                {
                    OtherLoginToken t = new OtherLoginToken();
                    t.ID = Guid.NewGuid();
                    t.UID = m2.ID;
                    t.CreateTime = t.UpdateTime = DateTime.Now;
                    t.ExpiredTime = t.CreateTime.AddDays(7);
                    t.Token = token = MD5(appid + m2.ID.ToString() + t.ID.ToString());
                    DBContext.OtherLoginToken.Add(t);
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        err = ErrorCode.没有错误;
                        return m2;
                    }
                }
            }
            token = "";
            err = ErrorCode.身份验证失败;
            return null;
        }

        #region app接口
        public List<Models.SearchUser> SearchFriend(string key)
        {
            using (DBContext)
            {
                string sql = @"select top 10 ID,Name,Phone,Sex,HeadImg1 HeadImg,CardNumber,Email,0 as IsFriend
                                from Users where Phone=@phone or CardNumber=@cardNumber or Email=@email";
                return DBContext.Database.SqlQuery<Models.SearchUser>(sql, new SqlParameter[] {
                    new SqlParameter("@phone",key),
                    new SqlParameter("@cardNumber",key),
                    new SqlParameter("@email",key)
                }).ToList();
            }
        }

        public List<Models.SearchUser> SearchFriend(string key, Guid UID)
        {
            using (DBContext)
            {
                string sql = @"select top 10 a.ID,a.Name,a.Phone,a.Sex,a.HeadImg1 as HeadImg,a.CardNumber,a.Email,
                                case when b.ID is null then 0 else 1 end as IsFriend 
                                from Users a
                                left join (select * from FriendShip where UID = @UID) b on b.FriendID=a.ID
                                where a.Phone=@phone or a.CardNumber=@cardNumber or a.Email=@email";
                return DBContext.Database.SqlQuery<Models.SearchUser>(sql, new SqlParameter[] {
                    new SqlParameter("@phone",key),
                    new SqlParameter("@cardNumber",key),
                    new SqlParameter("@email",key),
                    new SqlParameter("@UID",UID)
                }).ToList();
            }
        }
        #endregion

        public int ChangeAddFriendVerify(Guid UID, int Status)
        {
            using (DBContext)
            {
                Users u = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (u != null)
                {
                    u.AddFriendVerify = Status;
                    return DBContext.SaveChanges();
                }
            }
            return 0;
        }

        public Users GetUser(string name, string psw)
        {
            using (DBContext)
            {
                var q = DBContext.Users.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    q = q.Where(m => m.Name == name);
                }
                if (!string.IsNullOrWhiteSpace(psw))
                {
                    psw = MD5(psw);
                    q = q.Where(m => m.Psw == psw);
                }

                return q.FirstOrDefault();
            }
        }
        public Users GetUser(string name)
        {
            return GetUser(name, string.Empty);
        }

        public Users GetUserById(Guid id)
        {
            using (DBContext)
            {
                return DBContext.Users.FirstOrDefault(m => m.ID == id);
            }
        }
        public Users GetUserById(string id)
        {
            return GetUserById(Guid.Parse(id));
        }
        public Users GetUserByPhone(string before, string phone)
        {
            using (DBContext)
            {
                var q = DBContext.Users.Where(m => m.Phone == phone);
                if (!string.IsNullOrEmpty(before))
                {
                    q = q.Where(m => m.AreaCode == before);
                }
                return q.FirstOrDefault();
            }
        }

        public Users GetUserByKey(string num)
        {
            using (DBContext)
            {
                return DBContext.Users.FirstOrDefault(m => m.Phone == num || m.CardNumber == num);
            }
        }

        /// <summary>
        /// 获取推荐人，一级二级
        /// </summary>
        /// <param name="context"></param>
        /// <param name="UID"></param>
        /// <param name="FirFUser">一代推荐人</param>
        /// <param name="SecFuser">二代推荐人</param>
        /// <returns></returns>
        public static void GetFUsersByUID(RelexBarEntities context, Guid UID, out Users FirFUser, out Users SecFuser)
        {
            FirFUser = null; SecFuser = null;
            var user = context.Users.FirstOrDefault(m => m.ID == UID);
            if (user == null)
            {
                return;
            }
            FirFUser = context.Users.FirstOrDefault(m => m.ID == user.FID);
            if (FirFUser == null)
            {
                return;
            }
            var FID = FirFUser.FID;
            SecFuser = context.Users.FirstOrDefault(m => m.ID == FID);
        }

        /// <summary>
        /// 获取几级推荐人？不包含自己
        /// </summary>
        /// <param name="context"></param>
        /// <param name="UID"></param>
        /// <param name="ut">推荐人类型</param>
        /// <param name="deep">几代(剔除自己))</param>
        public static List<Users> GetLVUsersByUID2(RelexBarEntities context, Guid UID, enUserType ut, int deep)
        {
            List<Users> result = new List<Users>();
            var user = context.Users.FirstOrDefault(m => m.ID == UID);
            if (user != null)
            {
                result.AddRange(GetLVUsersByUID(context, user.FID, ut, deep));
            }
            return result;
        }

        /// <summary>
        /// 获取几级推荐人？
        /// </summary>
        /// <param name="context"></param>
        /// <param name="UID"></param>
        /// <param name="ut">推荐人类型</param>
        /// <param name="deep">几代(假如自己是代理的话，则自己为第一代)</param>
        public static List<Users> GetLVUsersByUID(RelexBarEntities context, Guid? FID, enUserType ut, int deep)
        {
            List<Users> result = new List<Users>();
            if (deep <= 0)
                return result;
            var user = context.Users.FirstOrDefault(m => m.ID == FID);
            if (user != null)
            {
                if (user.UserType == (int)ut)
                {
                    result.Add(user);
                    deep--;
                }
                result.AddRange(GetLVUsersByUID(context, user.FID, ut, deep));
            }
            return result;
        }

        /// <summary>
        /// 获取几级推荐人？不包含自己（获取代理或商家）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="UID"></param>
        /// <param name="deep">几代(剔除自己))</param>
        public static List<Users> GetShopAgentUsers(RelexBarEntities context, Guid UID, int deep)
        {
            List<Users> result = new List<Users>();
            var user = context.Users.FirstOrDefault(m => m.ID == UID);
            if (user != null)
            {
                result.AddRange(GetShopAgentUsers2(context, user.FID, deep));
            }
            return result;
        }

        /// <summary>
        /// 获取几级推荐人？不包含自己（获取代理或商家）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="FID">上级ID</param>
        /// <param name="deep"></param>
        /// <returns></returns>
        public static List<Users> GetShopAgentUsers2(RelexBarEntities context, Guid? FID, int deep)
        {
            List<Users> result = new List<Users>();
            if (deep <= 0)
                return result;
            var user = context.Users.FirstOrDefault(m => m.ID == FID);
            if (user != null)
            {
                if (user.UserType == (int)enUserType.Shop || user.UserType == (int)enUserType.Agent)
                {
                    result.Add(user);
                    deep--;
                }
                result.AddRange(GetShopAgentUsers2(context, user.FID, deep));
            }
            return result;
        }

        /// <summary>
        /// 获取上级
        /// </summary>
        /// <param name="context"></param>
        /// <param name="UID"></param>
        /// <param name="ut">推荐人类型</param>
        public static Users GetFUserByLV(RelexBarEntities context, Guid? FID, enUserType ut)
        {
            List<Users> result = new List<Users>();
            var user = context.Users.FirstOrDefault(m => m.ID == FID);
            if (user != null)
            {
                if (user.UserType == (int)ut)
                {
                    return user;
                }
                return GetFUserByLV(context, user.FID, ut);
            }
            return null;
        }

        public List<Users> GetUsersByFId(Guid fid)
        {
            using (DBContext)
            {
                return DBContext.Users.Where(m => m.FID == fid && m.UserType == (int)enUserType.User && m.Status == (int)enStatus.Enabled).ToList();
            }
        }
        public List<Users> GetUsersByFId(string fid)
        {
            return GetUsersByFId(Guid.Parse(fid));
        }

        public int GetUsersCountByFId(Guid fid)
        {
            using (DBContext)
            {
                return DBContext.Users.Count(m => m.FID == fid && m.UserType == (int)enUserType.User);
            }
        }

        public Users GetShopsByCardID(string cardid)
        {
            using (DBContext)
            {
                return DBContext.Users.FirstOrDefault(m => m.CardNumber == cardid && m.Status == (int)enStatus.Enabled);
            }
        }

        //public Users GetShopsDetails(Guid SID)
        //{
        //    using (DBContext)
        //    {
        //        return DBContext.Users.FirstOrDefault(m => m.UserType == (int)enUserType.Shop && m.ID == SID && m.Status == (int)enStatus.Enabled);
        //    }
        //}

        public List<Users> GetUsersSearch(string name, string truename, string phone, string wxname, string cardnumber)
        {
            using (DBContext)
            {
                var q = DBContext.Users.Where(m => m.UserType == (int)enUserType.User);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    q = q.Where(m => m.Name == name);
                }
                if (!string.IsNullOrWhiteSpace(truename))
                {
                    q = q.Where(m => m.TrueName == truename);
                }
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    q = q.Where(m => m.Phone == phone);
                }
                if (!string.IsNullOrWhiteSpace(wxname))
                {
                    q = q.Where(m => m.WxName == wxname);
                }
                if (!string.IsNullOrWhiteSpace(cardnumber))
                {
                    q = q.Where(m => m.CardNumber == cardnumber);
                }

                return q.ToList();
            }
        }

        public List<Users> GetUsersExcel(string key, string UserType)
        {
            using (DBContext)
            {
                var q = DBContext.Users.Where(m => m.ID != Guid.Empty);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.TrueName.Contains(key) || m.CardNumber == key || m.Name == key || m.Phone.Contains(key));
                }
                if (!string.IsNullOrEmpty(UserType))
                {
                    int Type = int.Parse(UserType);
                    q = q.Where(m => m.UserType == Type);
                }
                return q.OrderByDescending(m => m.CreateTime).ToList();
            }
        }
        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<Users> GetUsersSearch(string key, string UserType, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.Users.Where(m => m.ID != Guid.Empty);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.TrueName.Contains(key) || m.CardNumber == key || m.Name == key || m.Phone.Contains(key));
                }
                if (!string.IsNullOrEmpty(UserType))
                {
                    int Type = int.Parse(UserType);
                    q = q.Where(m => m.UserType == Type);
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }
        /// <summary>
        /// 查询获取正在申请审核的用户列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<Users> GetUsersRealCheckSearch(string key, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.Users.Where(m => m.ID != Guid.Empty && m.RealCheck == (int)enRealCheckStatus.审核中);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key)
                      || m.CardNumber == key || m.CredID == key);
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }

        public List<SearchQuickUser> GetUsersQuickSearch(string phone, string wxname, string name, enUserType? UserType, string FromTo, string card, string tag, string shopnum, string shoptype, string money, string money2, int index, int pageSize, out int sum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from (select ROW_NUMBER() over(order by u.CreateTime desc) row_number,u.AreaCode,u.HeadImg1,u.Phone,u.Name,u.Sex,u.WxName,u.Balance,u.TotalScore,u.ID,u.Status,u.CreateTime,u.LastLoginTime,u.UserType ");
            sql.Append("from Users u ");

            StringBuilder tj = new StringBuilder();
            sql.Append("where 1=1 ");
            if (string.IsNullOrEmpty(phone))
            {
                tj.Append(" and (1=1 or u.Email like @phone) ");
                //tj.Append(" and (1=1 or u.Phone like @phone) ");
            }
            else
            {
                tj.Append(" and u.Phone like @phone ");
                //tj.Append(" and u.Phone like @phone ");
            }
            //if (!string.IsNullOrEmpty(phone))
            //{
            //    tj.Append(" and u.Phone like @phone");

            //}
            if (!string.IsNullOrEmpty(wxname))
            {
                tj.Append(" and u.WxName like" + "'" + wxname + "'");
            }
            if (!string.IsNullOrEmpty(name))
            {
                tj.Append(" and u.Name like " + "'" + name + "'");
            }
            if (UserType != null)
            {
                tj.Append(" and u.UserType=" + (int)UserType.Value);
            }
            if (!string.IsNullOrEmpty(money))
            {
                tj.Append(" and u.TotalScore like " + money);
            }
            if (!string.IsNullOrEmpty(money2))
            {
                tj.Append(" and u.TotalScore like " + money2);
            }

            // if(!string.IsNullOrEmpty(UserType))
            //if (FromTo != null)
            //{
            //    tj.Append("and p.FromTo=" + (int)FromTo.Value);
            //}
            //if (InOut != null)
            //{
            //    tj.Append("and p.InOut=" + (int)InOut.Value);
            //}
            //if (string.IsNullOrEmpty(phone))
            //{
            //    tj.Append(" and (1=1 or u.Email like @phone) ");
            //    //tj.Append(" and (1=1 or u.Phone like @phone) ");
            //}
            //else
            //{
            //    tj.Append(" and u.Email like @phone ");
            //    //tj.Append(" and u.Phone like @phone ");
            //}
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
                sum = DBContext.Database.SqlQuery<int>("select count(*) from Users u where 1=1 " + tj.ToString(), new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%")
                }).FirstOrDefault();
                return DBContext.Database.SqlQuery<SearchQuickUser>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@phone","%"+phone+"%"),
                    new SqlParameter("@min", (index - 1) * pageSize),
                    new SqlParameter("@max", index * pageSize)
                }).ToList();

            }
        }

        /// <summary>
        /// 代理用户管理 查询
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<Users> GetUsersAgentSearch(string key, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {

                var q = DBContext.Users.Where(m => m.ID != Guid.Empty && m.UserType == (int)enUserType.Agent);
                //  q.Where(m => m.UserType == (int)enUserType.Agent);
                if (!string.IsNullOrEmpty(key))
                {
                    // q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key) || m.title.Contains(key));
                    q = q.Where(m => m.Name.Contains(key) || m.Phone.Contains(key) || m.WxName.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }
        public dynamic GetUsersSearch(string key, enUserType? usertype, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from u1 in DBContext.Users
                        join u2 in DBContext.Users on u1.FID equals u2.ID into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            ID = u1.ID,
                            Name = u1.Name,
                            CardNumber = u1.CardNumber,
                            TrueName = u1.TrueName,
                            WxName = u1.WxName,
                            Phone = u1.Phone,
                            CredID = u1.CredID,
                            CredImg1 = u1.CredImg1,
                            CredImg2 = u1.CredImg2,
                            LV = u1.LV,
                            Score = u1.Score,
                            TotalScore = u1.TotalScore,
                            Balance = u1.Balance,
                            UserType = u1.UserType,
                            AreaID = u1.AreaID,
                            RealCheck = u1.RealCheck,
                            Status = u1.Status,
                            Address = u1.Address,
                            Descrition = u1.Descrition,
                            HeadImg1 = u1.HeadImg1,
                            CreateTime = u1.CreateTime,
                            UpdateTime = u1.UpdateTime,
                            FID = u1.FID,
                            FuserName = tt != null ? (tt.Phone + "[" + tt.TrueName + "]") : "",
                            FuserPhone = tt != null ? tt.Phone : "",
                        };

                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone == key || m.CardNumber == key);
                }
                if (usertype.HasValue)
                {
                    q = q.Where(m => m.UserType == (int)usertype.Value);
                }

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }

        /// <summary>
        /// 获取实名制申请列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageinex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public dynamic GetRealChecklist(Guid id, string key, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from u1 in DBContext.Users
                        select new
                        {
                            ID = u1.ID,
                            Name = u1.Name,
                            CardNumber = u1.CardNumber,
                            TrueName = u1.TrueName,
                            Phone = u1.Phone,
                            CredID = u1.CredID,
                            CredImg1 = string.IsNullOrEmpty(u1.CredImg1) ? "" : u1.CredImg1,
                            CredImg2 = string.IsNullOrEmpty(u1.CredImg2) ? "" : u1.CredImg2,
                            UserType = u1.UserType,
                            RealCheck = u1.RealCheck,
                            Status = u1.Status,
                            CreateTime = u1.CreateTime,
                        };

                if (id != Guid.Empty)
                {
                    q = q.Where(m => m.ID == id);
                }
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key)
                      || m.CardNumber == key || m.CredID == key);
                }

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }

        public List<Users> GetUsersSearchAnd(string name, string truename, string phone, string wxname, string cardnumber)
        {
            using (DBContext)
            {
                var q = DBContext.Users.Where(m => m.UserType == (int)enUserType.User);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    q = q.Where(m => m.Name == name);
                }
                if (!string.IsNullOrWhiteSpace(truename))
                {
                    q = q.Where(m => m.TrueName == truename);
                }
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    q = q.Where(m => m.Phone == phone);
                }
                if (!string.IsNullOrWhiteSpace(wxname))
                {
                    q = q.Where(m => m.WxName == wxname);
                }
                if (!string.IsNullOrWhiteSpace(cardnumber))
                {
                    q = q.Where(m => m.CardNumber == cardnumber);
                }

                return q.ToList();
            }
        }

        /// <summary>
        /// 登陆，可用手机号、账号、微信等登陆
        /// </summary>
        /// <param name="name"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        public Users Login(string before, string phone, string psw, ref DateTime lastLoginTime, out DateTime nowLoginTime, enOSType? type = enOSType.Android, string device = "")
        {
            nowLoginTime = DateTime.Now;
            if (string.IsNullOrWhiteSpace(phone))
            {
                return null;
            }
            //logBll.InsertLog(string.Format("用户{0}尝试登陆中...", name), enLogType.Login);
            using (DBContext)
            {
                psw = MD5(psw);
                // var q = DBContext.Users.Where(m => ((m.Name == name || m.CardNumber == name || m.Phone == name) && m.Psw == psw)
                //  || m.WxName == name);
                var q = DBContext.Users.Where(m => m.AreaCode == before && m.Phone == phone && m.Psw == psw);
                var m2 = q.FirstOrDefault();

                if (m2 != null)
                {
                    logBll.InsertLog(m2.ID, string.Format("用户{0}-{1}-{2}登录成功...", m2.ID, m2.Phone, m2.CardNumber), enLogType.Login);
                    lastLoginTime = m2.LastLoginTime;
                    m2.LastLoginTime = nowLoginTime;
                    m2.OS = type == null ? (int)enOSType.Android : (int)type.Value;
                    m2.DEVICE = device;//registration_id

                    DBContext.SaveChanges();
                }
                return m2;
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="name"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        public Users Login(string email, string psw, out DateTime lastLoginTime, enOSType? type = enOSType.Android, string device = "")
        {
            lastLoginTime = DateTime.Now;
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }
            //logBll.InsertLog(string.Format("用户{0}尝试登陆中...", name), enLogType.Login);
            using (DBContext)
            {
                psw = MD5(psw);
                // var q = DBContext.Users.Where(m => ((m.Name == name || m.CardNumber == name || m.Phone == name) && m.Psw == psw)
                //  || m.WxName == name);
                var m2 = DBContext.Users.FirstOrDefault(m => m.Email == email && m.Psw == psw);

                if (m2 != null)
                {
                    logBll.InsertLog(m2.ID, string.Format("用户{0}-{1}-{2}登录成功...", m2.ID, m2.Phone, m2.CardNumber), enLogType.Login);
                    m2.LastLoginTime = lastLoginTime;
                    m2.OS = type == null ? (int)enOSType.Android : (int)type.Value;
                    if (!string.IsNullOrEmpty(device))
                    {
                        m2.DEVICE = device;
                    }
                    DBContext.SaveChanges();
                }
                return m2;
            }
        }
        public Users Login(string before, string phone, string psw)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return null;
            }
            using (DBContext)
            {
                psw = MD5(psw);
                var q = DBContext.Users.Where(m => m.AreaCode == before && m.Phone == phone && m.Psw == psw);
                var m2 = q.FirstOrDefault();
                return m2;
            }
        }

        /// <summary>
        /// 更新实时在线的ID标识，用于消息推送
        /// </summary>
        /// <param name="type"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public Users UpdateDevice(Guid UID, enOSType? type = enOSType.Android, string device = "")
        {
            using (DBContext)
            {
                var q = DBContext.Users.Where(m => m.ID == UID);
                var m2 = q.FirstOrDefault();
                if (m2 == null)
                {
                    return m2;
                }
                m2.OS = (int)type.Value;
                m2.DEVICE = device;
                DBContext.SaveChanges();

                return m2;
            }
        }

        public int ChangeLoginPsw(string phone, string oldpsw, string newpsw)
        {
            using (DBContext)
            {
                oldpsw = MD5(oldpsw);
                var q = DBContext.Users.FirstOrDefault(m => m.Phone == phone && m.Psw == oldpsw);
                if (q != null)
                {
                    q.Psw = MD5(newpsw);
                    q.UpdateTime = DateTime.Now;
                    var result = DBContext.SaveChanges();
                    logBll.InsertLog(q.ID, string.Format("用户：{0},修改登录密码成功", q.Name), enLogType.User);
                    return result;
                }
                return 0;
            }
        }

        public int ResetWXName(Guid UID, string NewWX)
        {
            using (DBContext)
            {
                var u = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (u != null)
                {
                    if (string.IsNullOrEmpty(NewWX) || DBContext.Users.FirstOrDefault(m => m.WxName == NewWX) == null)
                        u.WxName = NewWX;
                    else
                    {
                        return (int)ErrorCode.微信已被注册;
                    }
                    return DBContext.SaveChanges();
                }
                return (int)ErrorCode.账号不存在;
            }
        }

        public int ChangeLoginPsw(string phone, string newpsw)
        {
            using (DBContext)
            {
                var q = DBContext.Users.FirstOrDefault(m => m.Phone == phone);
                if (q != null)
                {
                    q.Psw = MD5(newpsw);
                    q.UpdateTime = DateTime.Now;
                    var result = DBContext.SaveChanges();
                    logBll.InsertLog(q.ID, string.Format("用户：{0},修改登录密码成功", q.Name), enLogType.User);
                    return result;
                }
                return 0;
            }
        }

        public int ChangePayPsw(Guid uid, string oldpsw, string newpsw)
        {
            using (DBContext)
            {
                oldpsw = MD5(oldpsw);
                var q = DBContext.Users.FirstOrDefault(m => m.ID == uid);
                if (q != null)
                {
                    q.PayPsw = MD5(newpsw);
                    q.UpdateTime = DateTime.Now;
                    var result = DBContext.SaveChanges();
                    logBll.InsertLog(q.ID, string.Format("用户：{0},修改支付密码成功", q.Name), enLogType.User);
                    return result;
                }
                return 0;
            }
        }

        /// <summary>
        /// 校验支付密码是否正确
        /// </summary>
        /// <param name="id"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        public int CheckPay(Guid id, string psw)
        {
            using (DBContext)
            {
                psw = MD5(psw);
                var u = DBContext.Users.FirstOrDefault(m => m.ID == id);
                if (u == null)
                {
                    return (int)ErrorCode.账号不存在;
                }
                if (string.IsNullOrEmpty(u.PayPsw))
                {
                    return (int)ErrorCode.密码尚未设置;
                }
                if (u.PayPsw != psw)
                {
                    return (int)ErrorCode.密码不正确;
                }
                return 1;
            }
        }

        public int InsertUser(string name, string psw, string before, string phone, string wxid, enUserType usertype, Guid Fid, decimal Chip = 0, string email = "")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                //?不正确
                return (int)ErrorCode.姓名不正确;
            }
            //密码不能少于6位数
            //if (string.IsNullOrEmpty(wxid) && (string.IsNullOrWhiteSpace(psw) || psw.Length < 6))
            //{
            //    //?不正确
            //    return (int)ErrorCode.密码格式不正确;
            //}
            if ((string.IsNullOrWhiteSpace(psw) || psw.Length < 6))
            {
                return (int)ErrorCode.密码格式不正确;
            }
            ////手机号码不能为空，且手机号码必须11位数
            //if (!IsPhone(phone))
            //{
            //    //?不正确
            //    return (int)ErrorCode.手机不正确;
            //}

            using (DBContext)
            {
                //if (DBContext.Users.FirstOrDefault(m => m.Email == email) != null)//用户名或者手机号重复？
                //    return (int)ErrorCode.账号已被注册;
                if (DBContext.Users.FirstOrDefault(m => m.Phone == phone) != null)//用户名或者手机号重复？
                    return (int)ErrorCode.账号已被注册;

                Users fuser = null;
                //推荐人不存在，推荐人也不能为空
#if DEBUG
                if (Fid != Guid.Empty)
#endif
                {
                    //?不正确
                    fuser = DBContext.Users.FirstOrDefault(m => m.ID == Fid);
                    if (fuser == null)
                        return (int)ErrorCode.推荐人不存在;
                }

                Users model = new Users()
                {
                    Name = name,
                    Psw = MD5(psw),
                    UserType = (int)usertype,
                    AreaCode = before,
                    Phone = phone,
                    FID = Fid,
                    HeadImg1 = "/img/chat/head.jpg",
                    //以下信息自动生成
                    ID = Guid.NewGuid(),
                    Status = (int)enStatus.Enabled,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    CardNumber = "",
                    TrueName = "",
                    WxName = wxid,
                    PayPsw = "",
                    //   LastLoginTime = DateTime.Parse("1945-08-15"),
                    LastLoginTime = DateTime.Now,
                    DEVICE = string.Empty,
                    AddFriendVerify = 0,
                    Email = email,
                };
                DBContext.Users.Add(model);

                try
                {
                    int result = DBContext.SaveChanges();
                    if (result > 0)
                    {
                        model.CardNumber = FunCardNum(model.C_index);

                        if (fuser != null)//有推荐人？
                        {
                            //推荐人福音积分+1
                            fuser.Score += 1;
                            fuser.TotalScore += 1;
                            PayListBLL.Insert(DBContext, model.ID, fuser.ID, enPayInOutType.In, enPayType.Point, enPayFrom.Reward, 1, "推荐好友注册奖励");
                        }

                        DBContext.SaveChanges();

                        new Services.HuanXinIM().CreateUser_Huanxin(model.ID, model.Phone);

                        logBll.InsertLog(model.ID, string.Format("注册用户成功：{0},手机号{1},推荐人ID{2}", name, phone, Fid), enLogType.User);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    logBll.InsertLog(model.ID, string.Format("注册用户失败：{0},手机号{1},推荐人ID{2},错误：{3}", name, phone, Fid, ex), enLogType.User);
                    return 0;
                }
            }
        }

        public int InsertUser_Admin(string name, string psw, string phone, string wxid, enUserType usertype, Guid Fid
    , int cartlv, decimal score, decimal balance, string address, string desc, string headimg, string truename)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                //?不正确
                return (int)ErrorCode.姓名不正确;
            }
            //密码不能少于6位数
            if (string.IsNullOrWhiteSpace(psw) || psw.Length < 6)
            {
                //?不正确
                return (int)ErrorCode.密码格式不正确;
            }
            //手机号码不能为空，且手机号码必须11位数
            if (!IsPhone(phone))
            {
                //?不正确
                return (int)ErrorCode.手机不正确;
            }

            using (DBContext)
            {
                if (DBContext.Users.FirstOrDefault(m => m.Phone == phone || m.Name == name || (!string.IsNullOrEmpty(wxid) && m.WxName == wxid)) != null)//用户名或者手机号重复？
                    return (int)ErrorCode.账号已被注册;

                Users model = new Users()
                {
                    Name = name,
                    Psw = MD5(psw),
                    UserType = (int)usertype,
                    Phone = phone,
                    FID = Fid,
                    LV = (int)cartlv,
                    Score = score,
                    TotalScore = score,
                    Balance = balance,
                    Address = address,
                    Descrition = desc,
                    HeadImg1 = headimg,
                    TrueName = truename,
                    WxName = wxid,

                    //以下信息自动生成
                    ID = Guid.NewGuid(),
                    Status = (int)enStatus.Enabled,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    CardNumber = "",
                    PayPsw = "",
                };
                DBContext.Users.Add(model);

                try
                {
                    int result = DBContext.SaveChanges();
                    if (result > 0)
                    {
                        model.CardNumber = FunCardNum(model.C_index);
                        DBContext.SaveChanges();

                        logBll.InsertLog(model.ID, string.Format("注册用户成功：{0},手机号{1},推荐人ID{2}", name, phone, Fid), enLogType.User);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    logBll.InsertLog(model.ID, string.Format("注册用户失败：{0},手机号{1},推荐人ID{2},错误：{3}", name, phone, Fid, ex), enLogType.User);
                    return 0;
                }
            }
        }

        public string FunCardNum(int index)
        {
            var i = (100000000 + index).ToString();
            return "FBD" + i.Substring(i.Length - 8);
        }

        /// <summary>
        /// 申请实名验证
        /// </summary>
        /// <returns></returns>
        public int ApplyRealCheck(Guid id, string realname, string cid, string img1, string img2)
        {
            using (DBContext)
            {
                var user = DBContext.Users.FirstOrDefault(m => m.ID == id);
                if (user == null && user.RealCheck != 0)//已经申请过，或者账号不存在
                    return 0;

                user.TrueName = realname;
                user.CredID = cid;
                user.CredImg1 = img1;
                user.CredImg2 = img2;
                user.RealCheck = (int)enRealCheckStatus.审核中;

                logBll.InsertLog(user.ID, "用户" + user.ID + "正在申请实名验证", enLogType.User);
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 通过实名验证
        /// </summary>
        /// <returns></returns>
        public int AcceptRealCheck(Guid id, enRealCheckStatus State, string remark)
        {
            using (DBContext)
            {
                var user = DBContext.Users.FirstOrDefault(m => m.ID == id);
                if (user == null)//已经申请过，或者账号不存在
                    return (int)ErrorCode.账号不存在;
                if (user.RealCheck != (int)enRealCheckStatus.审核中)
                    return (int)ErrorCode.状态异常或已处理;

                user.RealCheck = (int)State;

                UserMsgBLL msgbll = new UserMsgBLL();
                if (State == enRealCheckStatus.不通过)
                {
                    msgbll.Insert(user.ID, Guid.Empty, "实名验证不通过", "您的实名制认证不通过，原因：" + remark, enMessageType.System, "", "实名验证不通过");
                }
                else
                {
                    msgbll.Insert(user.ID, Guid.Empty, "实名验证通过", "您的实名制认证已通过，您现在可以进行提现操作了。", enMessageType.System, "", "实名验证通过");
                }

                user.UpdateTime = DateTime.Now;

                logBll.InsertLog(user.ID, "用户" + user.ID + "实名验证" + State, enLogType.User);
                //  return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作审核用户成功,用户名称:{1},状态:{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, user.Name + "--" + user.Phone, State), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作审核用户失败,用户名称:{1},状态:{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, user.Name + "--" + user.Phone, State), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int ChangePhone(Guid id, string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return 0;
            }
            using (DBContext)
            {
                var u = DBContext.Users.FirstOrDefault(m => m.ID == id);
                if (u == null && DBContext.Users.FirstOrDefault(m => m.Phone == phone) != null)
                    return 0;
                u.Phone = phone;
                var result = DBContext.SaveChanges();
                logBll.InsertLog(u.ID, string.Format("用户：{0},修改手机号码为{1}", u.Name, u.Phone), enLogType.User);
                return result;
            }
        }

        public int UpdateUser(Users model)
        {
            using (DBContext)
            {
                DBContext.Users.Attach(model);
                DBContext.Entry<Users>(model).State = System.Data.Entity.EntityState.Modified;
                return DBContext.SaveChanges();
            }
        }

        public int DeleteUser(string name)
        {
            using (DBContext)
            {
                var q = DBContext.Users.FirstOrDefault(m => m.Name == name);
                if (q != null)
                {
                    q.Status = (int)enStatus.Unabled;//不可用
                    return DBContext.SaveChanges();
                }
                return 0;
            }
        }
        public decimal GetBalance(Guid UID)
        {
            using (DBContext)
            {
                return DBContext.Users.Where(m => m.ID == UID).Select(m => m.Balance).FirstOrDefault();

            }
        }
        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int ChangeHeadImg(Guid UID, string path)
        {
            using (DBContext)
            {
                Users u = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (u != null)
                {
                    u.HeadImg1 = path;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 获得用户头像
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public string GetHeadImg(Guid UID)
        {
            using (DBContext)
            {
                return DBContext.Users.Where(m => m.ID == UID).Select(m => m.HeadImg1).FirstOrDefault();
            }
        }
        /// <summary>
        /// 修改名称
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int ChangeName(Guid UID, string Name)
        {
            using (DBContext)
            {
                Users u = DBContext.Users.Where(m => m.ID == UID).FirstOrDefault();
                DBContext.Database.ExecuteSqlCommand("update ChatGroupUser set UNick=@nick where UID=@uid", new SqlParameter[] {
                    new SqlParameter("nick",Name),
                    new SqlParameter("uid",UID)
                });
                DBContext.Database.ExecuteSqlCommand("update FriendShip set Remark=@remark where FriendID=@uid", new SqlParameter[] {
                    new SqlParameter("remark",Name),
                    new SqlParameter("uid",UID)
                });
                if (u != null)
                {
                    u.Name = Name;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 获得群组用户头像
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public List<Models.UserHeadModel> GetGroupUserHeadImg(Guid GID)
        {
            using (DBContext)
            {
                string sql = "select UID,HeadImg1 HeadImg,UNick from ChatGroupUser g left join Users u on g.UID=u.ID where GID=@gid ";
                return DBContext.Database.SqlQuery<Models.UserHeadModel>(sql, new SqlParameter[] {
                    new SqlParameter("@gid",GID)
                }).ToList();
            }
        }
        /// <summary>
        /// 修改用户性别
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public int ChangeSex(Guid UID, int sex)
        {
            using (DBContext)
            {
                Users u = DBContext.Users.Where(m => m.ID == UID).FirstOrDefault();
                if (u != null)
                {
                    u.Sex = sex;
                    u.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 更改用户状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int ChangeUserStatus(Guid uid, enStatus status)
        {
            using (DBContext)
            {
                Users user = DBContext.Users.Where(m => m.ID == uid).FirstOrDefault();
                if (user != null)
                {
                    user.Status = (int)status;
                    user.UpdateTime = DateTime.Now;
                }
                //return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更改用户状态成功,用户名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, "(" + user.TrueName + ")" + user.Phone, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更改用户状态失败,用户名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, user.TrueName + user.Phone, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    return 0;
                }
            }
        }

        /// <summary>
        /// 升级会员级别
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="newtype"></param>
        /// <returns></returns>
        public int UpgradeUsertype(Guid UID, enUserType newtype, Common.enPayment Payment)
        {
            using (DBContext)
            {
                var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (user.UserType >= (int)newtype)
                    return (int)ErrorCode.账号类型不正确;
                if (Payment == enPayment.LOCAL)
                {
                    decimal money = 0;
                    if (newtype == enUserType.Agent && user.Balance >= SysConfigBLL.AgentPrice)
                    {
                        money = SysConfigBLL.AgentPrice;
                    }
                    else if (newtype == enUserType.Shop && user.Balance >= SysConfigBLL.ShopPrice)
                    {
                        money = SysConfigBLL.ShopPrice;
                    }
                    else
                        return (int)ErrorCode.账户余额不足;

                    user.Balance -= money;
                    user.Score += money;
                    user.TotalScore += money;
                    PayListBLL.Insert(DBContext, null, UID, enPayInOutType.Out, enPayType.Coin, enPayFrom.UpgradeLV, money, "升级" + (newtype == enUserType.Agent ? "福相" : "福将") + "消费");
                    PayListBLL.Insert(DBContext, null, UID, enPayInOutType.In, enPayType.Point, enPayFrom.UpgradeLV, money, "升级" + (newtype == enUserType.Agent ? "福相" : "福将") + "奖励");
                }

                user.UserType = (int)newtype;
                DBContext.SaveChanges();

                if (newtype == enUserType.Agent)//如果是升级了代理商
                    new RedPacksBLL().SendRedPacket(Guid.Empty, enRedType.Auto_NewAgentSend, "亿万福包,疯狂来袭", "/Content/fbinfo/ptfb.jpg", "", 0, 0, 0, 0, "", "", null);//公司发送216红包
                new RedPacksBLL().SendRedPacket(Guid.Empty, enRedType.Auto_SystemAchieveSend, "亿万福包,疯狂来袭", "/Content/fbinfo/cjfb.jpg", "", 0, 0, 0, 0, "", "", null); //自动判断公司业绩是否达到
                new Rewards().UpadateUserLVRewards(user.ID);//计算奖励
                return 1;
            }
        }

        /// <summary>
        /// 验证余额是否满足
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool CheckUserBalance(Guid UID, decimal price)
        {
            using (DBContext)
            {
                return DBContext.Users.FirstOrDefault(m => m.ID == UID).Balance >= price;
            }
        }
    }
}
