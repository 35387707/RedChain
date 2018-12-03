using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    public partial class RoleBLL : BaseBll
    {
        public Role GetRoleByID(int ID)
        {
            using (DBContext)
            {
                return DBContext.Role.Where(m => m.ID == ID).FirstOrDefault();
            }
        }

        public List<Role> GetAllList()
        {
            using (DBContext)
            {
                return DBContext.Role.Where(m => m.Status == 1).ToList();
            }
        }
        
        public List<Role> GetRoleSearch(int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
               
                return GetPagedList(DBContext.Role.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }

        public int Insert(string Name)
        {
            using (DBContext)
            {
                Role model = new Role();
                model.Name = Name;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                DBContext.Role.Add(model);
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增角色名称成功,角色名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, Name), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增角色名称失败,角色名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, Name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int UpdateRoleById(Role model)
        {
            using (DBContext)
            {
                DBContext.Role.Attach(model);
                DBContext.Entry<Role>(model).State = System.Data.Entity.EntityState.Modified;
                // return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改角色名称成功,角色名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Name), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改角色名称成功,角色名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int ChangeRoleStatus(int Id, enStatus status)
        {
            using (DBContext)
            {

                Role r = DBContext.Role.Where(m => m.ID == Id).FirstOrDefault();
                if (r != null)
                {
                    r.Status = (int)status;
                    r.UpdateTime = DateTime.Now;
                }

                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改角色{1}状态成功,状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, r.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改角色{1}状态失败,状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, r.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    return 0;
                }
            }
        }

    }
}
