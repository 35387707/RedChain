using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using RelexBarBLL.EnumCommon;
using RelexBarBLL.Models;
using System.Data.SqlClient;

namespace RelexBarBLL
{
    /// <summary>
    /// 聊天基础类
    /// </summary>
    public class ChatsBLL : BaseBll
    {
        public List<ChatMessage> GetGroupChatMessage(Guid gid, int count)
        {
            using (DBContext)
            {
                return DBContext.ChatMessage.Where(m => m.TUID == gid).OrderByDescending(m => m.CreateTime).Take(count).ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gtype">1普通群，2扫雷群</param>
        /// <returns></returns>
        public List<Models.ChatGroupModel> GetGroupList(string key, int index, int pageSize, int? gtype, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.ChatGroup.AsQueryable();

                if (gtype != null)
                {
                    q = q.Where(m => m.Gtype == gtype.Value);
                }
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.GName.Contains(key));
                }
                return base.GetPagedList(q.Join(DBContext.Users, g => g.MUID, u => u.ID, (g, u) => new Models.ChatGroupModel
                {
                    GID = g.GID,
                    GName = g.GName,
                    MUID = g.MUID,
                    Gtype = g.Gtype,
                    Notice = g.Notice,
                    Status = g.Status,
                    LastMID = g.LastMID,
                    CreateTime = g.CreateTime,
                    UpdateTime = g.UpdateTime,
                    MName = u.Name
                }).OrderByDescending(m => m.CreateTime), pageSize, index, out sum);
            }
        }
        /// <summary>
        /// 发送消息(群发消息)
        /// </summary>
        /// <returns></returns>
        public int SendMessage(Guid SUID, List<Guid> TUID, string content)
        {
            using (DBContext)
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取群组聊天记录
        /// </summary>
        /// <param name="gid">群组id</param>
        /// <param name="lastMID">最早的一条消息id</param>
        /// <param name="pageSize">数据长度默认20条</param>
        /// <returns></returns>
        public List<ChatMessage> GetGroupChatMessage(Guid gid, Guid? lastMID, int pageSize = 10)
        {
            using (DBContext)
            {
                var q = DBContext.ChatMessage.Where(m => m.TUID == gid && m.IsDel == 0);//.OrderByDescending(m=>m.CreateTime);

                if (lastMID == null)
                {
                    q = q.OrderByDescending(m => m.CreateTime).Take(pageSize);
                }
                else
                {
                    q = q.Where(m => m.CreateTime.Value < (DBContext.ChatMessage.Where(m2 => m2.MID == lastMID).Select(m1 => m1.CreateTime).FirstOrDefault().Value));
                    q = q.OrderByDescending(m => m.CreateTime).Take(pageSize);
                }
                return q.ToList();
            }
        }
        /// <summary>
        /// 获取群组聊天记录--往下滑动
        /// </summary>
        /// <param name="gid">群组id</param>
        /// <param name="lastMID">最早的一条消息id</param>
        /// <param name="pageSize">数据长度默认20条</param>
        /// <returns></returns>
        public List<ChatMessage> GetGroupChatMessageButtom(Guid gid, Guid? lastMID, int pageSize = 10)
        {
            using (DBContext)
            {
                var q = DBContext.ChatMessage.Where(m => m.TUID == gid && m.IsDel == 0);//.OrderByDescending(m=>m.CreateTime);

                if (lastMID == null)
                {
                    q = q.OrderBy(m => m.CreateTime).Take(pageSize);
                }
                else
                {
                    q = q.Where(m => m.CreateTime.Value >= (DBContext.ChatMessage.Where(m2 => m2.MID == lastMID).Select(m1 => m1.CreateTime).FirstOrDefault().Value));
                    q = q.OrderBy(m => m.CreateTime).Take(pageSize);
                }
                return q.ToList();
            }
        }
        /// <summary>
        /// 获取与用户的聊天记录
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="fid"></param>
        /// <param name="lastMID"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<ChatMessage> GetChatMessage(Guid uid, Guid fid, Guid? lastMID, int pageSize = 10)
        {
            using (DBContext)
            {
                var q = DBContext.ChatMessage.Where(m => (((m.TUID == uid && m.FUID == fid) || (m.FUID == uid && m.TUID == fid))) && m.TType == (int)MessageTType.User && m.IsDel == 0);//.OrderByDescending(m=>m.CreateTime);

                if (lastMID == null)
                {
                    q = q.OrderByDescending(m => m.CreateTime).Take(pageSize);
                }
                else
                {
                    q = q.Where(m => m.CreateTime.Value < (DBContext.ChatMessage.Where(m2 => m2.MID == lastMID).Select(m1 => m1.CreateTime).FirstOrDefault().Value));
                    q = q.OrderByDescending(m => m.CreateTime).Take(pageSize);
                }
                return q.ToList();
            }
        }
        /// <summary>
        /// 获取与用户的聊天记录--往下滑动
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="fid"></param>
        /// <param name="lastMID"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<ChatMessage> GetChatMessageButtom(Guid uid, Guid fid, Guid? lastMID, int pageSize = 10)
        {
            using (DBContext)
            {
                var q = DBContext.ChatMessage.Where(m => (((m.TUID == uid && m.FUID == fid) || (m.FUID == uid && m.TUID == fid))) && m.TType == (int)MessageTType.User && m.IsDel == 0);//.OrderByDescending(m=>m.CreateTime);

                if (lastMID == null)
                {
                    q = q.OrderBy(m => m.CreateTime).Take(pageSize);
                }
                else
                {
                    q = q.Where(m => m.CreateTime.Value >= (DBContext.ChatMessage.Where(m2 => m2.MID == lastMID).Select(m1 => m1.CreateTime).FirstOrDefault().Value));
                    q = q.OrderBy(m => m.CreateTime).Take(pageSize);
                }
                return q.ToList();
            }
        }

        public List<ChatMessage> SearchMessage(Guid UID, Guid FUID, string Content, MessageType type)
        {
            return DBContext.ChatMessage.Where(m => ((m.FUID == UID && m.TUID == FUID) || (m.FUID == FUID && m.TUID == UID)) && m.MType == (int)type && m.IsDel == 0 && m.Content.Contains(Content)).OrderByDescending(m => m.CreateTime).Take(20).ToList();
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="MID"></param>
        /// <returns></returns>
        public ChatMessage Detail(Guid UID, Guid MID)
        {
            using (DBContext)
            {
                return DBContext.ChatMessage.FirstOrDefault(m => m.MID == MID);
            }
        }

        /// <summary>
        /// 获取用户所在群的昵称
        /// </summary>
        /// <returns></returns>
        public string GetGroupNick(Guid GID, Guid UID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID == UID).Select(m => m.UNick).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取群名称
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public string GetGroupName(Guid GID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroup.Where(m => m.GID == GID).Select(m => m.GName).FirstOrDefault();
            }
        }
        /// <summary>
        /// 判断该群组消息是否置顶
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        public int GetGroupisTop(Guid GID, Guid UID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID == UID).Select(m => m.IsTop).FirstOrDefault();
            }
        }
        /// <summary>
        /// 判断该好友组消息是否置顶
        /// </summary>
        /// <param name="FUID"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        public int GetFriendisTop(Guid FUID, Guid UID)
        {
            using (DBContext)
            {
                return DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == FUID).Select(m => m.IsTop).FirstOrDefault();
            }
        }
        /// <summary>
        /// 更改群消息是否置顶
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="UID"></param>
        /// <param name="isTop"></param>
        /// <returns></returns>
        public int UpdateGroupIsTop(Guid GID, Guid UID, int isTop)
        {
            using (DBContext)
            {
                ChatGroupUser u = DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID == UID).FirstOrDefault();
                if (u != null)
                {
                    u.IsTop = isTop;
                    u.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 更改朋友消息是否置顶
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="UID"></param>
        /// <param name="isTop"></param>
        /// <returns></returns>
        public int UpdateFriendIsTop(Guid FUID, Guid UID, int isTop)
        {
            using (DBContext)
            {
                FriendShip f = DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == FUID).FirstOrDefault();
                if (f != null)
                {
                    f.IsTop = isTop;
                    f.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 修改所在群昵称
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="UID"></param>
        /// <param name="nick"></param>
        /// <returns></returns>
        public int UpdateGroupNick(Guid GID, Guid UID, string nick)
        {
            using (DBContext)
            {
                ChatGroupUser u = DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID == UID).FirstOrDefault();
                if (u != null)
                {
                    u.UNick = nick;
                    u.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 扫雷游戏记录
        /// </summary>
        /// <param name="RID">红包id</param>
        /// <returns></returns>
        public List<SLGameRecordModel> GetSLGameRecord(Guid RID)
        {
            using (DBContext)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select temp.*,cgu.UNick Nick from(select grecord.*,slr.MID,cm.TUID,us.Name SName from (select g.RID,g.GID,g.UID,u2.HeadImg1 HeadImg,g.Price,g.CreateTime ");
                sql.Append("from GameRecord g left join Users u2 on g.UID = u2.ID ");
                sql.Append("where g.RID = @rid) as grecord ");
                sql.Append("left join SLRedPacket slr on grecord.RID = slr.RID ");
                sql.Append("left join ChatMessage cm on slr.MID = cm.MID left join Users us on cm.FUID=us.ID) as temp,ChatGroupUser cgu ");
                sql.Append("where temp.UID = cgu.UID and temp.TUID = cgu.GID ");


                return DBContext.Database.SqlQuery<SLGameRecordModel>(sql.ToString(), new SqlParameter[] {
                    new SqlParameter("@rid",RID)
                }).ToList();
            }
        }

        /// <summary>
        /// 获取消息发送者的id
        /// </summary>
        /// <param name="MID"></param>
        /// <returns></returns>
        public Guid GetMessageSender(Guid MID)
        {
            return DBContext.ChatMessage.Where(m => m.MID == MID).Select(m => m.FUID).FirstOrDefault();
        }
        /// <summary>
        /// 获取最新消息
        /// </summary>
        /// <returns></returns>
        public List<ChatMessage> GetNewMessage(DateTime lasttime)
        {
            using (DBContext)
            {
                return null;
            }
        }
        /// <summary>
        /// 新增一条消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int AddChatMessage(ChatMessage msg)
        {
            using (DBContext)
            {

                if (msg.TType == (int)MessageTType.User)
                {//如果是普通用户消息

                    DBContext.Database.ExecuteSqlCommand("update FriendShip set lastMID=@mid where UID=@fuid and FriendID=@tuid", new SqlParameter[] {
                        new SqlParameter("mid",msg.MID),
                        new SqlParameter("fuid",msg.FUID),
                        new SqlParameter("tuid",msg.TUID)
                    });
                    DBContext.Database.ExecuteSqlCommand("update FriendShip set lastMID=@mid where UID=@tuid and FriendID=@fuid", new SqlParameter[] {
                        new SqlParameter("mid",msg.MID),
                        new SqlParameter("fuid",msg.FUID),
                        new SqlParameter("tuid",msg.TUID)
                    });

                }
                else if (msg.TType == (int)MessageTType.Group)
                {//如果是群组消息
                    DBContext.Database.ExecuteSqlCommand("update ChatGroup set lastMID=@mid where GID=@gid", new SqlParameter[] {
                        new SqlParameter("mid",msg.MID),
                        new SqlParameter("gid",msg.TUID)
                    });
                }

                DBContext.ChatMessage.Add(msg);
                return DBContext.SaveChanges();
            }
        }


        public ChatMessage GetSayHello(Guid? FUID, Guid? TUID, MessageTType? TType, MessageType? Type)
        {
            using (DBContext)
            {

                return DBContext.ChatMessage.FirstOrDefault(m => m.FUID == FUID && m.TUID == TUID && m.TType == (int)TType && m.MType == (int)Type);
            }
        }

        public int GetSayHelloCount(Guid? FUID, Guid? TUID, MessageTType? TType, MessageType? Type, DateTime Time)
        {
            using (DBContext)
            {
                Time = Time.Date;
                DateTime dtend = Time.AddDays(1);

                return DBContext.ChatMessage.Count(m => m.FUID == FUID && m.TUID == TUID
                        && m.TType == (int)TType && m.MType == (int)Type && m.CreateTime > Time && m.CreateTime < dtend);
            }
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="FID"></param>
        /// <param name="Remark">好友备注</param>
        /// <returns></returns>
        public int AddFriend(Guid UID, Guid FID, string Remark)
        {
            using (DBContext)
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取群组里的用户id列表
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public List<Guid> GetGroupUID(Guid GID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroupUser.Where(m => m.GID == GID).Select(m => m.UID).ToList();
            }
        }
        /// <summary>
        /// 删除好友
        /// </summary>
        /// <returns></returns>
        public int DeleteFriend(Guid UID, Guid FUID)
        {
            using (DBContext)
            {
                FriendShip f = DBContext.FriendShip.Where(m => m.UID == UID && m.FriendID == FUID).FirstOrDefault();
                FriendShip f2 = DBContext.FriendShip.Where(m => m.UID == FUID && m.FriendID == UID).FirstOrDefault();
                if (f != null && f2 != null)
                {
                    DBContext.FriendShip.Remove(f);
                    DBContext.FriendShip.Remove(f2);
                }
                string sql = "update ChatMessage set IsDel=1 where (FUID=@u1 and TUID=@u2) or (FUID=@u2 and TUID=@u1)";
                DBContext.Database.ExecuteSqlCommand(sql, new SqlParameter[] {
                    new SqlParameter("@u1",UID),
                    new SqlParameter("@u2",FUID)
                });
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 是否是群管理员
        /// </summary>
        /// <returns></returns>
        public bool isGroupAdmin(Guid GID, Guid UID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroup.Where(m => m.GID == GID && m.MUID == UID).Count() > 0;
            }
        }

        /// <summary>
        /// 创建群
        /// </summary>
        /// <param name="MUID">创建者，管理员</param>
        /// <param name="GType">群类型</param>
        /// <returns></returns>
        public int CreateGroup(Guid MUID, string GName, GType GType, Guid[] UIDS = null, Guid? gid = null)
        {
            using (DBContext)
            {
                ChatMessage msg = new ChatMessage();
                msg.MID = Guid.NewGuid();
                msg.FUID = Guid.Empty;
                msg.TType = (int)MessageTType.System;
                msg.MType = (int)MessageType.Text;
                msg.Content = GName + "已创建成功";
                msg.HadRead = 0;
                msg.IsDel = 0;
                msg.Status = 1;
                msg.CreateTime = DateTime.Now;
                msg.UpdateTime = DateTime.Now;
                DBContext.ChatMessage.Add(msg);

                ChatGroup cg = new ChatGroup();
                cg.GID = gid == null ? Guid.NewGuid() : gid.Value;
                msg.TUID = cg.GID;
                cg.GName = GName;
                cg.MUID = MUID;
                cg.Gtype = (int)GType;
                cg.Notice = "";
                cg.Status = 1;
                cg.LastMID = msg.MID;
                cg.CreateTime = DateTime.Now;
                cg.UpdateTime = DateTime.Now;
                if (GType == GType.GameGroup)
                {
                    ///创建免死用户///
                    ChatGroupUser u = new ChatGroupUser();
                    u.CGUID = Guid.NewGuid();
                    u.GID = cg.GID;
                    u.UID = Guid.Empty;
                    u.UNick = "免死";
                    u.LastReadID = Guid.Empty;
                    u.Ignore = 1;
                    u.IsTop = 1;
                    u.Status = 1;
                    u.CreateTime = DateTime.Now;
                    u.UpdateTime = DateTime.Now;
                    DBContext.ChatGroupUser.Add(u);
                }
                if (UIDS != null)
                {
                    List<Users> us = DBContext.Users.Where(m => UIDS.Contains(m.ID)).ToList();
                    for (int i = 0; i < us.Count; i++)
                    {
                        ChatGroupUser u = new ChatGroupUser();
                        u.CGUID = Guid.NewGuid();
                        u.GID = cg.GID;
                        u.UID = us[i].ID;
                        u.UNick = us[i].Name;
                        u.LastReadID = Guid.Empty;
                        u.Ignore = 1;
                        u.IsTop = 0;
                        u.Status = 1;
                        u.CreateTime = DateTime.Now;
                        u.UpdateTime = DateTime.Now;
                        DBContext.ChatGroupUser.Add(u);
                    }
                }
                ChatGroupUser cu = new ChatGroupUser();
                cu.CGUID = Guid.NewGuid();
                cu.GID = cg.GID;
                cu.UID = MUID;
                cu.UNick = DBContext.Users.Where(m => m.ID == MUID).Select(u => u.Name).FirstOrDefault();
                cu.LastReadID = Guid.Empty;
                cu.Ignore = 1;
                cu.IsTop = 0;
                cu.LastReadID = msg.MID;
                cu.Status = 1;
                cu.CreateTime = DateTime.Now;
                cu.UpdateTime = DateTime.Now;
                DBContext.ChatGroupUser.Add(cu);
                /////

                DBContext.ChatGroup.Add(cg);
                return DBContext.SaveChanges();

            }
        }

        /// <summary>
        /// 创建群
        /// </summary>
        /// <param name="MUID">创建者，管理员</param>
        /// <param name="GType">群类型</param>
        /// <returns></returns>
        public int CreateGroup(Guid MUID, string GName, GType GType, out Guid GID, Guid[] UIDS = null)
        {
            using (DBContext)
            {
                ChatMessage msg = new ChatMessage();
                msg.MID = Guid.NewGuid();
                msg.FUID = Guid.Empty;
                msg.TType = (int)MessageTType.System;
                msg.MType = (int)MessageType.Text;
                msg.Content = GName + "已创建成功";
                msg.HadRead = 0;
                msg.IsDel = 0;
                msg.Status = 1;
                msg.CreateTime = DateTime.Now;
                msg.UpdateTime = DateTime.Now;
                DBContext.ChatMessage.Add(msg);

                ChatGroup cg = new ChatGroup();
                cg.GID = Guid.NewGuid();
                GID = cg.GID;
                msg.TUID = cg.GID;
                cg.GName = GName;
                cg.MUID = MUID;
                cg.Gtype = (int)GType;
                cg.Notice = "";
                cg.Status = 1;
                cg.LastMID = msg.MID;
                cg.CreateTime = DateTime.Now;
                cg.UpdateTime = DateTime.Now;
                if (GType == GType.GameGroup)
                {
                    ///创建免死用户///
                    ChatGroupUser u = new ChatGroupUser();
                    u.CGUID = Guid.NewGuid();
                    u.GID = cg.GID;
                    u.UID = Guid.Empty;
                    u.UNick = "免死";
                    u.LastReadID = Guid.Empty;
                    u.Ignore = 1;
                    u.IsTop = 1;
                    u.Status = 1;
                    u.CreateTime = DateTime.Now;
                    u.UpdateTime = DateTime.Now;
                    DBContext.ChatGroupUser.Add(u);
                }
                if (UIDS != null)
                {
                    List<Users> us = DBContext.Users.Where(m => UIDS.Contains(m.ID)).ToList();
                    for (int i = 0; i < us.Count; i++)
                    {
                        ChatGroupUser u = new ChatGroupUser();
                        u.CGUID = Guid.NewGuid();
                        u.GID = cg.GID;
                        u.UID = us[i].ID;
                        u.UNick = us[i].Name;
                        u.LastReadID = Guid.Empty;
                        u.Ignore = 1;
                        u.IsTop = 0;
                        u.Status = 1;
                        u.CreateTime = DateTime.Now;
                        u.UpdateTime = DateTime.Now;
                        DBContext.ChatGroupUser.Add(u);
                    }
                }
                ChatGroupUser cu = new ChatGroupUser();
                cu.CGUID = Guid.NewGuid();
                cu.GID = cg.GID;
                cu.UID = MUID;
                cu.UNick = DBContext.Users.Where(m => m.ID == MUID).Select(u => u.Name).FirstOrDefault();
                cu.LastReadID = Guid.Empty;
                cu.Ignore = 1;
                cu.IsTop = 0;
                cu.LastReadID = msg.MID;
                cu.Status = 1;
                cu.CreateTime = DateTime.Now;
                cu.UpdateTime = DateTime.Now;
                DBContext.ChatGroupUser.Add(cu);
                /////

                DBContext.ChatGroup.Add(cg);
                return DBContext.SaveChanges();

            }
        }
        /// <summary>
        /// 添加群组用户
        /// </summary>
        /// <returns></returns>
        public int AddGroupUser(Guid GID, Guid UID)
        {
            if (ExistGroup(UID, GID)) return 0;//已经存在在群组
            Users u = new UsersBLL().GetUserById(UID);
            using (DBContext)
            {
                ChatGroupUser cu = new ChatGroupUser();
                cu.CGUID = Guid.NewGuid();
                cu.GID = GID;
                cu.UID = UID;
                cu.UNick = u.Name;
                cu.LastReadID = Guid.Empty;
                cu.Ignore = 1;
                cu.IsTop = 0;
                cu.Status = 1;
                cu.CreateTime = DateTime.Now;
                cu.UpdateTime = DateTime.Now;
                DBContext.ChatGroupUser.Add(cu);
                return DBContext.SaveChanges();

            }
        }
        /// <summary>
        /// 删除群组用户
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        public int RemoveGroupUser(Guid GID, Guid UID)
        {
            using (DBContext)
            {
                ChatGroupUser u = DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID == UID).FirstOrDefault();
                if (u != null)
                {
                    DBContext.ChatGroupUser.Remove(u);
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 判断用户是否在这个群里里面
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="GID"></param>
        /// <returns></returns>
        public bool ExistGroup(Guid UID, Guid GID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID == UID).Count() > 0;
            }
        }
        /// <summary>
        /// 添加好友入群
        /// </summary>
        /// <param name="UID">用户ID</param>
        /// <param name="GID">群ID</param>
        /// <returns></returns>
        public int AddFriendToGroup(Guid UID, Guid GID)
        {
            using (DBContext)
            {
                if (DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID == UID).Count() > 0)
                {
                    return 1;
                }
                ChatGroup g = DBContext.ChatGroup.Where(m => m.GID == GID).FirstOrDefault();
                if (g == null)
                {
                    return -1;
                }
                ChatGroupUser cu = new ChatGroupUser();
                cu.CGUID = Guid.NewGuid();
                cu.GID = GID;
                cu.UID = UID;
                cu.UNick = DBContext.Users.Where(m => m.ID == UID).Select(u => u.Name).FirstOrDefault();
                cu.LastReadID = Guid.Empty;
                cu.Ignore = 1;
                cu.IsTop = 0;
                cu.Status = 1;
                cu.LastReadID = g.LastMID.Value;
                cu.CreateTime = DateTime.Now;
                cu.UpdateTime = DateTime.Now;
                DBContext.ChatGroupUser.Add(cu);
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 解散群
        /// </summary>
        /// <param name="UID">用户ID</param>
        /// <param name="lstFID">好友ID</param>
        /// <returns></returns>
        public int DeleteGroup(Guid UID, Guid GID)
        {
            using (DBContext)
            {
                return 0;
            }
        }
        /// <summary>
        /// 修改群状态
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public int UpdateGroupStatus(Guid GID, enStatus status)
        {
            using (DBContext)
            {
                ChatGroup g = DBContext.ChatGroup.Where(m => m.GID == GID).FirstOrDefault();
                if (g != null)
                {
                    g.Status = (int)status;
                }
                return DBContext.SaveChanges();
            }
        }


        /// <summary>
        /// 获取我的聊天群
        /// </summary>
        /// <returns></returns>
        public List<ChatGroup> GetChatGroup(Guid UID)
        {
            using (DBContext)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取群详情
        /// </summary>
        /// <param name="GID">群id</param>
        /// <returns></returns>
        public ChatGroup GetGroupDetail(Guid GID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroup.Where(m => m.GID == GID).FirstOrDefault();
            }
        }
        /// <summary>
        /// 修改群公告
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="notice"></param>
        /// <returns></returns>
        public int UpdateGroupNotice(Guid GID, string notice)
        {
            using (DBContext)
            {
                ChatGroup group = DBContext.ChatGroup.Where(m => m.GID == GID).FirstOrDefault();
                if (group == null)
                {
                    return -1;
                }
                group.Notice = notice;
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 修改群名称
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="gname"></param>
        /// <returns></returns>
        public int UpdateGroupName(Guid GID, string gname)
        {
            using (DBContext)
            {
                ChatGroup group = DBContext.ChatGroup.Where(m => m.GID == GID).FirstOrDefault();
                if (group == null)
                {
                    return -1;
                }
                group.GName = gname;
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 获取群用户列表
        /// </summary>
        /// <returns></returns>
        public List<GroupUser> GetGroupUsers(Guid GID)
        {
            using (DBContext)
            {
                //return DBContext.ChatGroupUser.Where(m => m.GID == GID).ToList();
                return DBContext.ChatGroupUser.Where(m => m.GID == GID && m.UID != Guid.Empty).Join(DBContext.Users, g => g.UID, u => u.ID, (g, u) => new GroupUser
                {
                    GID = g.GID,
                    UID = g.UID,
                    UNick = g.UNick,
                    LastReadID = g.LastReadID,
                    Ignore = g.Ignore,
                    IsTop = g.IsTop,
                    Status = g.Status,
                    CreateTime = g.CreateTime,
                    UpdateTime = g.UpdateTime,
                    HeadImg1 = u.HeadImg1
                }).ToList();

                //return DBContext.Users.Where(u => DBContext.ChatGroupUser.Where(m => m.GID == GID).Select(m => m.UID).Contains(u.ID)).ToList();
            }
        }
        /// <summary>
        /// 获取群用户列表UID
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public List<Guid> GetGroupUsersUID(Guid GID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroupUser.Where(m => m.GID == GID).Select(u => u.UID).ToList();
            }
        }
        /// <summary>
        /// 获得群用户数量
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public int GetGroupUsersCount(Guid GID)
        {
            using (DBContext)
            {
                return DBContext.ChatGroupUser.Where(m => m.GID == GID).Select(u => u.UID).Count();
            }
        }

        /// <summary>
        /// 验证此用户有没有红包点击权限
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool CheckRedClick(Guid mid, Guid uid)
        {
            using (DBContext)
            {
                ChatMessage msg = DBContext.ChatMessage.Where(m => m.MID == mid && m.TType == (int)MessageTType.Group && (m.MType == (int)MessageType.Red || m.MType == (int)MessageType.SLRed)).FirstOrDefault();

                if (ExistGroup(uid, msg.TUID))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 验证用户有没有权限点击红包
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool CheckSingleRedClick(Guid mid, Guid uid)
        {
            using (DBContext)
            {
                ChatMessage msg = DBContext.ChatMessage.Where(m => m.MID == mid && m.TType == (int)MessageTType.User && m.MType == (int)MessageType.Red).FirstOrDefault();
                if (msg.TUID == uid)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 插入消费记录
        /// </summary>
        /// <param name="DBContext"></param>
        /// <param name="CID"></param>
        /// <param name="UID"></param>
        /// <param name="InOut"></param>
        /// <param name="PayType"></param>
        /// <param name="FromTo"></param>
        /// <param name="Val"></param>
        /// <param name="Remark"></param>
        /// <param name="PayUID">收款人id</param>
        public void InsertPayList(RelexBarEntities DBContext, Guid? CID, Guid? UID, enPayInOutType InOut, enPayType PayType, enPayFrom FromTo, decimal Val, string Remark, Guid? PayUID)
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
            model.PayUID = PayUID;
            DBContext.PayList.Add(model);
        }
        /// <summary>
        /// 转账 (不会立即到帐)
        /// </summary>
        /// <param name="uid">我的id</param>
        /// <param name="fid">好友id</param>
        /// <param name="price"></param>
        /// <returns></returns>
        public int Transfer_V2(out ErrorCode err, Guid uid, Guid fid, decimal price, ChatMessage msg, string Remark = "")
        {
            err = ErrorCode.没有错误;
            using (DBContext)
            {
                Users user = DBContext.Users.Where(m => m.ID == uid).FirstOrDefault();
                Users fuser = DBContext.Users.Where(m => m.ID == fid).FirstOrDefault();
                if (user == null || fuser == null)
                {
                    err = ErrorCode.账号不存在;
                    return (int)err;
                }
                if (user.Balance < price)
                {
                    err = ErrorCode.账户余额不足;
                    return (int)err;
                }
                user.Balance -= price;
                user.UpdateTime = DateTime.Now;
                InsertPayList(DBContext, msg.MID, uid, enPayInOutType.Out, enPayType.Coin, enPayFrom.Exchange, price, "转账", fuser.ID);
                DBContext.ChatMessage.Add(msg);
                //fuser.Balance += price;
                //fuser.UpdateTime = DateTime.Now;
                //InsertPayList(DBContext,uid,fid, enPayInOutType.In, enPayType.Coin, enPayFrom.Exchange, price, "转账",user.ID);
                return DBContext.SaveChanges();
            }
        }
        public int TransferClick(out ErrorCode err, Guid MID, Guid uid, out Guid tuid)
        {
            tuid = Guid.Empty;
            err = ErrorCode.没有错误;
            using (DBContext)
            {
                ChatMessage msg = DBContext.ChatMessage.FirstOrDefault(m => m.MID == MID);
                tuid = msg.FUID;
                if (msg == null)
                {
                    err = ErrorCode.转账记录不存在;
                }
                if (msg.Status == (int)enStatus.Unabled)
                {
                    err = ErrorCode.已领取或已退还;
                }
                Users fuser = DBContext.Users.Where(m => m.ID == msg.TUID).FirstOrDefault();
                if (fuser == null)
                {
                    err = ErrorCode.账号不存在;
                    return (int)err;
                }
                decimal price;
                if (!decimal.TryParse(msg.Content.Split('|')[1], out price))
                {
                    err = ErrorCode.金额读取失败;
                }
                fuser.Balance += price;
                fuser.UpdateTime = DateTime.Now;
                msg.HadRead = 1;
                msg.Status = (int)enStatus.Unabled;
                ChatMessage nm = new ChatMessage();
                nm.MID = Guid.NewGuid();
                nm.FUID = Guid.Empty;
                nm.TUID = tuid;
                nm.TType = (int)MessageTType.System;
                nm.MType = (int)MessageType.TransferSuccess;
                nm.Content = MID.ToString();
                nm.HadRead = 0;
                nm.IsDel = 0;
                nm.Status = 1;
                nm.CreateTime = nm.UpdateTime = DateTime.Now;
                DBContext.ChatMessage.Add(nm);
                InsertPayList(DBContext, MID, uid, enPayInOutType.In, enPayType.Coin, enPayFrom.Exchange, price, "转账", msg.FUID);

                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 转账 (不会立即到帐)
        /// </summary>
        /// <param name="uid">我的id</param>
        /// <param name="fid">好友id</param>
        /// <param name="price"></param>
        /// <returns></returns>
        public int Transfer(Guid uid, Guid fid, decimal price, string Remark = "")
        {

            using (DBContext)
            {
                Users user = DBContext.Users.Where(m => m.ID == uid).FirstOrDefault();
                Users fuser = DBContext.Users.Where(m => m.ID == fid).FirstOrDefault();
                if (user == null || fuser == null)
                {
                    return -1;
                }
                user.Balance -= price;
                user.UpdateTime = DateTime.Now;
                InsertPayList(DBContext, fid, uid, enPayInOutType.Out, enPayType.Coin, enPayFrom.Exchange, price, "转账", fuser.ID);
                fuser.Balance += price;
                fuser.UpdateTime = DateTime.Now;
                InsertPayList(DBContext, uid, fid, enPayInOutType.In, enPayType.Coin, enPayFrom.Exchange, price, "转账", user.ID);
                return DBContext.SaveChanges();
            }
        }
    }
}
