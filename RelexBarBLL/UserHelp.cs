using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    public partial class UserHelpBLL:BaseBll
    {
        public int Insert(string title, int Type, string img, string Content)
        {
            using (DBContext)
            {
                UserHelp model = new UserHelp();
                model.ID = Guid.NewGuid();
                model.Title = title;
                model.Img = img;
                model.Content = Content;
                model.Type = Type;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                DBContext.UserHelp.Add(model);
              //  return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增帮助中心成功,标题:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name,title), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增帮助中心失败,标题:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name,title), enLogType.Admin);
                    return 0;
                }
            }
        }

        public List<UserHelp> GetUserHelpSearch(string key, int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.UserHelp.Where(m => m.ID != Guid.Empty);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Title.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }

        public UserHelp GetUserHelpById(Guid id)
        {
            using (DBContext)
            {
                return DBContext.UserHelp.FirstOrDefault(m => m.ID == id);
            }
        }

        public List<UserHelp> GetUserHelpList(Guid id)
        {
            using (DBContext)
            {
                return DBContext.UserHelp.Where(m => m.ID == id ).ToList();
            }
        }

        public int UpdateUserHelpById(UserHelp model)
        {
            using (DBContext)
            {
                DBContext.UserHelp.Attach(model);
                DBContext.Entry<UserHelp>(model).State = System.Data.Entity.EntityState.Modified;
               // return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改帮助中心成功,标题:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Title), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改帮助中心失败,标题:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name,model.Title), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int ChangeUserHelpStatus(Guid Id, enStatus status)
        {
            using (DBContext)
            {
                UserHelp help = DBContext.UserHelp.Where(m => m.ID == Id).FirstOrDefault();
                if (help != null)
                {
                    help.Status = (int)status;
                    help.UpdateTime = DateTime.Now;
                }
              //  return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更新帮助中心状态成功,标题:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, help.Title, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作更新帮助中心状态失败,标题:{1},状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, help.Title, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    return 0;
                }
            }
        }
    }
}
