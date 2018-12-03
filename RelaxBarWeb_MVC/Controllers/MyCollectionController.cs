using RelexBarBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RelaxBarWeb_MVC.Controllers
{
    /// <summary>
    /// 收藏Controller
    /// </summary>
    public class MyCollectionController : BaseController
    {

        MyCollectionBLL bll = new MyCollectionBLL();

        [Filter.CheckLogin]
        public JsonResult Add(Guid mid, int? mtype)
        {
            var mtp = Common.enMycollectionType.RedPacket;
            if (mtype.HasValue)
            {
                mtp = (Common.enMycollectionType)mtype;
            }
            var i = bll.Add(UserInfo.ID, mid, mtp);
            if (i != Guid.Empty)
            {
                return RJson("1", i.ToString());
            }
            else
            {
                return RJson("-1", "Error");
            }
        }

        /// <summary>
        /// 关注某人
        /// </summary>
        /// <param name="mid"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult FollowUser(Guid uid)
        {
            var i = bll.Add(UserInfo.ID, uid, Common.enMycollectionType.User);
            if (UserInfo.ID == uid)
            {
                return RJson("-1", "不能关注自己哦");
            }

            if (i != Guid.Empty)
            {
                return RJson("1", i.ToString());
            }
            else
            {
                return RJson("-1", "Error");
            }
        }

        public JsonResult Delete(Guid id)
        {
            if (bll.Delete(id) > 0)
            {
                return RJson("1", "Success");
            }
            else
            {
                return RJson("-1", "Error");
            }
        }

        /// <summary>
        /// 获取收藏列表
        /// </summary>
        /// <param name="mtype"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetList(int mtype, int index)
        {
            var list = bll.GetList(UserInfo.ID, (Common.enMycollectionType)mtype, index, PageSize, out DataCount);
            return Json2(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        /// <summary>
        /// 获取粉丝（被关注）列表
        /// </summary>
        /// <param name="mtype"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult GetFansList()
        {
            var list = bll.GetFansList(UserInfo.ID, PageIndex, PageSize, out DataCount);
            return Json2(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        public JsonResult AddLabel(Guid id, string label)
        {
            if (bll.AddLabel(id, label) > 0)
            {
                return RJson("1", "Success");
            }
            else
            {
                return RJson("-1", "Error");
            }
        }
        public JsonResult DeleteLabel(Guid id, string label)
        {
            if (bll.DeleteLabel(id, label) > 0)
            {
                return RJson("1", "Success");
            }
            else
            {
                return RJson("-1", "Error");
            }
        }
        public JsonResult EditLabel(Guid id, string oldLabel, string newLabel)
        {
            if (bll.EditLabel(id, oldLabel, newLabel) > 0)
            {
                return RJson("1", "Success");
            }
            else
            {
                return RJson("-1", "Error");
            }
        }
    }
}