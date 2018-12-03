using RelexBarBLL;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RelaxBarWeb_MVC.Controllers
{
#if !DEBUG
    [Filter.CheckLogin]
#endif
    public class AlbumController : BaseController
    {
        public AlbumController() {

        }
        /// <summary>
        /// 创建相册
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult CreateAlbum(string name,string desc) {
#if DEBUG
            Users u = new Users();
            u.ID = Guid.Empty;
            Session["user"] = u;
#endif
            AlbumListBLL bll = new AlbumListBLL();
            Guid id = Guid.NewGuid();
            int i= bll.Add(UserInfo.ID,name,desc,id);
            if (i > 0)
            {
                return RJson("1", id.ToString());
            }
            else {
                return RJson("-1","Error");
            }
        }
        /// <summary>
        /// 上传照片 没有建立默认相册
        /// </summary>
        /// <param name="file"></param>
        /// <param name="albumListID"></param>
        /// <returns></returns>
        public JsonResult UploadPhoto(HttpPostedFileBase file,Guid albumListID) {
#if DEBUG
            Users u = new Users();
            u.ID = Guid.Empty;
            Session["user"] = u;
#endif
            try
            {
                if (file == null)
                {
                    return RJson(((int)Common.ErrorCode.文件为空).ToString(), Common.ErrorCode.文件为空.ToString());
                }
                string[] tname = file.FileName.Split('.');
                if (!FileController.last.Contains(tname[1].ToLower()))
                {
                    return RJson(((int)Common.ErrorCode.文件类型不正确).ToString(),Common.ErrorCode.文件类型不正确.ToString());
                }
                string newfilename = string.Format("{0}.{1}", Guid.NewGuid(), tname[1]); 
                string serverPath = Server.MapPath("/");
                string dirPath =string.Format("/Album/{0}/{1}", UserInfo.ID,DateTime.Now.ToString("yyyyMMdd"));
                string uploadPath = string.Format("{0}/{1}", dirPath, newfilename);
                string fullPath = serverPath + uploadPath;
                if (!Directory.Exists(serverPath + dirPath))
                {
                    Directory.CreateDirectory(serverPath + dirPath);
                }
                file.SaveAs(fullPath);
                AlbumListBLL bll = new AlbumListBLL();
                int i= bll.AddPhoto(UserInfo.ID,albumListID, dirPath);
                if (i > 0)
                {
                    return RJson("1", uploadPath);
                }
                else {
                    return RJson(i.ToString(),((Common.ErrorCode)i).ToString());
                }
            }
            catch (Exception ex)
            {
                LogsBLL.InsertAPILog("/Album/UploadPhoto", Guid.Empty, "图片上传异常：" + ex.Message);
                return RJson("-1", ex.Message);
            }
        }

    }
}