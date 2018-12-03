using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    public partial class AdminUserBLL : BaseBll
    {
        public AdminUser GetAdmin(string name, string psw)
        {
            using (DBContext)
            {
                var q = DBContext.AdminUser.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    q = q.Where(m => m.Name == name);
                }
                if (!string.IsNullOrWhiteSpace(psw))
                {
                    psw = MD5(psw);
                    q = q.Where(m => m.Psw == psw);
                }

                return q.FirstOrDefault();
            }
        }
        public AdminUser GetAdmin(string name)
        {
            return GetAdmin(name, string.Empty);
        }
        /// <summary>
        /// 通过管理员id获得用户
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public AdminUser GetAdminByID(Guid UID) {
            using (DBContext) {
                return DBContext.AdminUser.Where(m=>m.ID==UID).FirstOrDefault();
            }
        }
        public AdminUser Login(string name, string psw)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(psw))
            {
                return null;
            }
            return GetAdmin(name, psw);
        }

        public int ChangePsw(string name, string oldpsw, string newpsw)
        {
            using (DBContext)
            {
                oldpsw = MD5(oldpsw);
                var q = DBContext.AdminUser.FirstOrDefault(m => m.Name == name && m.Psw == oldpsw);
                if (q != null)
                {
                    q.Psw = MD5(newpsw);
                    q.UpdateTime = DateTime.Now;
                    return DBContext.SaveChanges();
                }
                return 0;
            }
        }

        public int InsertAdminUser(string name, string psw, int powertype)
        {
            using (DBContext)
            {
                DBContext.AdminUser.Add(new AdminUser()
                {
                    CreateTime = DateTime.Now,
                    ID = Guid.NewGuid(),
                    Name = name,
                    PowerType = powertype,
                    Psw = MD5(psw),
                    Status = 1,
                    UpdateTime = DateTime.Now
                });
               // return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增管理员成功,新增管理员名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, name), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作新增管理员失败,新增管理员名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int UpdateAdminUser(AdminUser model)
        {
            using (DBContext)
            {
                DBContext.AdminUser.Attach(model);
                DBContext.Entry<AdminUser>(model).State = System.Data.Entity.EntityState.Modified;
              //  return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改管理员成功,被修改管理员名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Name), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改管理员失败,被修改管理员名称:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int UpdateAdminUser(string name, string psw, int powertype)
        {
            using (DBContext)
            {
                var q = DBContext.AdminUser.FirstOrDefault(m => m.Name == name);
                if (q != null)
                {
                    q.Name = name;
                    q.Psw = MD5(psw);
                    q.PowerType = powertype;
                    q.UpdateTime = DateTime.Now;
                    return DBContext.SaveChanges();
                }
                return 0;
            }
        }

        public int DeleteAdminUser(string name)
        {
            using (DBContext)
            {
                var q = DBContext.AdminUser.FirstOrDefault(m => m.Name == name);
                if (q != null)
                {
                    DBContext.AdminUser.Remove(q);
                    return DBContext.SaveChanges();
                }
                return 0;
            }
        }

        public bool CheckUserReal(Guid uid)
        {
            using (DBContext)
            {
                var u = DBContext.Users.FirstOrDefault(m => m.ID == uid);
                if (u == null)
                    return false;
                u.RealCheck = 1;
                return DBContext.SaveChanges() > 1;
            }
        }
        /// <summary>
        /// 获得管理员列表
        /// </summary>
        /// <returns></returns>
        public List<AdminUser> GetAdminList(int pageIndex,int pageSize,string name,out int sum) {
            using (DBContext) {
                var q = DBContext.AdminUser.Where(m=>1==1);
                if (!string.IsNullOrEmpty(name)) {
                    q = q.Where(m=>m.Name.Contains(name));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
                
        }
        /// <summary>
        /// 检查管理员是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist(string name) {
            using (DBContext) {
                return DBContext.AdminUser.Where(m => m.Name == name).Count()>0;
            }
        }

        public List<AdminUser> GetAllList()
        {
            using (DBContext)
            {
                return DBContext.AdminUser.Where(m => m.Status == 1).ToList();
            }
        }
    }
}
