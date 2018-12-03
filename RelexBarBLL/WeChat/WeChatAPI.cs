using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.WeChat
{
    public interface WeChatAPI
    {
        string Login(string appID,string appSecret,string code);
        WxUserInfo GetUserInfo(string code,string session_key);
        /// <summary>
        /// 统一下单
        /// </summary>
        /// <returns></returns>
        WxPayData Unifiedorder(string gzhAppid, string MCHID, string MCHIDKEY, string requestIP, WxPayData inputObj);
        /**
        *    
        * 查询订单
        * @param WxPayData inputObj 提交给查询订单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回订单查询结果，其他抛异常
        */
        WxPayData OrderQuery(string gzhAppid, string MCHID,string MCHIDKEY,string ip, WxPayData inputObj, int timeOut = 6);

    }
}
