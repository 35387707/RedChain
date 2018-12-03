using RelexBarBLL.Models;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    public class GCZoneBLL: BaseBll
    {
        public enum enGCZoneType {
            text=0,
            img=1
        }
        public int Add(Guid uid,string content,string location,enGCZoneType type=enGCZoneType.text, Guid? id = null) {
            try
            {
                using (DBContext)
                {
                    DateTime now = DateTime.Now;
                    //添加一个朋友圈实体
                    GCZone g = new GCZone();
                    g.ID = id == null ? Guid.NewGuid() : id.Value;
                    g.UID = uid;
                    g.IsDel = 0;
                    g.Type = (int)type;
                    g.Status = 1;
                    g.CreateTime = g.UpdateTime = now;
                    g.Content = content;
                    g.Location = location;
                    DBContext.GCZone.Add(g);
                    //添加一个时间轴
                    GCZoneTimeLine myTimeLine = new GCZoneTimeLine();
                    myTimeLine.ID = Guid.NewGuid();
                    myTimeLine.UID = uid;
                    myTimeLine.GCZID = g.ID;
                    myTimeLine.Is_own = 1;
                    myTimeLine.IsRead = 0;
                    myTimeLine.CreateTime = myTimeLine.UpdateTime = now;
                    DBContext.GCZoneTimeLine.Add(myTimeLine);
                    //查询朋友id
                    List<Guid> friendList = DBContext.FriendShip.Where(m => m.UID == uid && m.IsDel == 0 && m.Status == 1).Select(m => m.FriendID).ToList();
                    //给朋友时间轴添加实体
                    foreach (var item in friendList)
                    {
                        GCZoneTimeLine oTimeLine = new GCZoneTimeLine();
                        oTimeLine.ID = Guid.NewGuid();
                        oTimeLine.UID = item;
                        oTimeLine.GCZID = g.ID;
                        oTimeLine.Is_own = 0;
                        oTimeLine.IsRead = 0;
                        oTimeLine.CreateTime = oTimeLine.UpdateTime = now;
                        DBContext.GCZoneTimeLine.Add(oTimeLine);
                    }


                    return DBContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                LogsBLL.InsertAPILog("/GCZone/Add",uid,e.ToString());
                return -1;
            }
           
        }
        public int AddComment(Guid uid,Guid id, Guid? cid, string content,Guid? primaryID=null) {
            using (DBContext) {
                try
                {
                    GCZoneComment c = new GCZoneComment();
                    c.UID = uid;
                    c.ID =primaryID==null?Guid.NewGuid():primaryID.Value;
                    c.GCZID = id;
                    c.ReplyID = cid;
                    c.Status = 1;
                    c.CreateTime = c.UpdateTime=DateTime.Now;
                    c.Content = content;
                    DBContext.GCZoneComment.Add(c);
                    return DBContext.SaveChanges();
                }
                catch (Exception e)
                {
                    LogsBLL.InsertAPILog("/GCZone/AddComment", uid, e.ToString());
                    return -1;
                }
                
            }
        }
        //public int DeleteComment(Guid gid) { }
        public List<GCZoneModel> GetList(Guid uid, bool is_own, DateTime? date, int pageSize)
        {
            try
            {
                using (DBContext)
                {
                    //将时间轴消息改为已读
                    DBContext.Database.ExecuteSqlCommand(string.Format("update GCZoneTimeLine set IsRead=1 where UID='{0}' and IsRead=0", uid));
                    StringBuilder sql = new StringBuilder();

                    sql.Append("select g.ID,g.UID,g.Content,g.IsDel,g.Status,g.Location,g.CreateTime,g.UpdateTime,u.Name UserName,u.HeadImg1 HeadImg from(");
                    sql.Append("select t.GCZID from (");
                    sql.Append("select top {0} tl.GCZID from GCZoneTimeLine tl");
                    
                    sql.Append(" where tl.UID='{1}'");
                    if (date != null)
                    {
                        sql.Append(string.Format(" and tl.UpdateTime<CONVERT(datetime,'{0}')",date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    }
                    if (is_own)
                    {
                        sql.Append(" and tl.Is_own=1");
                    }
                    sql.Append(" order by CreateTime desc");
                    
                    sql.Append(") as t");
                    sql.Append(") as t2 left join GCZone g on t2.GCZID=g.ID left join Users u on g.UID=u.ID order by g.CreateTime desc");


                    /*
                    sql.Append("select g.ID,g.UID,g.Content,g.IsDel,g.Status,g.Location,g.CreateTime,g.UpdateTime,u.Name UserName,u.HeadImg1 HeadImg from(");
                    sql.Append("select t.rnum,t.GCZID from (");
                    sql.Append("select ROW_NUMBER() over(order by tl.createtime desc) rnum,tl.GCZID from GCZoneTimeLine tl");
                    sql.Append(" where tl.UID='{0}'");
                    if (is_own)
                    {
                        sql.Append(" and tl.Is_own=1");
                    }
                    sql.Append(") as t where t.rnum>{1} and t.rnum<={2}");
                    sql.Append(") as t2 left join GCZone g on t2.GCZID=g.ID left join Users u on g.UID=u.ID order by g.CreateTime desc");
                    */

                    List<GCZoneModel> list = DBContext.Database.SqlQuery<GCZoneModel>(string.Format(sql.ToString(),pageSize, uid)).ToList();
                    string commentsql = "select t.ID,t.GCZID,t.ReplyID,t.Status,t.CreateTime,t.UpdateTime,UID,Content,u.Name UserName from ("
                        + "select ID,GCZID,ReplyID,Status,CreateTime,UpdateTime,UID,Content from GCZoneComment c "
                        + "where c.GCZID='{0}') as t left join Users u on t.UID=u.ID order by t.CreateTime desc";
                    string likesql = "select t.ID,t.GCZID,t.UID,t.CreateTime,t.UpdateTime,u.Name UserName from ("
                        + "select ID,GCZID,UID,CreateTime,UpdateTime from GCZoneLike"
                        + " where GCZID='{0}') as t left join Users u on t.UID=u.ID";

                    foreach (var item in list)
                    {
                        Guid gczid = item.ID;
                        item.GCZoneComment = DBContext.Database.SqlQuery<GCZoneCommentModel>(string.Format(commentsql, gczid)).ToList();

                        item.IsLikeList = DBContext.Database.SqlQuery<GCZoneLikeModel>(string.Format(likesql,gczid)).ToList();
                    }
                    return list;
                }

            }
            catch (Exception e)
            {

                LogsBLL.InsertAPILog("/GCZone/GetList", uid, e.ToString());
                return null;
            }

        }
        /*
    public List<GCZoneModel> GetList(Guid uid,bool is_own,int index,int pageSize) {
        try
        {
            using (DBContext)
            {
                //将时间轴消息改为已读
                DBContext.Database.ExecuteSqlCommand(string.Format("update GCZoneTimeLine set IsRead=1 where UID='{0}' and IsRead=0", uid));
                StringBuilder sql = new StringBuilder();

                sql.Append("select g.ID,g.UID,g.Content,g.IsDel,g.Status,g.Location,g.CreateTime,g.UpdateTime,u.Name UserName,u.HeadImg1 HeadImg from(");
                sql.Append("select t.rnum,t.GCZID from (");
                sql.Append("select ROW_NUMBER() over(order by tl.createtime desc) rnum,tl.GCZID from GCZoneTimeLine tl");
                sql.Append(" where tl.UID='{0}'");
                if (is_own) {
                    sql.Append(" and tl.Is_own=1");
                }
                sql.Append(") as t where t.rnum>{1} and t.rnum<={2}");
                sql.Append(") as t2 left join GCZone g on t2.GCZID=g.ID left join Users u on g.UID=u.ID order by g.CreateTime desc");


                List<GCZoneModel> list = DBContext.Database.SqlQuery<GCZoneModel>(string.Format(sql.ToString(), uid, (index - 1) * pageSize, index * pageSize)).ToList();
                string commentsql = "select t.ID,t.GCZID,t.ReplyID,t.Status,t.CreateTime,t.UpdateTime,UID,Content,u.Name UserName from ("
                    + "select ID,GCZID,ReplyID,Status,CreateTime,UpdateTime,UID,Content from GCZoneComment c "
                    + "where c.GCZID='{0}') as t left join Users u on t.UID=u.ID order by t.CreateTime desc";
                foreach (var item in list)
                {
                    Guid gczid = item.ID;
                    item.GCZoneComment =DBContext.Database.SqlQuery<GCZoneCommentModel>(string.Format(commentsql,gczid)).ToList();

                    item.IsLikeList = DBContext.GCZoneLike.Where(m => m.GCZID == gczid).ToList();
                }
                return list;
            }

        }
        catch (Exception e)
        {

            LogsBLL.InsertAPILog("/GCZone/GetList", uid, e.ToString());
            return null;
        }

    }
    */

        /// <summary>
        /// 是否有新的朋友圈信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool HasNewMessage(Guid uid) {
            using (DBContext) {
                return DBContext.GCZoneTimeLine.Where(m => m.UID == uid && m.IsRead == 0).Count() > 0;
            }
        }


        #region 点赞相关
        public int IsLike(Guid GCZID, Guid uid,Guid? primaryID=null) {
            using (DBContext) {
                if (DBContext.GCZoneLike.Count(m=>m.GCZID==GCZID&&m.UID==uid)>0) {
                    return -1;
                }
                GCZoneLike l = new GCZoneLike();
                l.ID = primaryID == null ? Guid.NewGuid() : primaryID.Value;
                l.GCZID = GCZID;
                l.UID = uid;
                l.CreateTime = l.UpdateTime = DateTime.Now;
                DBContext.GCZoneLike.Add(l);
                return DBContext.SaveChanges();
            }
        }
        public int DeleteIsLike(Guid GCZID, Guid uid) {
            using (DBContext) {
                return DBContext.Database.ExecuteSqlCommand(string.Format("delete from GCZoneLike where gczid='{0}' and uid='{1}'",GCZID,uid));
            }
        }
        #endregion

        public int SetPhotoCover(Guid uid,string path) {
            using (DBContext) {
                Users u = DBContext.Users.FirstOrDefault(m=>m.ID==uid);
                if (u!=null) {
                    u.HeadImg2 = path;
                }
                return DBContext.SaveChanges();
            }
        }
        public string GetPhotoCover(Guid uid) {
            using (DBContext) {
                return DBContext.Users.Where(m => m.ID == uid).Select(m => m.HeadImg2).FirstOrDefault();
            }
        }
    }
}
