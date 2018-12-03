using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    public partial class CategoryBLL : BaseBll
    {
        public List<Category> GetAllList()
        {
            using (DBContext)
            {
                return DBContext.Category.Where(m => m.IsShow == 1).ToList();
            }
        }

        public List<Category> GetAllList(bool? isShow, string name, int? headid)
        {
            using (DBContext)
            {
                var q = DBContext.Category.AsEnumerable();
                if (string.IsNullOrEmpty(name))
                {
                    q = q.Where(m => m.Name.Contains(name));
                }
                if (headid.HasValue)
                {
                    q = q.Where(m => m.HeadID == headid.Value);
                }
                if (isShow.HasValue)
                {
                    q = q.Where(m => m.IsShow == (isShow.Value ? 1 : 0));
                }
                return q.ToList();
            }
        }

        public int Insert(int HeadID, string name, string Keywords, string title, int? order)
        {
            using (DBContext)
            {
                string Family = string.Empty;
                int lv = 0;
                if (HeadID != 0)
                {
                    var fcategory = DBContext.Category.FirstOrDefault(m => m.ID == HeadID);
                    if (fcategory != null)
                    {
                        Family = fcategory.Family;
                        lv = fcategory.LV.Value;
                    }
                }

                Family += HeadID + ",";
                lv++;

                Category model = new Category();
                model.IsShow = (int)enStatus.Enabled;
                model.Name = name;
                model.HeadID = HeadID;
                model.Keywords = Keywords;
                model.Title = title;
                model.Family = Family;
                model.LV = lv;
                model.OrderID = order;

                DBContext.Category.Add(model);
                //return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增商品分类成功,商品分类名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, name), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增商品分类失败,商品分类名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public Category GetDetail(int CID)
        {
            using (DBContext)
            {
                return DBContext.Category.FirstOrDefault(m => m.ID == CID);
            }
        }

        public int Update(int CID, string name, int headid, int? order, string title, string Keywords)
        {
            using (DBContext)
            {
                var model = DBContext.Category.FirstOrDefault(m => m.ID == CID);
                if (model == null)
                {
                    return 0;
                }
                if (headid == CID)
                {
                    return 0;
                }
                if (model.HeadID != headid)//父类发生更改
                {
                    var headc = DBContext.Category.FirstOrDefault(m => m.ID == headid);
                    string family = "0,";
                    int lv = 1;

                    if (headc != null)//付类别不存在
                    {
                        // family = headc.Family + CID + ",";
                        family = headc.Family + headid + ",";
                        lv = headc.LV.Value + 1;
                    }

                    model.HeadID = headid;
                    model.LV = lv;
                    model.Family = family;

                    var childC = DBContext.Category.Where(m => m.HeadID == CID).ToList();
                    if (childC != null)
                    {
                        childC.ForEach(m => { m.LV = lv + 1; m.Family = family + m.ID + ","; });
                    }
                }

                model.Name = name;
             //   model.IsShow = isshow;
                model.OrderID = order;
                model.Title = title;
                model.Keywords = Keywords;

                //  return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改商品分类成功,商品分类名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, name), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改商品分类失败,商品分类名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public List<Category> GetCategoryList(string key, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.Category.AsQueryable();
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.Name), pagesize, pageinex, out count);
            }
        }


        public int UpdateStatus(int ID, int status)
        {
            using (DBContext)
            {
                var pro = DBContext.Category.FirstOrDefault(m => m.ID == ID);
                if (pro == null)
                {
                    return 0;
                }
                pro.IsShow = status;
             //   return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作商品分类状态成功,分类名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, pro.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作商品分类状态失败,分类名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, pro.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    return 0;
                }
            }
        }


        public string CategoryTeam(string family)
        {
            string CateName = "";
            string familys = family.Substring(0, family.Length - 1); 
            string[] str = familys.Split(',');
            if (str.Length <= 1)
            {
                return "无";
            }
            foreach (string e in str)
            {
                if (e != "0")
                {
                    var Category = GetDetail(int.Parse(e));
                    if(Category!=null)
                    {
                        var s = Category.Name.ToString();
                        CateName = CateName + s + ",";
                    }
                }
            }
            if(!string.IsNullOrEmpty(CateName))
            {
                CateName = CateName.Substring(0, CateName.Length - 1);
            }
            return CateName;
        }
    }
}
