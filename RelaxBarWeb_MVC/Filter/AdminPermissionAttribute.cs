using RelexBarBLL;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RelaxBarWeb_MVC.Utils;

namespace RelaxBarWeb_MVC.Filter
{
    public class AdminPermissionAttribute: ActionFilterAttribute
    {
        private Common.PermissionName permissions;
        ActionExecutingContext context;
        RelexBarBLL.AdminPermissionBLL bll = new RelexBarBLL.AdminPermissionBLL();
        public AdminPermissionAttribute(Common.PermissionName permissions)
        {
            this.permissions = permissions;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.context = filterContext;

            object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoFilter), true);
            HttpCookie cookie = filterContext.HttpContext.Request.Cookies["adminToken"];
            if (attrs.Length == 0&& cookie != null)
            {
                string[] str = cookie.Value.Split('|');
                try
                {
                    Guid AdminID = Guid.Empty;
                    if (str.Length == 2)
                    {
                        AdminID = Guid.Parse(str[0]);
                    }
                    string permission = bll.GetAdminPermission(AdminID);
                    if (permission == null)
                    {
                        WriteAndEnd(Common.ErrorCode.暂无此操作权限);
                        return;
                    }
                    int[] ps = Array.ConvertAll(permission.Split(','), m => Convert.ToInt32(m));
                    if (ps.Contains((int)permissions))
                    {
                        base.OnActionExecuting(filterContext);
                        return;
                    }
                    else
                    {
                        WriteAndEnd(Common.ErrorCode.暂无此操作权限);
                        return;
                    }
                }
                catch (Exception)
                {

                    WriteAndEnd(Common.ErrorCode.暂无此操作权限);
                    return;
                }
            }
        }

        void WriteAndEnd(Common.ErrorCode error)
        {
            context.Result = new JsonResultPro(new { code = (int)error, msg = error.ToString() }, JsonRequestBehavior.AllowGet);
        }

       
    }
}