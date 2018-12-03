using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    /// <summary>
    /// 我的收藏
    /// </summary>
    public class MyCollectionBLL : BaseBll
    {
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="mid">ID</param>
        /// <param name="Mtype">收藏类型</param>
        /// <returns></returns>
        public Guid Add(Guid uid, Guid mid, enMycollectionType Mtype)
        {
            using (DBContext)
            {
                var temp = DBContext.MyCollection.FirstOrDefault(m => m.MID == mid && m.UID == uid);
                if (temp != null)
                    return temp.ID;

                MyCollection c = new MyCollection();
                c.ID = Guid.NewGuid();
                c.UID = uid;
                c.Labels = string.Empty;
                c.MID = mid;
                c.MType = (int)Mtype;
                c.Status = 1;
                c.CreateTime = c.UpdateTime = DateTime.Now;
                DBContext.MyCollection.Add(c);
                if (DBContext.SaveChanges() > 0)
                {
                    return c.ID;
                }
                return Guid.Empty;
            }
        }
        /// <summary>
        /// 获取收藏列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public dynamic GetList(Guid uid, enMycollectionType Mtype, int index, int pageSize, out int count)
        {
            using (DBContext)
            {
                if (Mtype == enMycollectionType.RedPacket)
                {
                    var q = from a in DBContext.MyCollection
                            from b in DBContext.RedPacket
                            from c in DBContext.Users
                            where a.UID == uid && a.MID == b.RID && b.UID == c.ID
                            orderby a.CreateTime descending
                            select new
                            {
                                a.ID,
                                a.UpdateTime,
                                a.Labels,
                                b.RID,
                                b.title,
                                b.imglist,
                                b.LinkTo,
                                b.ViewCount,
                                b.GoodCount,
                                b.EndTime,
                                b.UID,
                                c.TrueName,
                                c.HeadImg1,
                                c.Phone,
                                c.UserType,
                            };

                    return GetPagedList(q, pageSize, index, out count);
                }
                else if (Mtype == enMycollectionType.User)
                {
                    var q = from a in DBContext.MyCollection
                            from b in DBContext.Users
                                //from c in (
                                //            from t in DBContext.RedPacket
                                //            from t2 in(
                                //                        from p in DBContext.RedPacket
                                //                        group p by p.UID into g
                                //                        select new
                                //                        {
                                //                            g.Key,
                                //                            INDEX = g.Max(p => p.C_index)
                                //                        })
                                //            where t.C_index == t2.INDEX
                                //            select t )//用户最新发的那条红包记录？
                            join c in (
                                        from t in DBContext.RedPacket
                                        from t2 in (
                                                    from p in DBContext.RedPacket
                                                    group p by p.UID into g
                                                    select new
                                                    {
                                                        g.Key,
                                                        INDEX = g.Max(p => p.C_index)
                                                    })
                                        where t.C_index == t2.INDEX
                                        select t) on b.ID equals c.UID into temp2
                             from tt2 in temp2.DefaultIfEmpty()
                             
                            where a.UID == uid && a.MID == b.ID 
                            orderby a.CreateTime descending
                            select new
                            {
                                a.ID,
                                a.UpdateTime,
                                a.Labels,
                                UID = b.ID,
                                b.TrueName,
                                b.HeadImg1,
                                b.Phone,
                                b.UserType,
                                b.Descrition,
                                RID = tt2 == null ? Guid.Empty: tt2.RID,
                                title = tt2 == null ?"" : tt2.title,
                                imglist = tt2 == null ?"" : tt2.imglist,
                                LinkTo = tt2 == null ? "" : tt2.LinkTo,
                                TotalPrice = tt2 == null ?0: tt2.TotalPrice,
                                CreateTime = tt2 == null ? null : tt2.CreateTime,
                                EndTime = tt2 == null ? null : tt2.EndTime,
                                ViewCount = tt2 == null ? 0 : tt2.ViewCount,
                                GoodCount = tt2 == null ? 0 : tt2.GoodCount,
                                //c.RID,
                                //c.title,
                                //c.imglist,
                                //c.LinkTo,
                                //c.TotalPrice,
                                //c.CreateTime,
                                //c.EndTime,
                                //c.ViewCount,
                                //c.GoodCount,
                            };

                    return GetPagedList(q, pageSize, index, out count);
                }
                else if (Mtype == enMycollectionType.Information)
                {
                    var q = from a in DBContext.MyCollection
                            from b in DBContext.Infomations
                            from c in DBContext.Users
                            where a.UID == uid && a.MID == b.IID && b.UID == c.ID
                            orderby a.CreateTime descending
                            select new
                            {
                                a.ID,
                                a.UpdateTime,
                                a.Labels,
                                RID = b.IID,
                                b.title,
                                b.imglist,
                                b.LinkTo,
                                b.ViewCount,
                                b.GoodCount,
                                b.EndTime,
                                b.UID,
                                c.TrueName,
                                c.HeadImg1,
                                c.Phone,
                                c.UserType,
                            };

                    return GetPagedList(q, pageSize, index, out count);
                }
                else if (Mtype == enMycollectionType.Product)//收藏的商品
                {
                    var q = from a in DBContext.MyCollection
                            from b in DBContext.ProductList
                            where a.UID == uid && a.MID == b.ID
                            orderby a.CreateTime descending
                            select new
                            {
                                a.ID,
                                a.UpdateTime,
                                a.Labels,
                                ProID = b.ID,
                                Name = b.Name,
                                Title = b.Title,
                                Number = b.Number,
                                Img = b.Img,
                                PriceType = b.PriceType,
                                Price = b.Price,
                                Type = b.Type,
                                CompleteCount = b.CompleteCount,
                                GoodCount = b.GoodCount,
                                ViewCount = b.ViewCount,
                                CPoints = b.CPoints,
                            };

                    return GetPagedList(q, pageSize, index, out count);
                }
                else
                {
                    count = 0;
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取收藏列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public dynamic GetFansList(Guid uid, int pageindex, int pageSize, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.MyCollection
                        from b in DBContext.Users
                        where a.MID == uid && a.UID == b.ID
                        orderby a.CreateTime descending
                        select new
                        {
                            a.ID,
                            a.UpdateTime,
                            a.Labels,
                            UID = b.ID,
                            b.TrueName,
                            b.HeadImg1,
                            b.Phone,
                            b.UserType,
                            b.Descrition,
                        };

                return GetPagedList(q, pageSize, pageindex, out count);
            }
        }

        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            using (DBContext)
            {
                MyCollection c = DBContext.MyCollection.FirstOrDefault(m => m.ID == id);
                if (c != null)
                {
                    DBContext.MyCollection.Remove(c);
                }
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 编辑收藏说明
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public int AddLabel(Guid id, string label)
        {
            using (DBContext)
            {
                MyCollection c = DBContext.MyCollection.FirstOrDefault(m => m.ID == id);
                if (c == null)
                {
                    return 0;
                }

                if (string.IsNullOrEmpty(c.Labels))
                {
                    c.Labels += label;
                }
                else
                {
                    string[] t = c.Labels.Split(',');
                    for (int i = 0; i < t.Length; i++)
                    {
                        if (t[i] == label)
                        {
                            return 0;
                        }
                    }
                    c.Labels += string.Format(",{0}", label);
                }

                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 编辑收藏标签
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldLabel"></param>
        /// <param name="newLabel"></param>
        /// <returns></returns>
        public int EditLabel(Guid id, string oldLabel, string newLabel)
        {
            using (DBContext)
            {
                MyCollection c = DBContext.MyCollection.FirstOrDefault(m => m.ID == id);
                if (c == null)
                {
                    return 0;
                }
                string[] t = c.Labels.Split(',');
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i] == newLabel)
                    {
                        return 0;
                    }
                }
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i] == oldLabel)
                    {
                        t[i] = newLabel;
                        break;
                    }
                }
                c.Labels = string.Join(",", t);

                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 删除收藏标签
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public int DeleteLabel(Guid id, string label)
        {
            using (DBContext)
            {
                MyCollection c = DBContext.MyCollection.FirstOrDefault(m => m.ID == id);
                if (c == null)
                {
                    return 0;
                }
                if (c.Labels.IndexOf(string.Format(",{0}", label)) != -1)
                {
                    c.Labels = c.Labels.Replace(string.Format(",{0}", label), "");
                }
                else
                {
                    if (c.Labels.IndexOf(string.Format("{0},", label)) != -1)
                    {
                        c.Labels = c.Labels.Replace(string.Format("{0},"), "");
                    }
                }
                return DBContext.SaveChanges();
            }
        }

        public int GetCollectTotal(Guid? UID, Guid? MID, enStatus? status, enMycollectionType? MType)
        {
            using (DBContext)
            {
                
                return DBContext.MyCollection.Count(m => m.UID == UID && m.MID == MID && m.Status == (int)status && m.MType == (int)MType);
            }
        }

        public MyCollection GetMyCollect(Guid? UID, Guid? MID, enStatus? status, enMycollectionType? MType)
        {
            using (DBContext)
            {
               
                return DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == MID && m.Status == (int)status && m.MType == (int)MType);
            }
        }
    }
}
