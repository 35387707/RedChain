using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;
using System.Web.Script.Serialization;

namespace RelexBarBLL
{
    public partial class ProductsBLL : BaseBll
    {
        public List<ProductList> GetProductList(enProductType ptpye)
        {
            using (DBContext)
            {
                return DBContext.ProductList.Where(m => m.Status == (int)enStatus.Enabled && m.Type == (int)ptpye).OrderByDescending(m => m.OrderID).ToList();
            }
        }
        public List<ProductList> GetAllProductList(int categoryid)
        {
            using (DBContext)
            {
                return DBContext.ProductList.Where(m => m.CategoryID == categoryid && m.Status == (int)enStatus.Enabled).OrderByDescending(m => m.OrderID).ToList();
            }
        }
        public List<ProductList> GetAllProductList()
        {
            using (DBContext)
            {
                return DBContext.ProductList.OrderByDescending(m => m.OrderID).ToList();
            }
        }

        public List<ProductList> GetProductList(enProductType ptpye, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.ProductList.Where(m => m.Status == (int)enStatus.Enabled && m.Type == (int)ptpye).OrderByDescending(m => m.OrderID);
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }
        public dynamic GetAllProductList(int? categoryid, string key, int? status, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.ProductList
                        join b in DBContext.Category on a.CategoryID equals b.ID into t
                        from c in t.DefaultIfEmpty()
                        join d in DBContext.Users on a.ShopID equals d.ID into t2
                        from e in t2.DefaultIfEmpty()
                        select new
                        {
                            ID = a.ID,
                            Name = a.Name,
                            Title = a.Title,
                            Number = a.Number,
                            CategoryID = a.CategoryID,
                            Img = a.Img,
                            ImgList = a.ImgList,
                            Descrition = a.Descrition,
                            RealPrice = a.RealPrice,
                            PriceType = a.PriceType,
                            Stock = a.Stock,
                            Price = a.Price,
                            OrderID = a.OrderID,
                            Type = a.Type,
                            BeginTime = a.BeginTime,
                            EndTime = a.EndTime,
                            Status = a.Status,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime,
                            ShopID = a.ShopID,
                            ShopName = e == null ? "" : e.Name,
                            ShopTrueName = e == null ? "" : e.TrueName,
                            ShopStatus = e == null ? -1 : e.Status,
                            ShopAddress = e == null ? "" : e.Address,
                            ShopPhone = e == null ? "" : e.Phone,
                            CategoryName = c == null ? "" : c.Name,
                            CategoryShow = c == null ? null : c.IsShow,
                        };

                if (categoryid.HasValue)
                {
                    q = q.Where(m => m.CategoryID.Value == categoryid.Value);
                }
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name.Contains(key) || m.Number.Contains(key) || m.Title.Contains(key) || m.CategoryName.Contains(key) ||
                    m.ShopName.Contains(key) || m.Descrition.Contains(key));
                }
                if (status.HasValue)
                {
                    q = q.Where(m => m.Status == status.Value);
                }

                return GetPagedList(q.OrderByDescending(m => m.OrderID), pagesize, pageinex, out count);
            }
        }


        public List<Models.ProductListModel> GetAllProductLists(int? categoryid, string key, int? status, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.ProductList
                        join b in DBContext.Category on a.CategoryID equals b.ID into t
                        from c in t.DefaultIfEmpty()
                        join d in DBContext.Users on a.ShopID equals d.ID into t2
                        from e in t2.DefaultIfEmpty()
                        select new Models.ProductListModel
                        {
                            ID = a.ID,
                            Name = a.Name,
                            Title = a.Title,
                            Number = a.Number,
                            CategoryID = a.CategoryID,
                            Img = a.Img,
                            ImgList = a.ImgList,
                            Descrition = a.Descrition,
                            RealPrice = a.RealPrice,
                            PriceType = a.PriceType,
                            Stock = a.Stock,
                            FootQuanPrice =a.FootQuanPrice,
                            Price = a.Price,
                            OrderID = a.OrderID,
                            Type = a.Type,
                            BeginTime = a.BeginTime,
                            EndTime = a.EndTime,
                            Status = a.Status,
                            CreateTime = a.CreateTime,
                            UpdateTime = a.UpdateTime,
                            CompleteCount=a.CompleteCount,
                            ViewCount=a.ViewCount,
                            GoodCount=a.GoodCount,
                            CPoints=a.CPoints,
                            ShopID = a.ShopID,
                            ShopName = e == null ? "" : e.Name,
                            ShopTrueName = e == null ? "" : e.TrueName,
                            ShopStatus = e == null ? -1 : e.Status,
                            ShopAddress = e == null ? "" : e.Address,
                            ShopPhone = e == null ? "" : e.Phone,
                            CategoryName = c == null ? "" : c.Name,
                            CategoryShow = c == null ? null : c.IsShow,
                        };

                if (categoryid.HasValue)
                {
                    q = q.Where(m => m.CategoryID.Value == categoryid.Value);
                }
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name.Contains(key) || m.Number.Contains(key) || m.Title.Contains(key) || m.CategoryName.Contains(key) ||
                    m.ShopName.Contains(key) || m.Descrition.Contains(key));
                }
                if (status.HasValue)
                {
                    q = q.Where(m => m.Status == status.Value);
                }

                return GetPagedList(q.OrderByDescending(m => m.OrderID), pagesize, pageinex, out count);
            }
        }
    
        public List<ProductList> GetAllProductList(int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.ProductList.OrderByDescending(m => m.OrderID);

                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        public List<ProductList> GetProductExcel(int?categoryid,string key,int? status)
        {
            var q = DBContext.ProductList.Where(m => m.ID != Guid.Empty);

            if (categoryid.HasValue)
            {
                q = q.Where(m => m.CategoryID.Value == categoryid.Value);
            }
            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(m => m.Name.Contains(key) || m.Number.Contains(key));
            }
            if (status.HasValue)
            {
                q = q.Where(m => m.Status == status.Value);
            }
            return q.OrderByDescending(m => m.OrderID).ToList();
        }

        /// <summary>
        /// 显示到客户端APP
        /// </summary>
        /// <param name="categoryid"></param>
        /// <param name="key"></param>
        /// <param name="status"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageinex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public dynamic GetAppProductList(int? categoryid, string key, int? status, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.ProductList
                        join b in DBContext.Category on a.CategoryID equals b.ID into t
                        from c in t.DefaultIfEmpty()
                        join d in DBContext.Users on a.ShopID equals d.ID into t2
                        from e in t2.DefaultIfEmpty()
                        where a.Status == (int)enStatus.Enabled
                        select new
                        {
                            ID = a.ID,
                            Name = a.Name,
                            Title = a.Title,
                            Number = a.Number,
                            CategoryID = a.CategoryID,
                            Img = a.Img,
                            PriceType = a.PriceType,
                            Stock = a.Stock,
                            Price = a.Price,
                            FootQuanPrice = a.FootQuanPrice,
                            RealPrice = a.RealPrice,
                            OrderID = a.OrderID,
                            Type = a.Type,
                            ShopID = a.ShopID,
                            CompleteCount = a.CompleteCount,
                            GoodCount = a.GoodCount,
                            ViewCount = a.ViewCount,
                            CPoints = a.CPoints,
                            ShopName = e == null ? "" : e.Name,
                            ShopTrueName = e == null ? "" : e.TrueName,
                            ShopStatus = e == null ? -1 : e.Status,
                            ShopAddress = e == null ? "" : e.Address,
                            ShopPhone = e == null ? "" : e.Phone,
                            CategoryName = c == null ? "" : c.Name,
                            CategoryShow = c == null ? null : c.IsShow,
                        };

                if (categoryid.HasValue)
                {
                    q = q.Where(m => m.CategoryID.Value == categoryid.Value);
                }
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name.Contains(key) || m.Number.Contains(key) || m.Title.Contains(key) || m.CategoryName.Contains(key) ||
                    m.ShopName.Contains(key));
                }

                return GetPagedList(q.OrderByDescending(m => m.OrderID), pagesize, pageinex, out count);
            }
        }

        public ProductList GetAppProduct(Guid id)
        {
            using (DBContext)
            {
                var q = DBContext.ProductList.FirstOrDefault(m => m.ID == id && m.Status == (int)enStatus.Enabled);
                if (q != null)
                {
                    q.ViewCount++;

                    DBContext.SaveChanges();
                }
                return q;
            }
        }

        public ProductList GetProduct(string id)
        {
            return GetProduct(Guid.Parse(id));
        }
        public ProductList GetProduct(Guid id)
        {
            using (DBContext)
            {
                return DBContext.ProductList.FirstOrDefault(m => m.ID == id);
            }
        }

        public int Insert(Guid ShopID, string Name, string Title, int CategoryID, string Img, string ImgList, string Descrition,
            decimal RealPrice, int paytpye, decimal Price,decimal FootQuanPrice, decimal Stock, int OrderID, int ptype, DateTime? BeginTime, DateTime? EndTime,int CompleteCount,int GoodCount,int ViewCount,int CPoints, string Specification, string specList)
        {
            using (DBContext)
            {
                ProductList model = new ProductList();
                model.ID = Guid.NewGuid();
                model.ShopID = ShopID;
                model.Name = Name;
                model.Title = Title;
                model.CategoryID = CategoryID;
                model.Img = Img;
                model.ImgList = ImgList;
                model.Descrition = Descrition;
                model.RealPrice = RealPrice;
                model.PriceType = (int)paytpye;
                model.Price = Price;
                model.FootQuanPrice = FootQuanPrice;
                model.Stock = Stock;
                model.OrderID = OrderID;
                model.Type = (int)ptype;
                model.BeginTime = BeginTime;
                model.EndTime = EndTime;
                //model.Number = Number;
                model.Number = Common.GetNumer();//获取商品号
                model.Status = 1;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                model.CompleteCount = CompleteCount;
                model.GoodCount = GoodCount;
                model.ViewCount = ViewCount;
                model.CPoints = CPoints;

                if (!string.IsNullOrEmpty(Specification))
                {
                    JavaScriptSerializer Serializers = new JavaScriptSerializer();
                    List<ProductSpecification> pmodel = Serializers.Deserialize<List<ProductSpecification>>(specList);
                    for (int j = 0; j < pmodel.Count; j++)
                    {
                        pmodel[j].ID = Guid.NewGuid();
                        pmodel[j].ProductID = model.ID;
                        pmodel[j].Number = string.Empty;

                    }
                    model.Specification = Specification;
                    DBContext.ProductSpecification.AddRange(pmodel);
                }

                DBContext.ProductList.Add(model);
                try
                {
                    int result = DBContext.SaveChanges();
                    if (result > 0)
                    {
                        
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},新增商品成功,商品号:{1},商品名称:{2},", (HttpContext.Current.Session["admin"] as AdminUser).Name,model.Number,Name), enLogType.Admin);
                    }
                    return result;
                }
                catch (Exception)
                {
                    logBll.InsertLog(model.ID, string.Format("管理员：{0},新增商品失败,商品号:{1},商品名称:{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name,model.Number,Name), enLogType.Admin);
                    return 0;

                }
                
            }
        }

        public int Update(ProductList model, string specList)
        {
            using (DBContext)
            {
                DBContext.ProductList.Attach(model);
                DBContext.Entry<ProductList>(model).State = System.Data.Entity.EntityState.Modified;
                List<ProductSpecification> oldSpecList = DBContext.ProductSpecification.Where(m => m.ProductID == model.ID).ToList();
                if (!string.IsNullOrEmpty(model.Specification))
                {
                    JavaScriptSerializer Serializers = new JavaScriptSerializer();
                    List<ProductSpecification> pmodel = Serializers.Deserialize<List<ProductSpecification>>(specList);
                    for (int j = 0; j < pmodel.Count; j++)
                    {
                        var oldSpec = oldSpecList.FirstOrDefault(m => m.SpecificationAttr == pmodel[j].SpecificationAttr);
                        if (oldSpec != null)
                        {//如果 新的规格和老的规格一样
                            pmodel[j].ID = oldSpec.ID;
                        }
                        else
                        {
                            pmodel[j].ID = Guid.NewGuid();
                        }

                        pmodel[j].ProductID = model.ID;
                        if (pmodel[j].Number == null)
                        {
                            pmodel[j].Number = string.Empty;
                        }
                    }
                    DBContext.ProductSpecification.AddRange(pmodel);
                }
                DBContext.ProductSpecification.RemoveRange(oldSpecList);
                try
                {
                    int result = DBContext.SaveChanges();
                    if (result > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},修改商品成功,商品号:{1},商品名称:{2},", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Number,model.Name), enLogType.Admin);

                    }
                    return result;
                }
                catch (Exception)
                {
                    logBll.InsertLog(model.ID, string.Format("管理员：{0},修改商品失败,商品号:{1},商品名称:{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Number,model.Name), enLogType.Admin);
                    return 0;
                }
              
            }
        }

        public int UpdateStatus(Guid ID, int status)
        {
            using (DBContext)
            {
                var pro = DBContext.ProductList.FirstOrDefault(m => m.ID == ID);
                if (pro == null)
                {
                    return 0;
                }
                pro.Status = status;
                pro.UpdateTime = DateTime.Now;

               // return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更改商品状态成功,商品名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, "(" + pro.Number + ")" + pro.Name, (int)status == 1 ? "上架" : "下架"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更改商品状态失败,商品名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, "(" + pro.Number + ")" + pro.Name, (int)status == 1 ? "上架" : "下架"), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int DeleteProduct(Guid ID, out string err)
        {
            using (DBContext)
            {
                
                var pro = DBContext.ProductList.FirstOrDefault(m => m.ID == ID);
                if (pro == null)
                {
                    err = "商品不存在";
                    return -1;
                }
                List<ProductSpecification> spec = DBContext.ProductSpecification.Where(m => m.ProductID == pro.ID).ToList(); //同时删除对应的规格
                if (spec != null && spec.Count != 0)
                {
                    DBContext.ProductSpecification.RemoveRange(spec);
                }
                DBContext.ProductList.Remove(pro);
             
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        err = "商品删除成功";
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作删除商品成功,商品名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, "(" + pro.Number + ")" + pro.Name, err), enLogType.Admin);
                        return i;
                    }
                    else
                    {
                        err = "商品删除失败";
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作删除商品失败,商品名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, "(" + pro.Number + ")" + pro.Name, err), enLogType.Admin);

                        return -1;
                    }
                }
                catch (Exception)
                {
                    err = "商品删除失败";
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作删除商品失败,商品名称:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, "(" + pro.Number + ")" + pro.Name, err), enLogType.Admin);
                    return -1;
                }
               // return i;
            }
        }

        public dynamic GetAllProductSpecificationByPID(Guid? PID)
        {
            using (DBContext)
            {
                var q = from p in DBContext.ProductSpecification
                        where p.ProductID == PID
                        select new
                        {
                            ID = p.ID,
                            ProductID = p.ProductID,
                            Price = p.Price,
                            SpecificationAttr = p.SpecificationAttr,
                            CostPrice = p.CostPrice,
                            Stock = p.Stock,
                            SellCount = p.SellCount,
                            Integral = p.Integral,
                            IntegralPrice = p.IntegralPrice,
                            IntegralSalesVolume = p.IntegralSalesVolume
                        };

                return q.ToList();
            }
        }

        public List<ProductSpecification> GetProductSpecification(Guid productID)
        {
            using (DBContext)
            {
                return DBContext.ProductSpecification.Where(m => m.ProductID == productID).ToList();
            }
        }

    }
}
