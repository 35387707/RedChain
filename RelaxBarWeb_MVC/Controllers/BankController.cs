using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarBLL;
using RelexBarDLL;
namespace RelaxBarWeb_MVC.Controllers
{
    [Filter.CheckLogin]
    public class BankController : BaseController
    {
        /// <summary>
        /// 添加银行卡
        /// </summary>
        /// <param name="bankName"></param>
        /// <param name="bankZhiHang"></param>
        /// <param name="bankAccount"></param>
        /// <param name="bankUser"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        public JsonResult Add(string bankName, string bankZhiHang, string bankAccount, string bankUser, int? isDefault)
        {
            BankListBLL bll = new BankListBLL();
            Guid g = bll.Insert(UserInfo.ID, bankName, bankZhiHang, bankAccount, bankUser, isDefault.HasValue && isDefault == 1);
            if (g != Guid.Empty)
            {
                return RJson("1", g.ToString());
            }
            return RJson("-1", "Error");
        }
        /// <summary>
        /// 更新默认收货地址
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        public JsonResult EditDefault(Guid id, int isDefault)
        {
            BankListBLL bll = new BankListBLL();
            var g = bll.EditDefault(id, UserInfo.ID, isDefault == 1);
            return RJson(g.ToString(), "");
        }
        /// <summary>
        /// 获取默认银行卡
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        public JsonResult GetDefault()
        {
            BankListBLL bll = new BankListBLL();
            var model = bll.GetDefault(UserInfo.ID);
            return Json(new { code = "1", model = model });
        }

        /// <summary>
        /// 管理银行卡
        /// </summary>
        /// <returns></returns>
        public ActionResult BankList()
        {
            BankListBLL bll = new BankListBLL();
            List<BankList> list = bll.GetUserBankList(UserInfo.ID);
            return View(list);
        }
        /// <summary>
        /// 获取银行卡列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBankList()
        {
            BankListBLL bll = new BankListBLL();
            return Json(new { code = 1, list = bll.GetUserBankList(UserInfo.ID) });
        }
        /// <summary>
        /// 添加银行卡
        /// </summary>
        /// <returns></returns>
        public ActionResult EditBank(Guid? id)
        {
            List<dynamic> list = new List<dynamic>();
            list.Add(new { BankName = "中国农业银行" });
            list.Add(new { BankName = "中国工商银行" });
            list.Add(new { BankName = "中国建设银行" });
            list.Add(new { BankName = "中国民生银行" });
            list.Add(new { BankName = "中国邮政银行" });
            list.Add(new { BankName = "招商银行" });
            SelectList sli = new SelectList(list, "BankName", "BankName");
            ViewData["bankList"] = sli;
            BankList bank;
            if (id != null)
            {
                BankListBLL bll = new BankListBLL();
                bank = bll.GetDetail(id.Value);
                if (bank == null)
                {
                    bank = new BankList();
                }
            }
            else
            {
                bank = new BankList();
            }

            return View(bank);
        }
        /// <summary>
        /// 选择银行卡
        /// </summary>
        /// <returns></returns>
        public ActionResult ChooseBankCard()
        {
            BankListBLL bll = new BankListBLL();
            List<BankList> list = bll.GetUserBankList(UserInfo.ID);
            return PartialView(list);
        }

        public JsonResult DoEditBank(RelexBarDLL.BankList bank)
        {
            BankListBLL bll = new BankListBLL();
            if (bank.ID != Guid.Empty)
            {

                //更新操作
                return RJson("1", "修改成功");
            }
            else
            {
                Guid id = bll.Insert(UserInfo.ID, bank.BankName, bank.BankZhiHang, bank.BankAccount, bank.BankUser, false);
                if (id == Guid.Empty)
                {
                    return RJson("-1", "新增失败");
                }
                else
                {
                    return RJson("1", "新增成功");
                }
            }
        }
        public JsonResult Delete(Guid? id)
        {
            BankListBLL bll = new BankListBLL();
            if (id == null)
            {
                return RJson("-1", "参数不正确");
            }
            int i = bll.Delete(id.Value);
            if (i > 0)
            {
                return RJson("1", "银行卡删除成功");
            }
            else
            {
                return RJson("-1", "银行卡删除失败");
            }
        }
    }
}