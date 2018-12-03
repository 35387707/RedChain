using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;

namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class InfomationController : BaseController
    {
        /// <summary>
        /// 获取广告类型
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInfomationTypes()
        {
            InfomationsBLL bll = new InfomationsBLL();
            return Json(new { code = 1, list = bll.GetInfomationTypes() });
        }

        public JsonResult GetInfomationByIID(Guid IID)
        {
            InfomationsBLL bll = new InfomationsBLL();
            return Json(new { code = 1, list = bll.GetInfomationById(IID) });
        }

        /// <summary>
        /// 发布商圈广告
        /// </summary>
        /// <returns></returns>
        public JsonResult Publish(int type, string title, string img, string linkTo)
        {
            InfomationsBLL bll = new InfomationsBLL();
            var result = bll.Publish(UserInfo.ID, type, title, img, linkTo);
            if (result > 0)
                return RJson("1", "发布成功");
            else
                return RJson(result.ToString(), "发布失败：" + ((Common.ErrorCode)result).ToString());
        }
        /// <summary>
        /// 修改发布商圈广告 
        /// </summary>
        /// <returns></returns>
        public JsonResult EditPublish(Guid IID, int type, string title, string img, string linkTo)
        {
            InfomationsBLL bll = new InfomationsBLL();
            var result = bll.EditPublish(IID, title, type, img, linkTo);
            if (result > 0)
                return RJson("1", "发布成功");
            else
                return RJson(result.ToString(), "发布失败：" + ((Common.ErrorCode)result).ToString());
        }

        /// <summary>
        /// 获取广告列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList(int type)
        {
            InfomationsBLL bll = new InfomationsBLL();
            var list = bll.GetList(type, PageSize, PageIndex, UserInfo.ID, out DataCount);
            return Json(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        public JsonResult GetListByUID()
        {
            InfomationsBLL bll = new InfomationsBLL();
            var list = bll.GetListByUID(UserInfo.ID, PageSize, PageIndex, out DataCount);
            return Json(new { code = 1, pagecount = TotalPageCount, list = list });
        }

        /// <summary>
        /// 获取商圈详情
        /// </summary>
        /// <returns></returns>
        [Filter.CheckLogin]
        public JsonResult ViewInfo(Guid IID)
        {
            InfomationsBLL bll = new InfomationsBLL();
            bool IsFirstTime = false;
            var model = bll.ViewInfo(IID, UserInfo.ID, ref IsFirstTime);

            MyCollectionBLL mbll = new MyCollectionBLL();
            var Collect = mbll.GetMyCollect(UserInfo.ID, IID, Common.enStatus.Enabled, Common.enMycollectionType.Information);
            var CollectionID = Collect == null ? "0" : Collect.ID.ToString(); //收藏ID
            var IsCollection = Collect == null ? 0 : 1; //收藏状态（0未收藏，1已收藏） 

            var Good = bll.GetGood(UserInfo.ID, IID, Common.enViewGood.Good);
            var GoodID = Good == null ? "0" : Good.IVID.ToString();  //点赞ID
            var IsGoodFor = Good == null ? 0 : 1;//是否点赞

            return Json(new { code = 1, IsFirstTime = IsFirstTime ? 1 : 0, model = model, CollectionID = CollectionID, GoodID = GoodID, IsCollection = IsCollection, IsGoodFor = IsGoodFor });
        }

        /// <summary>
        /// 点赞商圈
        /// </summary>
        /// <returns></returns>
        public JsonResult Good(Guid IID)
        {
            InfomationsBLL bll = new InfomationsBLL();
            var result = bll.Good(IID, UserInfo.ID);
            if (result > 0)
                return RJson("1", "点赞成功");
            else
                return RJson(result.ToString(), "点赞失败：" + ((Common.ErrorCode)result).ToString());
        }

        /// <summary>
        /// 评论商圈
        /// </summary>
        /// <returns></returns>
        public JsonResult Comment(Guid IID, string content)
        {
            InfomationsBLL bll = new InfomationsBLL();
            var result = bll.Comment(IID, UserInfo.ID, content);
            return Json(new { code = result, msg = "评论成功" });
        }

        /// <summary>
        /// 获取所有商圈评论
        /// </summary>
        /// <returns></returns>
        public JsonResult GetComment(Guid IID)
        {
            InfomationsBLL bll = new InfomationsBLL();
            var list = bll.GetComments(IID, PageSize, PageIndex, out DataCount);

            return Json(new { code = 1, pagecount = TotalPageCount, list = list });
        }

    }
}