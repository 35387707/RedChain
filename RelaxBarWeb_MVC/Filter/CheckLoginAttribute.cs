using RelexBarBLL;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RelexBarBLL.Common;

namespace RelaxBarWeb_MVC.Filter
{
    public class CheckLoginAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            /*
             * 这里为了方便演示，直接在请求参数中获取了userName
             * 假设某一个Action只有Lucy可以访问。
             */
            object u = filterContext.HttpContext.Session["user"];
            object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoFilter), true);
            if (attrs.Length != 1)
            {
                if (u == null)//如果sessoin过期
                {
                    HttpCookie cookie = filterContext.HttpContext.Request.Cookies["token"];
                    string[] str = null;
                    if (cookie != null)
                    {
                        str = cookie.Value.Split('|');//如果cookie中有
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["token"]))
                            str = filterContext.HttpContext.Request.QueryString["token"].Split('|');//
                        else if (!string.IsNullOrEmpty(filterContext.HttpContext.Request["token"]))
                            str = filterContext.HttpContext.Request["token"].Split('|');//
                    }

                    if (str != null && str.Length == 2)
                    {
                        Guid uid = Guid.Empty;
                        if (Guid.TryParse(str[0], out uid))
                        {
                            UsersBLL bll = new UsersBLL();
                            Users user = bll.GetUserById(uid);
                            if (user != null && user.LastLoginTime.AddDays(360) > DateTime.Now)//cookie 360天内有效
                            {
                                if (user != null && CommonClass.EncryptDecrypt.GetMd5Hash(user.ID + user.Psw + SysConfigBLL.MD5Key + user.LastLoginTime.ToString("yyyyMMddHHmmss")) == str[1])
                                {
                                    filterContext.HttpContext.Session["user"] = user;
                                    base.OnActionExecuting(filterContext);
                                    return;
                                }
                            }
                        }
                    }
                    filterContext.HttpContext.Response.Write("{\"code\":\"" + (int)ErrorCode.重新登陆 + "\",\"msg\":\"请重新登陆\"}");
                    filterContext.HttpContext.Response.End();
                    filterContext.Result = new EmptyResult();//清空当前Action,不执行当前Action代码
                }
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
            // 
        }
    }
}