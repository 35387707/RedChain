using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.WeChat
{
    public static class WxConfig
    {
        //公司
        //public static string AppID = "wx23cf7ed19cde7509";
        //public static string AppSecret = "58abf96668d3d57ca70570777dd82dfd";
        //测试
        /// <summary>
        /// 小程序appid
        /// </summary>
        public static string AppID;
        /// <summary>
        /// 小程序appsecret
        /// </summary>
        public static string AppSecret;
        /// <summary>
        /// 商户号
        /// </summary>
        public const string MCHID = "1233410002";
        /// <summary>
        /// 商户号key
        /// </summary>
        public const string KEY = "e10adc3849ba56abbe56e056f20f883e";
        //=======【商户系统后台机器IP】===================================== 
        /* 此参数可手动配置也可在程序中自动获取
        */
        public const string IP = "192.168.0.163";
    }
}
