using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    public partial class AdsListBLL : BaseBll
    {
        public List<AdsList> GetUserAdsList()
        {
            using (DBContext)
            {
                return DBContext.AdsList.Where(m => m.Status == (int)enStatus.Enabled).OrderByDescending(m => m.UpdateTime).ToList();
            }
        }
        public List<AdsList> GetList()
        {
            using (DBContext)
            {
                return DBContext.AdsList.OrderByDescending(m => m.CreateTime).ToList();
            }
        }
        public AdsList GetDetail(Guid ID)
        {
            using (DBContext)
            {
                return DBContext.AdsList.FirstOrDefault(m => m.ID == ID);
            }
        }
        public int UpdateStatus(Guid ID, enStatus status)
        {
            using (DBContext)
            {
                var model = DBContext.AdsList.FirstOrDefault(m => m.ID == ID);
                if (model == null)
                {
                    return 0;
                }
                model.Status = (int)status;
                model.UpdateTime = DateTime.Now;
                return DBContext.SaveChanges();
            }
        }
        public int Delete(Guid ID)
        {
            using (DBContext)
            {
                var model = DBContext.AdsList.FirstOrDefault(m => m.ID == ID);
                if (model == null)
                {
                    return 0;
                }
                DBContext.AdsList.Remove(model);
                return DBContext.SaveChanges();
            }
        }
        public int Insert(string name, string title, string linkto, string img,string Descrition, DateTime? BeginTime, DateTime? EndTime)
        {
            using (DBContext)
            {
                AdsList model = new AdsList();
                model.ID = Guid.NewGuid();
                model.Name = name;
                model.Title = title;
                model.LinkTo = linkto;
                model.Img = img;
                model.Descrition = Descrition;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                model.BeginTime = BeginTime;
                model.EndTime = EndTime;

                DBContext.AdsList.Add(model);
                return DBContext.SaveChanges();
            }
        }
        public int Update(Guid id, string name, string title, string linkto, string img)
        {
            using (DBContext)
            {
                AdsList model = DBContext.AdsList.FirstOrDefault(m => m.ID == id);
                if (model == null)
                    return 0;

                model.Name = name;
                model.Title = title;
                model.LinkTo = linkto;
                if (!string.IsNullOrEmpty(img))
                    model.Img = img;
                model.UpdateTime = DateTime.Now;

                return DBContext.SaveChanges();
            }
        }

        public AdsList GetAdsById(Guid id)
        {
            using (DBContext)
            {
                return DBContext.AdsList.FirstOrDefault(m => m.ID == id);
            }
        }

        /// <summary>
        /// 查询广告
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public List<AdsList> GetAdsSearch(string key, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.AdsList.Where(m => m.ID != Guid.Empty);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name.Contains(key)||m.Title.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }


        public int UpdateAds(AdsList model)
        {
            using (DBContext)
            {
                DBContext.AdsList.Attach(model);
                DBContext.Entry<AdsList>(model).State = System.Data.Entity.EntityState.Modified;
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 更改广告状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int ChangeAdsStatus(Guid uid, enStatus status)
        {
            using (DBContext)
            {
                AdsList Ads = DBContext.AdsList.Where(m => m.ID == uid).FirstOrDefault();
                if (Ads != null)
                {
                    Ads.Status = (int)status;
                    Ads.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }

    }
}
