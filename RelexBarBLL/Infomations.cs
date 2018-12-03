using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    /// <summary>
    /// 商圈类
    /// </summary>
    public class InfomationsBLL : BaseBll
    {
        decimal PublishScore = 100;//100积分

        /// <summary>
        /// 发布商圈
        /// </summary>
        public int Publish(Guid UID, int type, string title, string imglist, string LinkTo)
        {
            using (DBContext)
            {
                var u = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (u.Score <= PublishScore)
                {
                    return (int)ErrorCode.账户积分不足;
                }
                u.Score -= PublishScore;

                Infomations model = new Infomations();
                model.IID = Guid.NewGuid();
                model.UID = UID;
                model.Type = type;
                model.BeginTime = DateTime.Now;
                model.EndTime = DateTime.Now.AddDays(7);//7天后过期时间
                model.title = title;
                model.imglist = imglist;
                model.LinkTo = LinkTo;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;

                DBContext.Infomations.Add(model);

                PayListBLL.Insert(DBContext, model.IID, UID, enPayInOutType.Out, enPayType.Point, enPayFrom.Infomations, PublishScore, "发布商圈广告");

                return DBContext.SaveChanges();
            }
        }

        public int EditPublish(Guid? id, string title, int type, string imglist, string LinkTo)
        {
            using (DBContext)
            {
                Infomations v = DBContext.Infomations.FirstOrDefault(m => m.IID == id);
                if (v == null)
                    return (int)ErrorCode.数据不存在;
                v.title = title;
                v.Type = type;
                v.BeginTime = DateTime.Now;
                v.EndTime = DateTime.Now.AddDays(7);//7天后过期时间
                v.imglist = imglist;
                v.LinkTo = LinkTo;
                v.UpdateTime = DateTime.Now;
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 获取商圈列表
        /// </summary>
        /// <returns></returns>
        public dynamic GetList(int? type, int pagesize, int pageinex, Guid UID, out int count)
        {
            using (DBContext)
            {
                var dt = DateTime.Now;
                if (type == null)
                {
                    type = DBContext.InfomationType.FirstOrDefault().ITID;
                }
                var q = from a in DBContext.Infomations
                        from c in DBContext.Users
                        where a.Status == (int)enStatus.Enabled && a.UID == c.ID && a.Type == type && a.BeginTime <= dt && a.EndTime >= dt
                        select new
                        {
                            a.IID,
                            a.title,
                            a.imglist,
                            a.LinkTo,
                            a.Type,
                            a.UID,
                            c.TrueName,
                            c.Phone,
                            c.HeadImg1,
                            c.Descrition,
                            a.GoodCount,
                            a.ViewCount,
                            a.CreateTime,
                            IsColletion = DBContext.MyCollection.Count(m => m.UID == UID && m.MID == a.IID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.Information) == 0 ? "0" : "1", //福音收藏状态（0未收藏，1已收藏） 
                            CollectionID = DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.IID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.Information) == null ? "0" : DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.IID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.Information).ID.ToString(), //收藏id
                            IsGoodFor = DBContext.InfomationViewList.Count(m => m.UID == UID && m.IID == a.IID && m.Type == (int)enViewGood.Good) == 0 ? "0" : "1", //点赞状态（0未点赞，1已收藏）
                            GoodID = DBContext.InfomationViewList.FirstOrDefault(m => m.UID == UID && m.IID == a.IID && m.Status == (int)enStatus.Enabled && m.Type == (int)enViewGood.Good) == null ? "0" : DBContext.InfomationViewList.FirstOrDefault(m => m.UID == UID && m.IID == a.IID && m.Status == (int)enStatus.Enabled && m.Type == (int)enViewGood.Good).IVID.ToString(), //点赞id
                        };

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }

        public dynamic GetListByUID(Guid?UID,int pagesize,int pageindex,out int count)
        {
            using (DBContext)
            {
                var dt = DateTime.Now;
                var q = from a in DBContext.Infomations
                        from c in DBContext.Users
                        where a.Status == (int)enStatus.Enabled && a.UID == UID && a.UID==c.ID && a.BeginTime <= dt && a.EndTime >= dt
                        select new
                        {
                            a.IID,
                            a.title,
                            a.imglist,
                            a.LinkTo,
                            a.Type,
                            a.UID,
                            c.TrueName,
                            c.Phone,
                            c.HeadImg1,
                            a.GoodCount,
                            a.ViewCount,
                            a.CreateTime,
                        };
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageindex, out count);
            }
        }

        /// <summary>
        /// 获取商圈详情
        /// </summary>
        /// <returns></returns>
        public dynamic ViewInfo(Guid ID, Guid UID, ref bool IsFirstTime)
        {
            using (DBContext)
            {
                var model = DBContext.Infomations.FirstOrDefault(m => m.IID == ID);
                if (model == null)
                    return null;
                model.ViewCount++;

                IsFirstTime = DBContext.InfomationViewList.Count(m => m.UID == UID && m.IID == ID && m.Type == (int)enViewGood.View) == 0;//是否第一次看该广告

                DateTime nowDate = DateTime.Now.Date;
                var view = new InfomationViewList();
                view.CreateTime = view.UpdateTime = DateTime.Now;
                view.UID = UID;
                view.IID = ID;
                view.Type = (int)enViewGood.View;
                DBContext.InfomationViewList.Add(view);

                var u = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (u != null)
                {
                    var count = DBContext.InfomationViewList.Count(m => m.UID == UID && m.Type == (int)enViewGood.View && m.CreateTime >= nowDate);
                    if (count < 5) //是否当日已封顶
                    {
                        u.Score += 1;//看广告每次1分，每天封顶5分
                        u.TotalScore += 1;

                        PayListBLL.Insert(DBContext, model.IID, u.ID, enPayInOutType.In, enPayType.Point, enPayFrom.Infomations, 1, "浏览商圈广告");
                    }
                }
                DBContext.SaveChanges();

                var q = from a in DBContext.Infomations
                        from c in DBContext.Users
                        where a.Status == (int)enStatus.Enabled && a.UID == c.ID && a.IID == ID
                        select new
                        {
                            a.IID,
                            a.title,
                            a.imglist,
                            a.LinkTo,
                            a.Type,
                            a.UID,
                            c.TrueName,
                            c.Phone,
                            c.HeadImg1,
                            c.Descrition,
                            a.GoodCount,
                            a.ViewCount,
                            a.CreateTime,

                            IsColletion = DBContext.MyCollection.Count(m => m.UID == UID && m.MID == a.IID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.Information) == 0 ? "0" : "1", //福音收藏状态（0未收藏，1已收藏） 
                            CollectionID = DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.IID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.Information) == null ? "0" : DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.IID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.Information).ID.ToString(), //收藏id
                            IsGoodFor = DBContext.InfomationViewList.Count(m => m.UID == UID && m.IID == a.IID && m.Type == (int)enViewGood.Good) == 0 ? "0" : "1", //点赞状态（0未点赞，1已收藏）
                            GoodID = DBContext.InfomationViewList.FirstOrDefault(m => m.UID == UID && m.IID == a.IID && m.Status == (int)enStatus.Enabled && m.Type == (int)enViewGood.Good) == null ? "0" : DBContext.InfomationViewList.FirstOrDefault(m => m.UID == UID && m.IID == a.IID && m.Status == (int)enStatus.Enabled && m.Type == (int)enViewGood.Good).IVID.ToString(), //点赞id
                        };

                var result = q.FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// 点赞商圈
        /// </summary>
        /// <returns></returns>
        public int Good(Guid ID, Guid UID)
        {
            using (DBContext)
            {
                var model = DBContext.Infomations.FirstOrDefault(m => m.IID == ID);
                if (model == null)
                    return (int)ErrorCode.数据不存在;
                var good = DBContext.InfomationViewList.FirstOrDefault(m => m.UID == UID && m.IID == ID && m.Type == (int)enViewGood.Good);
                if (good != null)
                    return (int)ErrorCode.您已经点赞过;

                model.GoodCount++;

                DateTime nowDate = DateTime.Now.Date;
                var view = new InfomationViewList();
                view.CreateTime = view.UpdateTime = DateTime.Now;
                view.UID = UID;
                view.IID = ID;
                view.Type = (int)enViewGood.Good;
                DBContext.InfomationViewList.Add(view);

                return DBContext.SaveChanges();
            }
        }


        public InfomationViewList GetGood(Guid? UID, Guid? IID, enViewGood? Good)
        {
            using (DBContext)
            {
                return DBContext.InfomationViewList.FirstOrDefault(m => m.UID == UID && m.IID == IID && m.Type == (int)Good);
            }
        }

        public int GetGoodTotal(Guid? UID, Guid? IID, enViewGood? Good)
        {
            using (DBContext)
            {

                return DBContext.InfomationViewList.Count(m => m.UID == UID && m.IID == IID  && m.Type == (int)Good);
            }
        }


        public List<Infomations> GetInfomationsSearch(string key, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.Infomations.Where(m => m.IID !=Guid.Empty);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.title.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }

        public List< Models.InfomationsModel> GetInfomationsSearch(string key, DateTime? beginTime, DateTime? endTime,int pageSize,int pageIndex,out int sum)
        {
            using (DBContext)
            {
                var q = from a in DBContext.Infomations
                        select new Models.InfomationsModel
                        {
                          
                            Name = (a.UID!=Guid.Empty? DBContext.Users.FirstOrDefault(m => m.ID == a.UID).Phone : "系统"),
                            HeadImg = (a.UID != Guid.Empty ? DBContext.Users.FirstOrDefault(m => m.ID == a.UID).HeadImg1 : "/homecss/image/logo.png"),
                            Phone = DBContext.Users.FirstOrDefault(m => m.ID == a.UID).Phone,
                            TrueName=DBContext.Users.FirstOrDefault(m=>m.ID==a.UID).TrueName,
                            CardNumber= DBContext.Users.FirstOrDefault(m => m.ID == a.UID).CardNumber,
                            IID = a.IID,
                            UID = a.UID,
                            Type = a.Type,
                            title = a.title,
                            imglist = a.imglist,
                            LinkTo = a.LinkTo,
                            ViewCount = a.ViewCount,
                            GoodCount = a.GoodCount,
                            AreaX = a.AreaX,
                            AreaY = a.AreaY,
                            AreaLimit = a.AreaLimit,
                            Status = a.Status,
                            BeginTime = a.BeginTime,
                            EndTime = a.EndTime,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime
                        };
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key) || m.title.Contains(key));
                }
               
                if (beginTime.HasValue)
                {
                    q = q.Where(m => m.CreateTime > beginTime);
                }
                if (endTime.HasValue)
                {
                    DateTime end = endTime.Value.AddDays(1);
                    q = q.Where(m => m.CreateTime < end);
                }

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }

        public List<Models.InfomationsModel> GetInfomationsExcel(string key, DateTime? beginTime, DateTime? endTime)
        {
            using (DBContext)
            {
                var q = from a in DBContext.Infomations
                        select new Models.InfomationsModel
                        {

                            Name = (a.UID != Guid.Empty ? DBContext.Users.FirstOrDefault(m => m.ID == a.UID).Phone : "系统"),
                            Phone = DBContext.Users.FirstOrDefault(m => m.ID == a.UID).Phone,
                            TrueName = DBContext.Users.FirstOrDefault(m => m.ID == a.UID).TrueName,
                            IID = a.IID,
                            UID = a.UID,
                            Type = a.Type,
                            title = a.title,
                            imglist = a.imglist,
                            LinkTo = a.LinkTo,
                            ViewCount = a.ViewCount,
                            GoodCount = a.GoodCount,
                            AreaX = a.AreaX,
                            AreaY = a.AreaY,
                            AreaLimit = a.AreaLimit,
                            Status = a.Status,
                            BeginTime = a.BeginTime,
                            EndTime = a.EndTime,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime
                        };
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key) || m.title.Contains(key));
                }

                if (beginTime.HasValue)
                {
                    q = q.Where(m => m.CreateTime > beginTime);
                }
                if (endTime.HasValue)
                {
                    DateTime end = endTime.Value.AddDays(1);
                    q = q.Where(m => m.CreateTime < end);
                }

                return q.OrderByDescending(m => m.CreateTime).ToList();
            }
        }


        public Infomations GetInfomationById(Guid id)
        {
            using (DBContext)
            {
                return DBContext.Infomations.FirstOrDefault(m => m.IID == id);
            }
        }
        
        public int ChangeInfomationStatus(Guid IID, enStatus status)
        {
            using (DBContext)
            {
                Infomations type = DBContext.Infomations.Where(m => m.IID == IID).FirstOrDefault();
                if (type != null)
                {
                    type.Status = (int)status;
                    type.UpdateTime = DateTime.Now;
                }
                //return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更新福音天地状态成功,名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, type.title,(int)status==1? "启用" : "禁用"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更新福音天地状态失败,名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, type.title, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 系统发布商圈
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SystemPublish(Infomations model)
        {
            using (DBContext)
            {
                DBContext.Infomations.Add(model);
              //  return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增福音天地成功,名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.title), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增福音天地失败,名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.title), enLogType.Admin);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 系统更新商圈
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Name"></param>
        /// <param name="Desr"></param>
        /// <returns></returns>
        public int UpdatePublish(Guid? id, string title, int type, string imglist, string LinkTo, int GoodCount, int ViewCount, DateTime? BeginTime, DateTime? EndTime)
        {
            using (DBContext)
            {
                Infomations v = DBContext.Infomations.FirstOrDefault(m => m.IID == id);
                if (v == null)
                {
                    return -1;
                }
                else
                {
                    v.title = title;
                    v.Type = type;
                    v.imglist = imglist;
                    v.LinkTo = LinkTo;
                    v.GoodCount = GoodCount;
                    v.ViewCount = ViewCount;
                    v.BeginTime = BeginTime;
                    v.EndTime = EndTime;
                    v.UpdateTime = DateTime.Now;
                   // return DBContext.SaveChanges();
                    try
                    {
                        int i = DBContext.SaveChanges();
                        if (i > 0)
                        {
                            logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改福音天地成功,名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, title), enLogType.Admin);
                        }
                        return i;
                    }
                    catch (Exception)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改福音天地失败,名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, title), enLogType.Admin);
                        return 0;
                    }
                }
            }
        }




        #region 类型
        /// <summary>
        /// 获取广告类型
        /// </summary>
        /// <returns></returns>
        public List<InfomationType> GetInfomationTypes()
        {
            using (DBContext)
            {
                return DBContext.InfomationType.Where(m => m.Status == (int)enStatus.Enabled).ToList();
            }
        }
        /// <summary>
        /// 分页获取广告类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<InfomationType> GetInfomationTypesSearch(string key, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.InfomationType.Where(m => m.ITID !=0);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }

        public InfomationType GetInfomationTypeById(int id)
        {
            using (DBContext)
            {
                return DBContext.InfomationType.FirstOrDefault(m => m.ITID == id);
            }
        }
        /// <summary>
        /// 添加InfomationsType
        /// </summary>
        /// <returns></returns>
        public int Add(InfomationType model)
        {
            using (DBContext)
            {
                DBContext.InfomationType.Add(model);
                //return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增福音天地类型成功,福音天地类型名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Name), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增福音天地类型失败,福音天地类型名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Name), enLogType.Admin);
                    return 0;
                }
            }
        }
        public int Update(int id, string Name, string Desr)
        {
            using (DBContext)
            {
                InfomationType v = DBContext.InfomationType.FirstOrDefault(m => m.ITID == id);
                if (v == null)
                {
                    return -1;
                }
                else
                {
                    v.Name = Name;
                    v.Desr = Desr;
                    v.UpdateTime = DateTime.Now;
                  //  return DBContext.SaveChanges();
                    try
                    {
                        int i = DBContext.SaveChanges();
                        if (i > 0)
                        {
                            logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改福音天地类型成功,福音天地类型名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, Name), enLogType.Admin);
                        }
                        return i;
                    }
                    catch (Exception)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改福音天地类型失败,福音天地类型名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, Name), enLogType.Admin);
                        return 0;
                    }
                }
            }
        }

        public int ChangeInfomationTypeStatus(int ITID, enStatus status)
        {
            using (DBContext)
            {
                InfomationType type = DBContext.InfomationType.Where(m => m.ITID == ITID).FirstOrDefault();
                if (type != null)
                {
                    type.Status = (int)status;
                    type.UpdateTime = DateTime.Now;
                }
                //return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更新福音天地类型状态成功,福音天地类型名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, type.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更新福音天地类型状态失败,福音天地类型名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, type.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 添加InfomationsType
        /// </summary>
        /// <returns></returns>
        public int InsertType()
        {
            using (DBContext)
            {
                return 0;
            }
        }

        #endregion
        #region 评论

        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        public int Comment(Guid IID, Guid UID, string content)
        {
            using (DBContext)
            {
                InfomationComment rpc = new InfomationComment();
                rpc.CID = Guid.NewGuid();
                rpc.Content = content;
                rpc.IID = IID;
                rpc.UID = UID;
                rpc.Type = 0;
                rpc.Status = (int)enStatus.Enabled;
                rpc.CreateTime = rpc.UpdateTime = DateTime.Now;
                DBContext.InfomationComment.Add(rpc);
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        public dynamic GetComments(Guid IID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.InfomationComment
                        from b in DBContext.Users
                        where a.UID == b.ID && a.IID == IID && a.Status == (int)enStatus.Enabled
                        orderby a.UpdateTime descending
                        select new
                        {
                            a.UID,
                            b.Name,
                            b.Phone,
                            b.TrueName,
                            b.HeadImg1,
                            a.Content,
                            a.UpdateTime
                        };
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }


        public List<RelexBarBLL.Models.InfomationsCommentModel> GetCommentslist(Guid IID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.InfomationComment
                        from b in DBContext.Users
                        where a.UID == b.ID && a.IID == IID && a.Status == (int)enStatus.Enabled
                        orderby a.UpdateTime descending
                        select  new RelexBarBLL.Models.InfomationsCommentModel
                        {
                           UID=  a.UID,
                          Name=  b.Name,
                           Phone= b.Phone,
                           TrueName= b.TrueName,
                          HeadImg1=  b.HeadImg1,
                           Content= a.Content,
                           UpdateTime= a.UpdateTime
                        };
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        #endregion
    }
}
