using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    public partial class UserMsgBLL : BaseBll
    {
        public List<UserMsg> GetAllList(Guid Uid, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.UserMsg.Where(m => (m.UID == Uid || m.UID == Guid.Empty) && m.IsShow == 1 && m.Status != (int)enMessageState.Unabled);
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }

        public List<UserMsg> GetAllListByType(string key, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.UserMsg.Where(m => (m.UID == Guid.Empty) && m.MessageType == (int)enMessageType.Other);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Title.Contains(key) || m.Subject.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }


        public dynamic GetAllList(string key, int? type, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.UserMsg
                        join b in DBContext.Users on a.UID equals b.ID into T1
                        from c in T1.DefaultIfEmpty()
                        join d in DBContext.Users on a.FromUid equals d.ID into T2
                        from e in T1.DefaultIfEmpty()
                        select new
                        {
                            ID = a.ID,
                            UID = a.UID,
                            FromUid = a.FromUid,
                            Subject = a.Subject,
                            Content = a.Content,
                            MessageType = a.MessageType,
                            Img = a.Img,
                            Title = a.Title,
                            IsShow = a.IsShow,
                            Status = a.Status,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime,

                            UName = c == null ? "所有人" : c.Phone + "【" + c.CardNumber + "】",
                            FName = e == null ? "系统" : e.Phone + "【" + e.CardNumber + "】",
                        };

                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.UName.Contains(key) || m.FName.Contains(key) || m.Subject.Contains(key) ||
                    m.Content.Contains(key) || m.Title.Contains(key));
                }
                if (type.HasValue)
                {
                    q = q.Where(m => m.MessageType == type.Value);
                }

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }


        public List<Models.UserMsgModel> GetUserMsgAllList(string key, int? type, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.UserMsg
                        join b in DBContext.Users on a.UID equals b.ID into T1
                        from c in T1.DefaultIfEmpty()
                        join d in DBContext.Users on a.FromUid equals d.ID into T2
                        from e in T2.DefaultIfEmpty()
                        select new Models.UserMsgModel
                        {
                            ID = a.ID,
                            UID = a.UID,
                            FromUid = a.FromUid,
                            Subject = a.Subject,
                            Content = a.Content,
                            MessageType = a.MessageType,
                            Img = a.Img,
                            Title = a.Title,
                            IsShow = a.IsShow,
                            Status = a.Status,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime,

                            UName = c == null ? "所有人" : c.Phone + "【" + c.CardNumber + "】",
                            FName = e == null ? "系统" : e.Phone + "【" + e.CardNumber + "】",
                        };

                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.UName.Contains(key) || m.FName.Contains(key) || m.Subject.Contains(key) ||
                    m.Content.Contains(key) || m.Title.Contains(key));
                }
                if (type.HasValue)
                {
                    q = q.Where(m => m.MessageType == type.Value);
                }

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }

        public int GetNoReadCount(Guid Uid)
        {
            using (DBContext)
            {
                return DBContext.UserMsg.Count(m => m.IsShow == 1 && m.UID == Uid && m.Status == (int)enMessageState.Enabled);
            }
        }

        public UserMsg GetLastNews(Guid Uid, out int count, out int realStatus)
        {
            using (DBContext)
            {
                realStatus = (int)enMessageState.Unabled;
                count = DBContext.UserMsg.Count(m => m.IsShow == 1 && m.UID == Uid && m.Status == (int)enMessageState.Enabled);
                var model = DBContext.UserMsg.OrderByDescending(m => m.CreateTime).FirstOrDefault(m => m.IsShow == 1 && m.UID == Uid);

                if (model != null && model.Status == (int)enMessageState.Enabled)
                {
                    realStatus = model.Status;
                    model.Status = (int)enMessageState.HadRead;
                    model.UpdateTime = DateTime.Now;
                    DBContext.SaveChanges();
                }
                return model;
            }
        }

        public int Insert(Guid ToUid, Guid FromUid, string subject, string content, enMessageType type, string Img, string Title)
        {
            using (DBContext)
            {
                UserMsg msg = new UserMsg();
                msg.ID = Guid.NewGuid();
                msg.Img = Img;
                msg.UID = ToUid;
                msg.FromUid = FromUid;
                msg.Subject = subject;
                msg.Content = content;
                msg.MessageType = (int)type;
                msg.Title = Title;
                msg.Status = (int)enStatus.Enabled;
                msg.IsShow = (int)enStatus.Enabled;

                msg.CreateTime = msg.UpdateTime = DateTime.Now;

                DBContext.UserMsg.Add(msg);
                return DBContext.SaveChanges();
            }
        }

        public UserMsg GetDetail(Guid ID)
        {
            using (DBContext)
            {
                UserMsg model = DBContext.UserMsg.FirstOrDefault(m => m.ID == ID);
                if (model != null && model.Status == (int)enMessageState.Enabled)
                {
                    model.Status = (int)enMessageState.HadRead;
                    model.UpdateTime = DateTime.Now;
                    DBContext.SaveChanges();
                }
                return model;
            }
        }

        public int UpdateMsg(UserMsg model)
        {
            using (DBContext)
            {
                DBContext.UserMsg.Attach(model);
                DBContext.Entry<UserMsg>(model).State = System.Data.Entity.EntityState.Modified;
                return DBContext.SaveChanges();
            }
        }

        public UserMsg GetUserMsgById(Guid id)
        {
            using (DBContext)
            {
                return DBContext.UserMsg.FirstOrDefault(m => m.ID == id);
            }
        }
        /// <summary>
        /// 更新是否显示
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateShow(Guid ID, enStatus status)
        {
            using (DBContext)
            {
                UserMsg msg = DBContext.UserMsg.Where(m => m.ID == ID).FirstOrDefault();
                if (msg != null)
                {
                    msg.IsShow = (int)status;
                    msg.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 更新已读状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateStatus(Guid ID)
        {
            using (DBContext)
            {
                UserMsg msg = DBContext.UserMsg.Where(m => m.ID == ID).FirstOrDefault();
                if (msg != null)
                {
                   // msg.Status = (int)status;
                    msg.Status = (int)enMessageState.HadRead;
                    msg.UpdateTime = DateTime.Now;
                }
              //  return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},查阅系统通知成功, 描述:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, msg.Content, "已读"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},查阅系统通知失败, 描述:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, msg.Content, "已读"), enLogType.Admin);
                    return 0;
                }
            }
        }



    }
}
