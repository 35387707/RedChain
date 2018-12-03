
using RelexBarBLL;
using RelexBarDLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace RelaxBarWeb_MVC.Controllers
{
    public class QRCodeController : BaseController
    {
        [Filter.CheckLogin]
        public void GetQRCode(Guid? UID)
        {
            try
            {
                UID = !UID.HasValue ? UserInfo.ID : UID.Value;
                string headimg = string.Empty;
                Users u = new UsersBLL().GetUserById(UID.Value);
                if (u == null)
                {
                    return;
                }
                //data = data.Replace("^*", "&");
                Response.ClearContent();
                string result = HttpUtility.UrlDecode(string.Format("{0}/Account/Regist?cn={1}", RelexBarBLL.SysConfigBLL.DOMAIN, u.CardNumber));
                Response.ContentType = "image/jpeg";
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    //var q = Common.GetQrCodeImgAndLogo(id, HttpContext.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["qrcode_logo"]));

                    //var q = createQR(HttpContext.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["qrcode_logo"]), id, 250);

                    //if (string.IsNullOrEmpty(u.HeadImg1))
                    //{
                    //    headimg = "/img/chat/head.jpg";
                    //}
                    //else
                    //{
                    //    headimg = u.HeadImg1;
                    //}
                    //string path = HttpContext.Server.MapPath(headimg);
                    var q = RelexBarBLL.Common.GetQrCodeImg(result);
                    q.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    Response.BinaryWrite(ms.ToArray());
                    q.Dispose();
                }
                Response.End();
            }
            catch (Exception ex)
            {
                RelexBarBLL.LogsBLL.InsertAPILog("/QRCode/GetQRCode", UserInfo.ID,ex.ToString());
            }
            
        }

    }
}