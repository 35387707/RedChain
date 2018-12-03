using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RelexBarDLL;
using RelexBarBLL;
using RelaxBarWeb_MVC.Utils;
using static RelexBarBLL.Common;
using System.Data.SqlClient;
using System.Data;
using RelexBarBLL.Models;
using System.IO;
using System.Text;
using NPOI;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace RelaxBarWeb_MVC.Controllers
{
    public class ExcelController: BaseController
    {
        /// <summary>
        /// 提现数据导出
        /// </summary>
        /// <returns></returns>
        public FileResult TransFerOutExcel()
        {

            //传递的参数
            string key = Request.QueryString["key"];
            string beginTime = Request.QueryString["beginTime"];
            string endTime = Request.QueryString["endTime"];
            DateTime? begin; DateTime? end;

            if (string.IsNullOrEmpty(beginTime))
            {
                begin = null;

            }
            else
            {
                begin = Convert.ToDateTime(beginTime);
            }
            if (string.IsNullOrEmpty(endTime))
            {
                end = null;
            }
            else
            {
                end = Convert.ToDateTime(endTime).AddDays(1);
              //  endTime.Value.AddDays(1).ToString("yyyy-MM-dd")
            }
            //获取list数据
            TransferOutBLL bll = new TransferOutBLL();
           // int sum;
            List<TransferOutModel> list = new List<TransferOutModel>();
            //  list = bll.GetList(null, null, null, null, 10, 1, out sum, null);
            list = bll.GetAllTansferOutList(key, begin, end);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("登陆账号");
            row1.CreateCell(1).SetCellValue("昵称");
            row1.CreateCell(2).SetCellValue("银行信息");
            row1.CreateCell(3).SetCellValue("提现额度");
            row1.CreateCell(4).SetCellValue("预计手续费");
            row1.CreateCell(5).SetCellValue("提现原因");
            row1.CreateCell(6).SetCellValue("拒绝原因");
            row1.CreateCell(7).SetCellValue("处理状态");
            row1.CreateCell(8).SetCellValue("申请时间");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Phone);
                rowtemp.CreateCell(1).SetCellValue(list[i].Name);
                rowtemp.CreateCell(2).SetCellValue("银行:" + list[i].BankName + " -- 支行：" + list[i].BankZhiHang + "  账户:" + list[i].BankAccount + "  姓名:" + list[i].BankUser);
                rowtemp.CreateCell(3).SetCellValue(list[i].Price.ToString());
                rowtemp.CreateCell(4).SetCellValue(list[i].ComPrice.ToString());
                rowtemp.CreateCell(5).SetCellValue(list[i].Reason);
                rowtemp.CreateCell(6).SetCellValue(list[i].ApplyRemark);
                rowtemp.CreateCell(7).SetCellValue(((list[i].Status == 0) ? "未处理" : (list[i].Status == 1) ? "已提取" : "已拒绝"));
                rowtemp.CreateCell(8).SetCellValue(list[i].CreateTime.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            //  File(ms, "application/vnd.ms-excel", "提现数据总表" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls");

            //Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", "提现数据总表" + DateTime.Now.ToString("yyyyMMddHHmmssfff")));
            //Response.BinaryWrite(ms.ToArray());
            //book = null;
            //ms.Close();
            //  Response.AddHeader(ms, "application/vnd.ms-excel", "提现数据总表" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls");


            // string excelPath = this.Server.MapPath(string.Format("attachment/vnd.ms-excel; filename={0}.xls", "提现数据总表" + DateTime.Now.ToString("yyyyMMddHHmmssfff")));

            // return RJson("1", excelPath);
            return File(ms, "application/vnd.ms-excel", "提现数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }


        /// <summary>
        /// 佣金数据导出
        /// </summary>
        /// <returns></returns>
        public FileResult PayRewarsExcel()
        {
            //传递的参数
            string key = Request.QueryString["key"];
            string beginTime = Request.QueryString["beginTime"];
            string endTime = Request.QueryString["endTime"];
            DateTime? begin; DateTime? end;
          
            if (string.IsNullOrEmpty(beginTime))
            {
                begin = null;

            }
            else
            {
                begin = Convert.ToDateTime(beginTime);
            }
            if (string.IsNullOrEmpty(endTime))
            {
                end = null;
            }
            else
            {
                end = Convert.ToDateTime(endTime);
            }
            //获取list数据
            PayListBLL bll = new PayListBLL();
            int sum;
            List<AdminPayListModel> list = new List<AdminPayListModel>();
            list = bll.GetAllPayRewarsList(key, begin, end);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("登陆账号");
            row1.CreateCell(1).SetCellValue("升级的用户昵称");
            row1.CreateCell(2).SetCellValue("手机号");
            row1.CreateCell(3).SetCellValue("昵称");
            row1.CreateCell(4).SetCellValue("金额");
            row1.CreateCell(5).SetCellValue("金额类型");
            row1.CreateCell(6).SetCellValue("获取途径");
            row1.CreateCell(7).SetCellValue("备注");
            row1.CreateCell(8).SetCellValue("消费时间");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Phone);
                rowtemp.CreateCell(1).SetCellValue(list[i].Name);
                rowtemp.CreateCell(2).SetCellValue(list[i].OPhone);
                rowtemp.CreateCell(3).SetCellValue(list[i].OName); //Model[i].InOut == 1 ? "+" : "-")@Model[i].Val
                rowtemp.CreateCell(4).SetCellValue(((list[i].InOut == 1) ? "+" : "-") +list[i].Val.ToString());
                rowtemp.CreateCell(5).SetCellValue((list[i].PayType == 1) ? "余额": "筹码");
                rowtemp.CreateCell(6).SetCellValue((list[i].FromTo == 13) ? "佣金" : "红包");
                rowtemp.CreateCell(7).SetCellValue(list[i].Remark);
                rowtemp.CreateCell(8).SetCellValue(list[i].CreateTime.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "佣金数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }


        public FileResult PayListExcel(string phone,DateTime? beginTime,DateTime? endTime, enPayFrom? FromTo, enPayInOutType? InOut, enPayType? PayType)
        {
            //传递的参数
            //string phone = Request.QueryString["phone"];
            //string FromTo = Request.QueryString["FromTo"];
            //string InOut = Request.QueryString["InOut"];
            //// if(begi)
            //string beginTime = Request.QueryString["beginTime"];
            //string endTime = Request.QueryString["endTime"];
            //DateTime? begin; DateTime? end;
            //DateTime? beginTime = Convert.ToDateTime( Request.QueryString["beginTime"]);
            //DateTime? endTime = Convert.ToDateTime(Request.QueryString["endTime"]);
            //if (string.IsNullOrEmpty(beginTime))
            //{
            //    begin = null;
               
            //}
            //else
            //{
            //    begin = Convert.ToDateTime(beginTime);
            //}
            //if (string.IsNullOrEmpty(endTime))
            //{
            //    end = null;
            //}
            //else
            //{
            //    end = Convert.ToDateTime(endTime);
            //}
            //获取list数据
            PayListBLL bll = new PayListBLL();
            //int sum;
            List<AdminPayListModel> list = new List<AdminPayListModel>();
            //  list = bll.GetList(null, null, null, null, 10, 1, out sum, null);
            list = bll.GetAllPayList(phone, FromTo, InOut, PayType, beginTime, endTime);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("登陆账号");
            row1.CreateCell(1).SetCellValue("用户昵称");
            row1.CreateCell(2).SetCellValue("手机号");
            row1.CreateCell(3).SetCellValue("昵称");
            row1.CreateCell(4).SetCellValue("收支金额");
            row1.CreateCell(5).SetCellValue("金额类型");
            row1.CreateCell(6).SetCellValue("备注");
            row1.CreateCell(7).SetCellValue("消费时间");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Phone);
                rowtemp.CreateCell(1).SetCellValue(list[i].Name);
                rowtemp.CreateCell(2).SetCellValue(list[i].OPhone);
                rowtemp.CreateCell(3).SetCellValue(list[i].OName); //Model[i].InOut == 1 ? "+" : "-")@Model[i].Val
                rowtemp.CreateCell(4).SetCellValue(((list[i].InOut == 1) ? "+" : "-") + list[i].Val.ToString());
                rowtemp.CreateCell(5).SetCellValue((list[i].PayType == (int)RelexBarBLL.Common.enPayType.Coin ? "余额" : (list[i].PayType == (int)RelexBarBLL.Common.enPayType.Point ? "福音积分" : "福利积分")));
                rowtemp.CreateCell(6).SetCellValue(list[i].Remark);
                rowtemp.CreateCell(7).SetCellValue(list[i].CreateTime.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "收支数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }



        public FileResult UserExcel(string key,string UserType)
        {
            UsersBLL bll = new UsersBLL();
           
            List<Users> list= bll.GetUsersExcel(key,UserType);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("手机号");
            row1.CreateCell(1).SetCellValue("昵称");
            row1.CreateCell(2).SetCellValue("真实姓名");
            row1.CreateCell(3).SetCellValue("性别");
            row1.CreateCell(4).SetCellValue("级别");
            row1.CreateCell(5).SetCellValue("推荐码");
            row1.CreateCell(6).SetCellValue("余额");
            row1.CreateCell(7).SetCellValue("总消费");
            row1.CreateCell(8).SetCellValue("状态");
            row1.CreateCell(9).SetCellValue("推荐人");
            row1.CreateCell(10).SetCellValue("注册时间");
            row1.CreateCell(11).SetCellValue("最后登录时间");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue("(" + list[i].AreaCode +")"+ list[i].Phone);
                rowtemp.CreateCell(1).SetCellValue(list[i].Name);
                rowtemp.CreateCell(2).SetCellValue(list[i].TrueName);
                rowtemp.CreateCell(3).SetCellValue(((list[i].Sex == null||list[i].Sex == (int)RelexBarBLL.Common.enSex.Man) ? "男" : "女"));
                rowtemp.CreateCell(4).SetCellValue(((list[i].UserType == (int)RelexBarBLL.Common.enUserType.User) ? "福星" : (list[i].UserType == (int)RelexBarBLL.Common.enUserType.Shop) ? "福将" : "福相"));
                rowtemp.CreateCell(5).SetCellValue(list[i].CardNumber);
                rowtemp.CreateCell(6).SetCellValue(list[i].Balance.ToString());
                rowtemp.CreateCell(7).SetCellValue(list[i].TotalScore.ToString());
                rowtemp.CreateCell(8).SetCellValue((list[i].Status == 1) ? "启用" : "禁用");
                rowtemp.CreateCell(9).SetCellValue(list[i].FID.HasValue&&list[i].FID.Value!=Guid.Empty? bll.GetUserById(list[i].FID.Value).Phone:"无");
                rowtemp.CreateCell(10).SetCellValue(list[i].CreateTime.ToString());
                rowtemp.CreateCell(11).SetCellValue(list[i].LastLoginTime.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "application/vnd.ms-excel", "会员数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }

        public FileResult ProductsExcel(int? categoryid, int? status, string key)
        {
            ProductsBLL bll = new ProductsBLL();
            CategoryBLL bll2 = new CategoryBLL();
            List<ProductList> list = bll.GetProductExcel(categoryid, key, status);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("商品号");
            row1.CreateCell(1).SetCellValue("名称");
            row1.CreateCell(2).SetCellValue("标题");
            row1.CreateCell(3).SetCellValue("商品类型");
            row1.CreateCell(4).SetCellValue("库存");
            row1.CreateCell(5).SetCellValue("交易数量");
            row1.CreateCell(6).SetCellValue("访问量");
            row1.CreateCell(7).SetCellValue("点赞数");
            row1.CreateCell(8).SetCellValue("评分");
            row1.CreateCell(9).SetCellValue("创建时间");
            row1.CreateCell(10).SetCellValue("商品状态");
            row1.CreateCell(11).SetCellValue("排序");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Number);
                rowtemp.CreateCell(1).SetCellValue(list[i].Name);
                rowtemp.CreateCell(2).SetCellValue(list[i].Title);
                rowtemp.CreateCell(3).SetCellValue(bll2.GetDetail(list[i].CategoryID.Value).Name);
                rowtemp.CreateCell(4).SetCellValue(list[i].Stock.ToString());
                rowtemp.CreateCell(5).SetCellValue(list[i].CompleteCount.ToString());
                rowtemp.CreateCell(6).SetCellValue(list[i].ViewCount.ToString());
                rowtemp.CreateCell(7).SetCellValue(list[i].GoodCount.ToString());
                rowtemp.CreateCell(8).SetCellValue(list[i].CPoints.ToString());
                rowtemp.CreateCell(9).SetCellValue(list[i].CreateTime.ToString()); 
                rowtemp.CreateCell(10).SetCellValue((list[i].Status == 1) ? "启用" : "禁用");
                rowtemp.CreateCell(11).SetCellValue(list[i].OrderID.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "application/vnd.ms-excel", "商品数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }


        public FileResult OrderExcel(string phone, enOrderStatus? Status, DateTime? beginTime, DateTime? endTime)
        {


            OrdersBLL bll = new OrdersBLL();
            List<RelexBarBLL.Models.OrderListModel> list = bll.GetOrderListExcel(phone,Status,beginTime,endTime);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("订单号");
            row1.CreateCell(1).SetCellValue("商品名称");
            row1.CreateCell(2).SetCellValue("价格");
            row1.CreateCell(3).SetCellValue("数量");
            row1.CreateCell(4).SetCellValue("合计");
            row1.CreateCell(5).SetCellValue("收货信息");
            row1.CreateCell(6).SetCellValue("订单状态");
            row1.CreateCell(7).SetCellValue("下单时间");
            row1.CreateCell(8).SetCellValue("支付时间");
            row1.CreateCell(9).SetCellValue("发货时间");
            row1.CreateCell(10).SetCellValue("收货时间");
            row1.CreateCell(11).SetCellValue("完成时间");
            
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Number);
                rowtemp.CreateCell(1).SetCellValue(list[i].OName);
                rowtemp.CreateCell(2).SetCellValue(list[i].OPrice.ToString());
                rowtemp.CreateCell(3).SetCellValue(list[i].OCount.ToString());
                rowtemp.CreateCell(4).SetCellValue(list[i].Price.ToString());
                rowtemp.CreateCell(5).SetCellValue("会员账号：" + ((list[i].Name == null) ? "系统" : (list[i].TrueName + "(" + list[i].Phone + ")"))+ " 收货人："+list[i].RecName+"--"+list[i].RecPhone+" 收货地址："+list[i].RecAddress);
                rowtemp.CreateCell(6).SetCellValue((list[i].Status == -1) ? "取消" : ((list[i].Status == 0) ? "下单" : ((list[i].Status == 1) ? "已支付" : ((list[i].Status == 2) ? "已发货" : ((list[i].Status == 3) ? "已收货" : ((list[i].Status == 4) ? "已完成订单" : "退货中"))))));
                rowtemp.CreateCell(7).SetCellValue(list[i].CreateTime.ToString());
                rowtemp.CreateCell(8).SetCellValue(list[i].PayTime.ToString());
                rowtemp.CreateCell(9).SetCellValue(list[i].SendTime.ToString());
                rowtemp.CreateCell(10).SetCellValue(list[i].RecTime.ToString());
                rowtemp.CreateCell(11).SetCellValue(list[i].FinishTime.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "application/vnd.ms-excel", "订单数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }


        //发福包报表
        public FileResult RedPacketExcel(string key, string type, string userType, DateTime? beginTime, DateTime? endTime)
        {


            RedPacksBLL bll = new RedPacksBLL();
            List<RelexBarBLL.Models.RedPacksModel> list = bll.GetRedPacketExcel(key, userType, type, beginTime, endTime);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("用户名称");
            row1.CreateCell(1).SetCellValue("用户类型");
            row1.CreateCell(2).SetCellValue("内容");
            row1.CreateCell(3).SetCellValue("单价");
            row1.CreateCell(4).SetCellValue("福包个数");
            row1.CreateCell(5).SetCellValue("总价");
            row1.CreateCell(6).SetCellValue("福包类型");
            row1.CreateCell(7).SetCellValue("开始时间");
            row1.CreateCell(8).SetCellValue("结束时间");
            row1.CreateCell(9).SetCellValue("状态");

            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
      
                rowtemp.CreateCell(0).SetCellValue((list[i].Name == null) ? "系统" : (list[i].TrueName + "(" + list[i].Phone + ")"));
                rowtemp.CreateCell(1).SetCellValue(((list[i].UserType == 0) ? "福星" : ((list[i].UserType == 1) ? "福将" : ((list[i].UserType == 2) ? "福相" : "系统管理员"))));
                rowtemp.CreateCell(2).SetCellValue(list[i].title.ToString());
                rowtemp.CreateCell(3).SetCellValue(list[i].SinglePrice.ToString());
                rowtemp.CreateCell(4).SetCellValue(list[i].OncePacketCount.ToString());
                rowtemp.CreateCell(5).SetCellValue(list[i].TotalPrice.ToString());
                rowtemp.CreateCell(6).SetCellValue(((list[i].RedType == 1) ? "用户/商家主动发福包" : ((list[i].RedType == 2) ? "定时自动发红包" : ((list[i].RedType == 3) ? "系统福包" : ((list[i].RedType == 4) ? "达到条件自动发福包" : ((list[i].RedType == 5) ? "升级代理自动发福包" : "公司业绩自动发福包"))))));
                rowtemp.CreateCell(7).SetCellValue(list[i].BeginTime.ToString());
                rowtemp.CreateCell(8).SetCellValue(list[i].EndTime.ToString());
                rowtemp.CreateCell(9).SetCellValue((list[i].Status == 1) ? "启用" : "禁用");
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "application/vnd.ms-excel", "发福包数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }

        //收福包报表
        public FileResult RedPacketListExcel(string key, string redType, string userType, DateTime? beginTime, DateTime? endTime)
        {


            RedPacksBLL bll = new RedPacksBLL();
            List<RelexBarBLL.Models.RedPacksListModels> list = bll.GetRedPacketListExcel(key, userType, redType, beginTime, endTime, null);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("用户名称");
            row1.CreateCell(1).SetCellValue("用户类型");
            row1.CreateCell(2).SetCellValue("标题");
            row1.CreateCell(3).SetCellValue("金额");
            row1.CreateCell(4).SetCellValue("福包流水号");
            row1.CreateCell(5).SetCellValue("创建时间");
            row1.CreateCell(6).SetCellValue("开始时间");
            row1.CreateCell(7).SetCellValue("结束时间");
            row1.CreateCell(8).SetCellValue("领取时间");

            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);

                rowtemp.CreateCell(0).SetCellValue((list[i].Name == null) ? "系统" : (list[i].TrueName + "(" + list[i].Phone + ")"));
                rowtemp.CreateCell(1).SetCellValue(((list[i].UserType == 0) ? "福星" : ((list[i].UserType == 1) ? "福将" : ((list[i].UserType == 2) ? "福相" : "系统管理员"))));
                rowtemp.CreateCell(2).SetCellValue(list[i].title.ToString());
                rowtemp.CreateCell(3).SetCellValue(list[i].Money.ToString());
                rowtemp.CreateCell(4).SetCellValue(list[i].Number.ToString());
                rowtemp.CreateCell(5).SetCellValue(list[i].CreateTime.ToString());
                rowtemp.CreateCell(6).SetCellValue(list[i].BeginTime.ToString());
                rowtemp.CreateCell(7).SetCellValue(list[i].EndTime.ToString());
                rowtemp.CreateCell(8).SetCellValue(list[i].UpdateTime.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "application/vnd.ms-excel", "收福包数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }


        //福音天地报表
        public FileResult InformationsExcel(string key, DateTime? beginTime, DateTime? endTime)
        {


            InfomationsBLL bll = new InfomationsBLL();
            List<RelexBarBLL.Models.InfomationsModel> list = bll.GetInfomationsExcel(key, beginTime, endTime);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("用户名称");
            row1.CreateCell(1).SetCellValue("标题");
            row1.CreateCell(2).SetCellValue("类型");
            row1.CreateCell(3).SetCellValue("访问量");
            row1.CreateCell(4).SetCellValue("点赞数");
            row1.CreateCell(5).SetCellValue("状态");
            row1.CreateCell(6).SetCellValue("开始时间");
            row1.CreateCell(7).SetCellValue("结束时间");
            row1.CreateCell(8).SetCellValue("更新时间");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);

                rowtemp.CreateCell(0).SetCellValue(list[i].Name + "--" + list[i].TrueName + "--" + list[i].Phone);
                rowtemp.CreateCell(1).SetCellValue(list[i].title.ToString());
                rowtemp.CreateCell(2).SetCellValue(bll.GetInfomationTypeById(list[i].Type).Name);
                rowtemp.CreateCell(3).SetCellValue(list[i].ViewCount.ToString());
                rowtemp.CreateCell(4).SetCellValue(list[i].GoodCount.ToString());
                rowtemp.CreateCell(5).SetCellValue((list[i].Status == 1) ? "启用" : "禁用");
                rowtemp.CreateCell(6).SetCellValue(list[i].BeginTime.ToString());
                rowtemp.CreateCell(7).SetCellValue(list[i].EndTime.ToString());
                rowtemp.CreateCell(8).SetCellValue(list[i].UpdateTime.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "application/vnd.ms-excel", "福音天地数据总表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }


    }
}