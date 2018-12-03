using RelexBarBLL;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelaxBarWeb_MVC.Utils;
using RelexBarBLL.Models;
using static RelexBarBLL.GCZoneBLL;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class GCZoneController : BaseController
    {
        // GET: GCZone
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Add(string content,string location, enGCZoneType type) {
            GCZoneBLL bll = new GCZoneBLL();

            Guid id = Guid.NewGuid();
            int i=bll.Add(UserInfo.ID,content,location,type,id);
            if (i>0) {
                return RJson("1",id.ToString());
            }
            return RJson("-1","Error");
        }
        public JsonResult AddComment(Guid id,Guid? cid,string content) {
            GCZoneBLL bll = new GCZoneBLL();

            Guid primaryID = Guid.NewGuid();
            int i = bll.AddComment(UserInfo.ID, id, cid, content,primaryID);
            if (i>0) {
                return RJson("1",primaryID.ToString());
            }
            return RJson("-1","Error");
        }

        public JsonResult GetList(DateTime? date,Guid? uid) {
            GCZoneBLL bll = new GCZoneBLL();
            List<GCZoneModel> list;
            if (uid == null)
            {
                 list= bll.GetList(UserInfo.ID,false, date, 10);
            }
            else {
                list = bll.GetList(uid.Value,true, date, 10);
            }
            
            return JsonPro(new { code=1,msg=list}, "yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 朋友圈是否有新消息
        /// </summary>
        /// <returns></returns>
        public JsonResult HasNewMessage()
        {
            GCZoneBLL bll = new GCZoneBLL();
            if (bll.HasNewMessage(UserInfo.ID)) {
                return RJson("1","");
            };
            return RJson("-1","");
        }
        public JsonResult IsLike(Guid id) {
            GCZoneBLL bll = new GCZoneBLL();
            Guid nid = Guid.NewGuid();
            int i = bll.IsLike(id, UserInfo.ID, nid);
            if (i > 0)
            {
                return RJson("1", nid.ToString());
            }
            else {
                return RJson("-1","Error");
            }

        }
        public JsonResult DeleteIsLike(Guid id) {
            GCZoneBLL bll = new GCZoneBLL();
            int i = bll.DeleteIsLike(id, UserInfo.ID);
            if (i > 0)
            {
                return RJson("1","Success");
            }
            else
            {
                return RJson("-1", "Error");
            }
        }

        /// <summary>
        /// 修改相册封面
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public JsonResult EditAlbumCover(string path) {
            GCZoneBLL bll = new GCZoneBLL();
            int i= bll.SetPhotoCover(UserInfo.ID,path);
            if (i > 0)
            {
                return RJson("1", "Success");
            }
            else {
                return RJson("-1","Error");
            }
        }
        public JsonResult GetAlbumCover(Guid uid) {
            GCZoneBLL bll = new GCZoneBLL();
            return RJson("1",bll.GetPhotoCover(uid));
        }
    }
}