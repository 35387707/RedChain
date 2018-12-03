using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Data.SqlClient;
using RelexBarBLL.EnumCommon;

namespace RelexBarBLL
{
    public class FriendBLL : BaseBll
    {
        public static Dictionary<int, int> MaxAddFriend = new Dictionary<int, int>() { { (int)enUserType.User, 1000 }, { (int)enUserType.Shop, (int)SysConfigBLL.ShopPrice }, { (int)enUserType.Agent, (int)SysConfigBLL.AgentPrice } };

        /// <summary>
        /// 通过name或者phone查询用户
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Users> FindFriend(string key)
        {
            using (DBContext)
            {
                return DBContext.Users.Where(m => m.Name.Contains(key) || m.Phone == key).ToList();
            }
        }
        /// <summary>
        /// 是否是好友
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="FID"></param>
        /// <returns></returns>
        public bool isFriend(Guid UID, Guid FID)
        {
            using (DBContext)
            {
                int i = DBContext.FriendShip.Count(m => m.UID == UID && m.FriendID == FID && m.IsDel == 0);
                if (i > 0)//已經是好友
                {
                    DBContext.Database.ExecuteSqlCommand(string.Format("update ChatMessage set HadRead=1,updatetime=getdate() where MType=10 and ((FUID='{0}' and TUID='{1}') or (FUID='{1}' and TUID='{0}')) ", UID, FID));
                }
                return i > 0;
            }
        }
        /// <summary>
        /// 是否有未处理的好友请求
        /// </summary>
        /// <param name="SID">发送者</param>
        /// <param name="RID">接收者</param>
        /// <returns></returns>
        public bool haveFriendRequest(Guid SID, Guid RID)
        {
            using (DBContext)
            {
                return DBContext.ChatMessage.Where(m => m.FUID == SID && m.TUID == RID && m.MType == (int)MessageType.FriendRequest && m.HadRead == 0).Count() > 0;
            }
        }
        /// <summary>
        /// 获得好友备注
        /// </summary>
        /// <returns></returns>
        public string GetRemark(Guid UID, Guid FUID)
        {
            using (DBContext)
            {
                FriendShip f = DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == FUID && m.IsDel == 0).FirstOrDefault();
                if (f != null)
                {
                    return f.Remark;
                }
                else
                {
                    return null;
                }
            }
        }
        public int AutoAgreeFriend(Guid MID)
        {
            using (DBContext)
            {
                ChatMessage msg = DBContext.ChatMessage.FirstOrDefault(m => m.MID == MID);
                if (msg != null)
                {
                    Users u = DBContext.Users.FirstOrDefault(m => m.ID == msg.TUID);
                    if (!(u.AddFriendVerify > 0))//1表示不需要验证，0需要验证
                    {
                        return AddFriend(msg.TUID, msg.FUID, msg);
                    }
                    else
                    {
                        return -1;
                    }

                }
            }
            return -1;
        }

        /// <summary>
        /// 相互添加好友
        /// </summary>
        /// <returns></returns>
        public int AddFriend(Guid UID, Guid FID, ChatMessage message)
        {
            if (UID == FID)//自己不能加自己
            {
                return 0;
            }
            using (DBContext)
            {
                //先判断以前是否是好友
                FriendShip f1 = DBContext.FriendShip.FirstOrDefault(m => m.UID == UID && m.FriendID == FID);
                FriendShip f2 = DBContext.FriendShip.FirstOrDefault(m => m.UID == FID && m.FriendID == UID);
                if (f1 != null && f2 != null)
                {
                    if (f1.IsDel == 1 && f1.IsDel == 1)//已经是好友关系
                    {
                        return 0;
                    }
                    f1.IsDel = 0;
                    f2.IsDel = 0;

                    int i = DBContext.SaveChanges();
                    //if (i > 0)
                    //{
                    //    return i;
                    //}
                    return 1;
                }

                FriendShip fs = new FriendShip();
                fs.ID = Guid.NewGuid();
                fs.UID = UID;
                fs.FriendID = FID;
                fs.Remark = "";//添加好友不设置默认备注
                fs.Ignore = 1;
                fs.IsTop = 0;
                fs.IsDel = 0;
                fs.IsBlack = 0;
                fs.Status = 1;
                fs.LastMID = message.MID;
                fs.lastReadID = message.MID;
                fs.CreateTime = DateTime.Now;
                fs.UpdateTime = DateTime.Now;
                FriendShip fs1 = new FriendShip();
                fs1.ID = Guid.NewGuid();
                fs1.UID = FID;
                fs1.FriendID = UID;
                fs1.Remark = "";//添加好友不设置默认备注
                fs1.Ignore = 1;
                fs1.IsTop = 0;
                fs1.IsDel = 0;
                fs1.IsBlack = 0;
                fs1.Status = 1;
                fs1.LastMID = message.MID;
                fs1.lastReadID = message.MID;
                fs1.CreateTime = DateTime.Now;
                fs1.UpdateTime = DateTime.Now;
                DBContext.FriendShip.Add(fs);
                DBContext.FriendShip.Add(fs1);
                //int j = DBContext.Database.ExecuteSqlCommand(string.Format("update ChatMessage set HadRead=1 where MID='{0}'", message.MID));
                int j = DBContext.Database.ExecuteSqlCommand(string.Format("update ChatMessage set HadRead=1,updatetime=getdate() where MID='{2}';update ChatMessage set HadRead=1,updatetime=getdate() where MType=10 and ((FUID='{0}' and TUID='{1}') or (FUID='{1}' and TUID='{0}')) ", UID, FID, message.MID));

                if (j > 0)
                {
                    return DBContext.SaveChanges();
                }
                else
                {
                    return -1;
                }

            }
        }
        /// <summary>
        /// 修改好友备注
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="FID"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public int ChangeRemark(Guid UID, Guid FID, string Remark)
        {
            using (DBContext)
            {
                FriendShip f = DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == FID && m.IsDel == 0).FirstOrDefault();
                if (f != null)
                {
                    f.Remark = Remark;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public FriendShip GetFriend(Guid uid, Guid fid)
        {
            using (DBContext)
            {
                return DBContext.FriendShip.Where(m => m.UID == uid && m.FriendID == fid).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取我的好友
        /// </summary>
        /// <returns></returns>
        public List<Models.ShowReceiverModel> GetFriendList(Guid UID)
        {
            using (DBContext)
            {
                //string sql = "select f.FriendID ID,f.Remark,u.HeadImg1 headimg,m.Content lastContent,m.CreateTime lastTime,1 Type,0 Gtype,m.MType,f.IsTop  from FriendShip f left join ChatMessage m on f.LastMID = m.MID left join Users u on f.FriendID = u.ID where (m.MID is null or m.IsDel=0) and f.UID = @uid";

                StringBuilder sql = new StringBuilder();
                sql.Append("select *,(select COUNT(c.MID) from ChatMessage c where ");
                sql.Append("c.CreateTime>temp.lastReadMsgTime ");
                sql.Append("and ((c.FUID=temp.UID and c.TUID=temp.ID) or(c.FUID=temp.ID and c.TUID=temp.UID)) ");
                sql.Append("and c.IsDel=0) WDCount from( ");
                sql.Append("select f.lastReadID,rm.CreateTime lastReadMsgTime,f.UID,f.FriendID ID,(case when (f.Remark='' or f.Remark=null) then u.TrueName else f.Remark end) Remark,u.Phone,u.HeadImg1 headimg,m.Content lastContent,m.CreateTime lastTime,1 Type,0 Gtype,m.MType,f.IsTop ");
                sql.Append("from FriendShip f left join ChatMessage m on f.LastMID = m.MID ");
                sql.Append("left join ChatMessage rm on f.lastReadID=rm.MID ");
                sql.Append("left join Users u on f.FriendID = u.ID where (m.MID is null or m.IsDel=0) ");
                sql.Append("and f.UID =@uid) as temp");

                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@uid",UID)
                };
                return DBContext.Database.SqlQuery<Models.ShowReceiverModel>(sql.ToString(), param).ToList();
            }
        }

        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public dynamic GetMyRecommendList(Guid uid, int? type, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from u in DBContext.Users
                        where u.FID == uid && u.Status == (int)enStatus.Enabled && (!type.HasValue || u.UserType == type)
                        orderby u.CreateTime descending
                        select new
                        {
                            ID = u.ID,
                            Name = u.Name,
                            TrueName = u.TrueName,
                            CardNumber = u.CardNumber,
                            Phone = u.Phone,
                            Sex = u.Sex,
                            HeadImg1 = u.HeadImg1,
                            FID = u.FID,
                            Status = u.Status,
                            UserType = u.UserType,
                            CreateTime = u.CreateTime,
                            UpdateTime = u.UpdateTime,
                            Descrition = u.Descrition,
                            Address = u.Address,
                            AreaCode = u.AreaCode,
                            Agent = DBContext.Users.Count(m => m.FID == u.ID && m.Status == (int)enStatus.Enabled && m.UserType == (int)enUserType.Agent),
                            Shop = DBContext.Users.Count(m => m.FID == u.ID && m.Status == (int)enStatus.Enabled && m.UserType == (int)enUserType.Shop),
                            User = DBContext.Users.Count(m => m.FID == u.ID && m.Status == (int)enStatus.Enabled && m.UserType == (int)enUserType.User),
                        };

                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public dynamic GetMyRecommendCount(Guid uid)
        {
            using (DBContext)
            {
                var q = from u in DBContext.Users
                        where u.FID == uid && u.Status == (int)enStatus.Enabled
                        select u.UserType;

                return new
                {
                    Agent = q.Count(m => m == (int)enUserType.Agent),
                    Shop = q.Count(m => m == (int)enUserType.Shop),
                    User = q.Count(m => m == (int)enUserType.User),
                };
            }
        }

        public List<Models.APPFriendModel> GetFriendListByAPP(Guid UID, Guid? fid)
        {
            using (DBContext)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select t.ID,UID,FriendID,");
                sql.Append("(case when (t.Remark='' or t.Remark=null) then u.Name else t.Remark end) Remark");
                sql.Append(",LastMID,t.Status,t.CreateTime,t.UpdateTime,lastReadID,IsTop,u.HeadImg1 HeadImg,Ignore,u.Name,u.Phone,u.Sex,u.LastLoginTime,u.CardNumber from (");
                sql.Append("select ID,UID,FriendID,Remark,LastMID,Status,CreateTime,UpdateTime");
                sql.Append(",lastReadID,IsTop,Ignore From FriendShip where UID=@uid and IsDel=0");
                if (fid != null)
                {
                    sql.Append(string.Format(" FriendID='{0}'", fid.Value));
                }
                sql.Append(") ");
                sql.Append("as t left join Users u on t.FriendID=u.ID");
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@uid",UID)
                };
                return DBContext.Database.SqlQuery<Models.APPFriendModel>(sql.ToString(), param).ToList();
            }
        }

        /// <summary>
        /// 最大好友数
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="uType"></param>
        /// <returns></returns>
        public int GetMyMaxFriendCount(Guid uid, int uType)
        {
            var max = MaxAddFriend[uType];
            using (RelexBarEntities db = new RelexBarEntities())
            {
                var sum = db.RedPacket.Where(m => m.UID == uid && m.RedType == (int)enRedType.Single).Sum(m => (decimal?)m.TotalPrice);
                if (sum.HasValue)
                    max += (int)sum.Value * 2; //(2018-9-26 09:52:06 乘以*2)
            }
            return max;
        }

        /// <summary>
        /// 是否可以添加好友，可能已超过最大数
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool CanAddFriend(Guid uid)
        {
            using (DBContext)
            {
                var u = DBContext.Users.FirstOrDefault(m => m.ID == uid);
                if (u == null)
                {
                    return false;
                }
                var c = DBContext.FriendShip.Count(m => m.UID == uid && m.IsDel == 0);
                return c <= GetMyMaxFriendCount(u.ID, u.UserType);
            }
        }

        /// <summary>
        /// 是否可以添加好友，可能已超过最大数
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int GetMyFriendCount(Guid uid)
        {
            using (DBContext)
            {
                return DBContext.FriendShip.Count(m => m.UID == uid && m.IsDel == 0);
            }
        }

        /// <summary>
        /// 获取我的群
        /// </summary>
        /// <returns></returns>
        public List<Models.ShowReceiverModel> GetGroupList(Guid UID, Guid? gid = null)
        {
            using (DBContext)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select g.GID ID,g.MUID,g.GName Remark,'' headimg,m.Content lastContent,m.CreateTime lastTime,2 Type,g.Gtype,m.MType,gu.IsTop,g.Notice,(select COUNT(MID) from");
                sql.Append(" ChatMessage c where c.TUID=g.GID and c.CreateTime>");
                sql.Append("(select CreateTime from ChatMessage cm where cm.MID=gu.LastReadID)) WDCount from");
                sql.Append(" ChatGroupUser gu left join ChatGroup g on gu.GID = g.GID");
                if (gid != null)
                {
                    sql.Append(string.Format(" and g.GID='{0}'", gid));
                }
                else
                {
                    sql.Append(string.Format(" and g.Gtype<>2", gid));
                }

                sql.Append(" left join ChatMessage m on g.LastMID = MID");
                sql.Append(" where (m.MID is null or m.IsDel=0) and g.status=1 and UID = @uid");

                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@uid",UID)
                };


                List<Models.ShowReceiverModel> list = DBContext.Database.SqlQuery<Models.ShowReceiverModel>(sql.ToString(), param).ToList();
                string sql2 = "select t.*,u.HeadImg1 HeadImg from (select* from ChatGroupUser where GID = '{0}') as t left join Users u on t.UID = u.ID";
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Users = DBContext.Database.SqlQuery<Models.APPChatGroupUser>(string.Format(sql2, list[i].ID)).ToList();
                }
                return list;
            }
        }
        /// <summary>
        /// 获取我的好友请求
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public List<Models.NewFriendModel> GetFriendRequestList(Guid UID, int index, int pageSize, MessageType type = MessageType.FriendRequest)
        {
            using (DBContext)
            {
                string sql = "select * from (select ROW_NUMBER() over(order by m.createtime desc) row_number,u.ID as UID,u.TrueName as Name,u.HeadImg1 HeadImg,u.Phone,m.MID,m.Content,m.HadRead,m.CreateTime from ChatMessage m left join Users u on FUID=u.ID where MType=@mtype and m.TUID=@tuid and hadread=0) as t where t.row_number>@min and t.row_number<=@max";
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@mtype",type),
                    new SqlParameter("@tuid",UID),
                    new SqlParameter("@min",(index-1)*pageSize),
                    new SqlParameter("@max",index*pageSize)
                };
                return DBContext.Database.SqlQuery<Models.NewFriendModel>(sql, param).ToList();
            }
        }

        /// <summary>
        /// 获得不同状态的好友请求数量
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="status">0 为未处理，1为已通过好友请求 2 为未通过好友请求</param>
        /// <returns></returns>
        public int GetFriendRequestCount(Guid UID, int status)
        {
            using (DBContext)
            {
                return DBContext.ChatMessage.Where(m => m.TUID == UID && m.MType == (int)MessageType.FriendRequest && m.HadRead == status).Count();
            }
        }

        /// <summary>
        /// 获得不同状态的好友请求数量
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="status">0 为未处理，1为已通过好友请求 2 为未通过好友请求</param>
        /// <returns></returns>
        public void GetUndoRequestCount(Guid UID, ref int sayhello, ref int friend, ref DateTime? saylt, ref DateTime? reqlt, ref string sayname, ref string reqname)
        {
            using (DBContext)
            {
                sayhello = DBContext.ChatMessage.Count(m => m.TUID == UID && m.MType == (int)MessageType.SayHello && m.HadRead == (int)enMessageState.Unabled);
                friend = DBContext.ChatMessage.Count(m => m.TUID == UID && m.MType == (int)MessageType.FriendRequest && m.HadRead == (int)enMessageState.Unabled);
                if (sayhello > 0)
                {
                    var s = DBContext.ChatMessage.OrderByDescending(m => m.CreateTime).FirstOrDefault(m => m.TUID == UID && m.MType == (int)MessageType.SayHello && m.HadRead == (int)enMessageState.Unabled);
                    sayname = s.Content;
                    saylt = s.CreateTime;
                }
                if (friend > 0)
                {
                    var t = DBContext.ChatMessage.OrderByDescending(m => m.CreateTime).FirstOrDefault(m => m.TUID == UID && m.MType == (int)MessageType.FriendRequest && m.HadRead == (int)enMessageState.Unabled);
                    reqname = t.Content;
                    reqlt = t.CreateTime;
                }
            }
        }

        /// <summary>
        /// 设置用户与好友的最后一条阅读消息
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="FriendID"></param>
        /// <param name="MID"></param>
        /// <returns></returns>
        public int SetFriendLastMessageID(Guid UID, Guid FriendID, Guid? MID)
        {
            using (DBContext)
            {

                FriendShip fs = DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == FriendID).FirstOrDefault();
                if (fs != null)
                {
                    if (MID == null)
                    {
                        //将与好友聊天的最新一个消息id赋值
                        fs.lastReadID = DBContext.ChatMessage.Where(m => (m.FUID == UID && m.TUID == FriendID) || (m.FUID == FriendID || m.TUID == UID)).OrderByDescending(m => m.CreateTime).Take(1).Select(m => m.MID).FirstOrDefault();
                    }
                    else
                    {
                        fs.lastReadID = MID;
                    }

                }
                return DBContext.SaveChanges();
            }
        }
        public int SetGroupLastMessageID(Guid UID, Guid GID, Guid? MID)
        {
            using (DBContext)
            {

                ChatGroupUser gu = DBContext.ChatGroupUser.Where(m => m.UID == UID && m.GID == GID).FirstOrDefault();
                if (gu != null)
                {
                    if (MID == null)
                    {
                        //将与好友聊天的最新一个消息id赋值
                        gu.LastReadID = DBContext.ChatMessage.Where(m => m.TUID == GID && m.TType < 3).OrderByDescending(m => m.CreateTime).Take(1).Select(m => m.MID).FirstOrDefault();
                    }
                    else
                    {
                        gu.LastReadID = MID.Value;
                    }

                }
                return DBContext.SaveChanges();
            }
        }

    }
}
