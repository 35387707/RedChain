using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;

namespace RelaxBarWeb_MVC.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        [Filter.CheckLogin]
        public JsonResult Detail(Guid ID)
        {
            ProductsBLL bll = new ProductsBLL();
            var model = bll.GetAppProduct(ID);
            MyCollectionBLL mbll = new MyCollectionBLL();
            var Collect = mbll.GetMyCollect(UserInfo.ID, ID, Common.enStatus.Enabled, Common.enMycollectionType.Product);
            
            var CollectionID = Collect == null ? "0" : Collect.ID.ToString();
            var FollowerID = Collect == null ? "0" : Collect.ID.ToString();
            if (model != null)
            {
                var buy = new OrdersBLL().GetOrdersByPro(model.ID, PageSize, 1, out DataCount);

                int IsCollection = mbll.GetCollectTotal(UserInfo.ID, ID, Common.enStatus.Enabled, Common.enMycollectionType.Product) == 0 ? 0 : 1; //收藏状态（0未收藏，1已收藏） 

                int IsFollower = mbll.GetCollectTotal(UserInfo.ID, ID, Common.enStatus.Enabled, Common.enMycollectionType.Product) == 0 ? 0 : 1; //关注状态（0未关注，1已关注） 
              //  var Balance = UserInfo.Balance; //用户余额
                return Json(new { code = 1, model = model, buylist = buy, IsCollection= IsCollection, IsFollower= IsFollower,CollectionID= CollectionID, FollowerID= FollowerID, });//商品详情
            }
            return RJson("-1", "商品不存在");
        }


        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public JsonResult List(int? CID, string key)
        {
            ProductsBLL bll = new ProductsBLL();
            if (!CID.HasValue)
            {
                CID = 0;
            }
            var list = bll.GetAppProductList(CID, key, (int)(Common.enStatus.Enabled), PageSize, PageIndex, out DataCount);

            return Json(new { code = 1, pagecount = TotalPageCount, list = list });//获取商品列表
        }

        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public JsonResult Categorist(int? headid)
        {
            CategoryBLL bll = new CategoryBLL();
            var list = bll.GetAllList(true, "", headid);

            return Json(new { code = 1, list = list });//获取商品列表
        }


        public JsonResult ProductSpec_API(Guid productId)
        {

            ProductsBLL bll = new ProductsBLL();
            var list = bll.GetProductSpecification(productId);
            return Json(new { code = 1, list = list });//获取规格列表

        }
    }
}