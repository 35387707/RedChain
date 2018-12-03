using RelexBarBLL.EnumCommon;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarBLL.Models;
using System.Data.SqlClient;
using System.Web;

namespace RelexBarBLL
{
    public class ChatMessageBLL : BaseBll
    {
        #region app新增方法
        /// <summary>
        /// 把isDel改为1
        /// </summary>
        /// <param name="MID"></param>
        /// <returns></returns>
        public int DeleteMessage(Guid UID, Guid MID)
        {
            using (DBContext)
            {
                ChatMessage msg = DBContext.ChatMessage.Where(m => m.MID == MID && m.FUID == UID).FirstOrDefault();
                if (msg == null)
                {
                    return -2;//消息不存在
                }
                //红包转装不允许撤回
                if (msg.MType == 2 || msg.MType == 9 || msg.MType == 3)
                {
                    return -3;//消息类型不正确
                }
                msg.IsDel = 1;
                return DBContext.SaveChanges();
            }
        }

        public Guid? GetLastReadMID(Guid UID, MessageTType TType, Guid ID)
        {
            using (DBContext)
            {
                switch (TType)
                {
                    case MessageTType.User:
                        return DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == ID).Select(m => m.lastReadID).FirstOrDefault();
                    case MessageTType.Group:
                        return DBContext.ChatGroupUser.Where(m => m.GID == ID && m.UID == UID).Select(m => m.LastReadID).FirstOrDefault();
                    default:
                        return null;
                }
            }
        }
        public int SetLastReadMID(Guid UID, MessageTType TType, Guid ID, Guid MID)
        {
            using (DBContext)
            {
                switch (TType)
                {
                    case MessageTType.User:
                        FriendShip f = DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == ID).FirstOrDefault();
                        if (f != null)
                            f.lastReadID = MID;
                        break;
                    case MessageTType.Group:
                        ChatGroupUser c = DBContext.ChatGroupUser.Where(m => m.GID == ID && m.UID == UID).FirstOrDefault();
                        if (c != null)
                            c.LastReadID = MID;
                        break;
                    default:
                        break;
                }
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 获取接收者时间大于date的消息
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<ChatMessage> GetMyRecMessage(Guid UID, DateTime? date)
        {
            using (DBContext)
            {
                string head = "select * from ChatMessage ";
                string head2 = "select top " + SysConfigBLL.MessageCount + " * from ChatMessage ";

                StringBuilder sql = new StringBuilder();
                sql.Append("where isdel=0 ");
                if (date != null)
                {
                    sql.Append(string.Format("and CreateTime>Convert(datetime,'{0}')", date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                }
                sql.Append(string.Format("and ((TType<>2 and TUID='{0}') ", UID));
                sql.Append(string.Format("or ((TType=2 or TType=3) and (select COUNT(CGUID) from ChatGroupUser where GID=TUID and UID='{0}')>0))", UID));
                sql.Append(" order by CreateTime desc");
                List<ChatMessage> list;
                if (date == null)
                {
                    list = DBContext.Database.SqlQuery<ChatMessage>(head2 + sql.ToString()).ToList();
                }
                else
                {
                    list = DBContext.Database.SqlQuery<ChatMessage>(head + sql.ToString()).ToList();
                }
                return list;
            }
        }

        public int Add(ChatMessage m)
        {
            using (DBContext)
            {
                DBContext.ChatMessage.Add(m);
                return DBContext.SaveChanges();
            }
        }
        #endregion
        public List<ChatMessage> GetNewMessageByLastMID(Guid UID, Guid MID)
        {
            using (DBContext)
            {
                ChatMessage c = DBContext.ChatMessage.Where(m => m.MID == MID).FirstOrDefault();
                if (c == null)
                {
                    return new List<ChatMessage>();
                }
                return DBContext.ChatMessage.Where(m => m.FUID == UID || m.TUID == UID && m.IsDel == 0 && m.CreateTime > c.CreateTime).OrderByDescending(m => m.CreateTime).ToList();
            }
        }
        /// <summary>
        /// 把isDel改为1
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="MID"></param>
        /// <returns></returns>
        public int DeleteMsg(Guid UID, Guid MID)
        {
            using (DBContext)
            {
                ChatMessage msg = DBContext.ChatMessage.Where(m => m.MID == MID && m.FUID == UID).FirstOrDefault();
                if (msg == null)
                {
                    return -1;
                }
                //红包转装不允许撤回
                if (msg.MType == 2 || msg.MType == 9 || msg.MType == 3)
                {
                    return -1;
                }
                msg.IsDel = 1;
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 获取消息表
        /// </summary>
        /// <param name="TType"></param>
        /// <param name="sid"></param>
        /// <param name="fid"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<ChatMessage> getMessageByTType(MessageTType? TType, Guid sid, Guid fid, int index, int pageSize, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.ChatMessage.Where(m => 1 == 1 && m.IsDel == 0);
                if (TType != null)
                {
                    q = q.Where(m => m.TType == (int)TType);
                }
                if (sid != null)
                {
                    q = q.Where(m => ((m.TUID == sid && m.FUID == fid) || (m.FUID == sid && m.TUID == fid)));
                }
                q = q.OrderByDescending(m => m.CreateTime);
                sum = q.Count();
                return q.Skip((index - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public ChatMessage Get(Guid MID)
        {
            using (DBContext)
            {
                return DBContext.ChatMessage.Where(m => m.MID == MID).FirstOrDefault();
            }
        }
        /// <summary>
        /// 消息改为已读
        /// </summary>
        public int Readed(Guid MID, int hadRead)
        {
            using (DBContext)
            {
                ChatMessage m = DBContext.ChatMessage.Where(m1 => m1.MID == MID).FirstOrDefault();
                if (m != null)
                {
                    m.HadRead = hadRead;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RID"></param>
        /// <param name="status">1未被领取 0已被领取</param>
        /// <returns></returns>
        public List<RedDetailModel> GetRedRecordByRID(Guid RID, int status)
        {
            using (DBContext)
            {
                string sql = "select m.MID,m.MType,r.RID,rr.Price,r.Count,rr.UID,u.Name,u.HeadImg1 HeadImg,rr.UpdateTime from ChatMessage m join SLRedPacket r on m.MID=r.MID join RedRecord rr on r.RID = rr.RID left join Users u on rr.UID = u.ID where r.RID=@rid  and(MType = 2 or MType = 9) and rr.Status = @status and UID is not null";
                return DBContext.Database.SqlQuery<RedDetailModel>(sql, new SqlParameter[] {
                    new SqlParameter("@rid",RID),
                    new SqlParameter("@status",status)
                }).ToList();
            }
        }
        //public List<dynamic> GetRedRecord(Guid MID) {

        //}

        public void ChatPageShow(Guid uid)
        {
            //select* from (
            //select MID, FUID, TUID, Content, CreateTime from ChatMessage where FUID= '58268BCD-338F-499E-BD2F-DFF9BA049744'
            //union
            //(select MID, TUID as FUID, FUID as TUID, Content, CreateTime from ChatMessage where TUID = '58268BCD-338F-499E-BD2F-DFF9BA049744')
            //) as temp order by CreateTime desc
            //using (DBContext) {
            //   List<string> users= DBContext.ChatMessage.Where(m => (m.TUID == uid.ToString()||m.FUID==uid.ToString())).Select(m => m.FUID).Distinct().ToList();
            //    foreach (string u in users)
            //    {
            //        ChatPageModel model = new ChatPageModel();
            //        model.UID = Guid.Parse(u);
            //        //先查询用户对用户记录
            //        ChatMessage msg= DBContext.ChatMessage.Where(m => m.TType == (int)MessageTType.User && (m.FUID == u || m.TUID == u)).Take(1).FirstOrDefault();
            //        Users tempu;
            //        if (Guid.Parse(msg.FUID) == uid)
            //        {
            //            tempu = DBContext.Users.Where(m => m.ID == Guid.Parse(msg.TUID)).Take(1).FirstOrDefault();
            //        }
            //        else {
            //            tempu = DBContext.Users.Where(m => m.ID == Guid.Parse(msg.FUID)).Take(1).FirstOrDefault();
            //        }
            //        model.Name = tempu.Name;
            //        model.
            //    }
            //}
        }

        /// <summary>
        /// 获得系统消息
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="status">0 为未处理，1为已通过好友请求 2 为未通过好友请求</param>
        /// <returns></returns>
        public dynamic GetSystemMessage(Guid UID, DateTime? time)
        {
            using (DBContext)
            {
                var gm = Guid.Empty;
                var t = from m in DBContext.ChatMessage
                        where m.FUID == gm && m.TUID == gm
                        orderby m.CreateTime descending
                        select new
                        {
                            m.MID,
                            m.FUID,
                            m.MType,
                            m.Content,
                            m.CreateTime,
                        };
                if (time.HasValue)
                {
                    t = t.Where(m => m.CreateTime > time);
                }

                return t.Take(50).ToList().OrderBy(m => m.CreateTime);

            }
        }

        public int Insert(Guid ToUid, Guid FromUid, string content)
        {
            using (DBContext)
            {
                
                ChatMessage message = new ChatMessage();
                message.MID = Guid.NewGuid();
                message.FUID = FromUid;
                message.TUID = ToUid;
                message.TType = 3;
                message.MType = 0;
                message.Content = content;

                message.HadRead = 0;
                message.IsDel = (int)enStatus.Enabled;
                message.Status = (int)enStatus.Enabled;
                message.CreateTime = message.UpdateTime = DateTime.Now;
                DBContext.ChatMessage.Add(message);
                return DBContext.SaveChanges();
            }
        }

        public int UpdateMsg(ChatMessage model)
        {
            using (DBContext)
            {
                DBContext.ChatMessage.Attach(model);
                DBContext.Entry<ChatMessage>(model).State = System.Data.Entity.EntityState.Modified;
                return DBContext.SaveChanges();
            }
        }

        public List<ChatMessage> GetAllList(string key, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.ChatMessage.Where(m => m.FUID == Guid.Empty && m.TUID == Guid.Empty);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Content.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }

        //public int UpdateStatus(Guid MID)
        //{
        //    using (DBContext)
        //    {
        //        ChatMessage msg = DBContext.ChatMessage.Where(m => m.MID == MID).FirstOrDefault();
        //        if (msg != null)
        //        {
        //            // msg.Status = (int)status;
        //            msg.Status = (int)enStatus.Unabled;
        //            msg.UpdateTime = DateTime.Now;
        //        }
        //        //  return DBContext.SaveChanges();
        //        try
        //        {
        //            int i = DBContext.SaveChanges();
        //            if (i > 0)
        //            {
        //                logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},查阅系统通知成功, 描述:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, msg.Content, "已读"), enLogType.Admin);
        //            }
        //            return i;
        //        }
        //        catch (Exception)
        //        {
        //            logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},查阅系统通知失败, 描述:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, msg.Content, "已读"), enLogType.Admin);
        //            return 0;
        //        }
        //    }
        //}
        public int UpdateStatus(Guid MID, int status)
        {
            using (DBContext)
            {
                var msg = DBContext.ChatMessage.FirstOrDefault(m => m.MID == MID);
                if (msg == null)
                {
                    return 0;
                }
                msg.Status = status;
                msg.UpdateTime = DateTime.Now;

                // return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作系统公告状态成功,状态：{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, status == 1 ? "启用" : "禁止"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作系统公告状态失败,状态：{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, status == 1 ? "启用" : "禁止"), enLogType.Admin);
                    return 0;
                }
            }
        }


    }
}
