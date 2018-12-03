using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;

namespace RelexBarBLL
{
    public partial class AdminPermissionBLL: BaseBll
    {
        public string GetAdminPermission(Guid AdminId)
        {
            using (DBContext)
            {
                return DBContext.AdminPermission.Where(m => m.AID == AdminId).Select(m => m.PermissionID).FirstOrDefault();
            }
        }

        public AdminPermission GetAdminPermissionByID(Guid ID)
        {
            using (DBContext)
            {
                return DBContext.AdminPermission.FirstOrDefault(m => m.ID == ID);
            }
        }
        public List<AdminPermission> GetAdminPermissionSearch(int pageSize, int pageIndex, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.AdminPermission.Where(m => m.ID != Guid.Empty);
                //if (!string.IsNullOrEmpty(key))
                //{
                //    q = q.Where(m => m..Contains(key));
                //}
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }
        }

        public int Insert(Guid? AID, int RoleID, string PermissionID,out string err)
        {
            using (DBContext)
            {
                var ad = DBContext.AdminUser.FirstOrDefault(m => m.ID == AID);
                var perminssion = DBContext.AdminPermission.FirstOrDefault(m => m.AID == AID);
                if(ad == null)
                {
                    err = "授权管理员不存在";
                    return 0;
                }

                if (perminssion != null)
                {
                    err = "已存在授权管理员";
                    return 0;
                }
                    //  UserHelp model = new UserHelp();
                AdminPermission model = new AdminPermission();
                model.ID = Guid.NewGuid();
                model.AID = AID.Value;
                model.RoleID = RoleID;
                model.PermissionID = PermissionID;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;
                DBContext.AdminPermission.Add(model);
               
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        err = "授权成功";
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作授权管理员{1} 授权成功", (HttpContext.Current.Session["admin"] as AdminUser).Name, ad.Name), enLogType.Admin);
                        return i;

                    }
                    else
                    {
                        err = "授权失败";
                        return 0;
                    }
                }
                catch (Exception)
                {
                    err = "授权失败";
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作授权管理员{1} 授权失败", (HttpContext.Current.Session["admin"] as AdminUser).Name,ad.Name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int UpdateAdminPermissionById(AdminPermission model, out string err)
        {
            using (DBContext)
            {
                Guid aid = model.AID;
                var ad = DBContext.AdminUser.FirstOrDefault(m => m.ID == model.AID);
                if (ad == null)
                {
                    err = "授权管理员不存在";
                    return 0;
                }

                DBContext.AdminPermission.Attach(model);
                DBContext.Entry<AdminPermission>(model).State = System.Data.Entity.EntityState.Modified;
                // return DBContext.SaveChanges();
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        err = "修改授权成功";
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改授权管理员{1} 授权成功", (HttpContext.Current.Session["admin"] as AdminUser).Name, ad.Name), enLogType.Admin);
                        return i;

                    }
                    else
                    {
                        err = "修改授权失败";
                        return 0;

                    }
                }
                catch (Exception)
                {
                    err = "修改授权失败";
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改授权管理员{1} 授权失败", (HttpContext.Current.Session["admin"] as AdminUser).Name, ad.Name), enLogType.Admin);
                    return 0;
                }
            }
        }

        public int ChangeAdminPermissionStatus(Guid Id, enStatus status)
        {
            using (DBContext)
            {
                
                AdminPermission adPer = DBContext.AdminPermission.Where(m => m.ID == Id).FirstOrDefault();
                var ad = DBContext.AdminUser.FirstOrDefault(m => m.ID == adPer.AID);
                if (adPer != null)
                {
                    adPer.Status = (int)status;
                    adPer.UpdateTime = DateTime.Now;
                }
                
                try
                {
                    int i = DBContext.SaveChanges();
                    if (i > 0)
                    {
                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改授权管理员{1}状态成功,状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, ad.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    }
                    return i;
                }
                catch (Exception)
                {
                    logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改授权管理员{1}状态失败,状态：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, ad.Name, (int)status == 1 ? "启用" : "禁用"), enLogType.Admin);
                    return 0;
                }
            }
        }

    }
}
