using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Web;
using System.Linq.Expressions;

namespace RelexBarBLL
{
    public partial class PermissionBLL : BaseBll
    {
        public Permission GetPermissionByID(int ID)
        {
            using (DBContext)
            {
                return DBContext.Permission.Where(m => m.ID == ID).FirstOrDefault();
            }
        }


        public string AdminPermissionTeam(string permission)
        {
            string permissionName = "";

            //int[] ps = Array.ConvertAll(permission.Split(','), m => Convert.ToInt32(m));
            //int[]
            string[] ps = permission.Split(',');
            foreach (string e in ps)
            {
                if (e != null&&e!="")
                {
                    var permisson = GetPermissionByID(int.Parse(e));
                    if (permisson != null)
                    {
                        var s = permisson.Name.ToString();
                        permissionName = permissionName + s + ",";
                    }
                }
            }
            if (!string.IsNullOrEmpty(permissionName))
            {
                permissionName = permissionName.Substring(0, permissionName.Length - 1);
            }
            return permissionName;
        }


        public List<Permission> GetParment()
        {
            using (DBContext)
            {
                return  DBContext.Permission.Where(m => m.FID == null && m.Status == (int)Common.enStatus.Enabled).OrderBy(m => m.Order).ToList();
               
            }
        }

        public List<Permission> GetChildPermisson(int? Fid)
        {
            return DBContext.Permission.Where(m => m.FID == Fid && m.Status == (int)Common.enStatus.Enabled).OrderBy(m => m.Order).ToList();
        }
        

        //public List<Permission> GetParment(Expression<Func<Permission, bool>> predicate)
        //{
        //    using (DBContext)
        //    {
        //        return DBContext.Permission.Where(predicate).OrderBy(m => m.Order).ToList();
        //    }
        //}
    }
}
