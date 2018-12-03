using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using RelexBarBLL;
using System.Web;

namespace RelexBarBLL
{
    /// <summary>
    /// 系统变量
    /// </summary>
    public class SysConfigBLL
    {
        //常用变量：
        private const string MD5KEY = "MD5KEY";
        private const string SMSURL = "SMSURL";
        private const string SMSUSER = "SMSUSER";
        private const string SMSPSW = "SMSPSW";
        private const string EMAILUSER = "EMAILUSER";
        private const string EMAILPSW = "EMAILPSW";
        private const string EMAILSERVER = "EMAILSERVER";
        private const string POUNDAGE = "POUNDAGE";
        private const string TRANSOUT = "TRANSOUT";

        //红包金额设置
        private const string REDLVSMALL1 = "REDLVSMALL1";
        private const string REDLVSMALL2 = "REDLVSMALL2";
        private const string REDLVBIG1 = "REDLVBIG1";
        private const string REDLVBIG2 = "REDLVBIG2";

        /// <summary>
        /// 系统福包池
        /// </summary>
        private const string SYSTEMMONEY = "SYSTEMMONEY";

        //微信参数设置
        private const string WXAPPID = "WXAPPID";
        private const string WX_APPID = "WX_APPID";
        private const string WXMCHID = "WXMCHID";
        private const string WXKEY = "WXKEY";
        private const string WXPAYKEY = "WXPAYKEY";
        private const string WXPAYNOTIFY = "WXPAYNOTIFY";
        //支付宝参数设置
        private const string ALIPAYKEY = "ALIPAYKEY";
        private const string ALIPAY_APP_PRIVATE_KEY = "ALIPAY_APP_PRIVATE_KEY";
        private const string ALIPAY_PUBLIC_KEY = "ALIPAY_PUBLIC_KEY";
        private const string ALIPAYNOTIFY = "ALIPAYNOTIFY";
        //威富通参数设置
        private const string WFT_APPID = "WFT_APPID";
        private const string WFT_MCHID = "WFT_MCHID";
        private const string WFT_KEY = "WFT_KEY";

        public static List<SysConfig> SysConfigList;//所有可用的系统配置

        private static string _md5key = string.Empty;
        public static string MD5Key
        {
            get
            {
                return _md5key;
            }
        }

        private static string _SMSUrl = string.Empty;
        public static string SMSUrl
        {
            get
            {
                return _SMSUrl;
            }
        }

        private static string _SMSUser = string.Empty;
        public static string SMSUser
        {
            get
            {
                return _SMSUser;
            }
        }

        private static string _SMSPsw = string.Empty;
        public static string SMSPsw
        {
            get
            {
                return _SMSPsw;
            }
        }

        private static string _SMSSignName = string.Empty;
        public static string SMSSignName
        {
            get
            {
                return _SMSSignName;
            }
        }

        private static string _SMSTemplateCode = string.Empty;
        public static string SMSTemplateCode
        {
            get
            {
                return _SMSTemplateCode;
            }
        }

        private static string _SMSTemplateCode2 = string.Empty;
        public static string SMSTemplateCode2
        {
            get
            {
                return _SMSTemplateCode2;
            }
        }

        private static string _EmailUser = string.Empty;
        public static string EmailUser
        {
            get
            {
                return _EmailUser;
            }
        }

        private static string _EmailPsw = string.Empty;
        public static string EmailPsw
        {
            get
            {
                return _EmailPsw;
            }
        }

        private static string _EmailServer = string.Empty;
        public static string EmailServer
        {
            get
            {
                return _EmailServer;
            }
        }

        private static decimal _Transout = 0.1M;//提现手续费
        /// <summary>
        /// 提现手续费
        /// </summary>
        public static decimal Transout//手续费
        {
            get
            {
                return _Transout;
            }
        }
        private static decimal _Poundage = 0.1M;//自动扣除10%的收益继续发红包
        /// <summary>
        /// 转出手续费
        /// </summary>
        public static decimal Poundage//自动扣除10%的收益继续发红包
        {
            get
            {
                return _Poundage;
            }
        }

        private static decimal _SystemAchieveSend = 3000000;
        /// <summary>
        /// 公司总业绩每增加300万业绩，拿出2%累计60000元
        /// </summary>
        public static decimal SystemAchieveSend
        {
            get
            {
                return _SystemAchieveSend;
            }
        }

        private static object lockSystemMoney = new object();
        private static decimal _SystemMoney = 0;
        /// <summary>
        /// 公司总业绩(个人发红包+会员升级为业绩)
        /// </summary>
        public static decimal SystemMoney
        {
            get
            {
                return _SystemMoney;
            }
        }

        private static decimal _ShopPrice = 3900;//商家价格
        /// <summary>
        /// 商家价格
        /// </summary>
        public static decimal ShopPrice//商家价格
        {
            get
            {
                return _ShopPrice;
            }
        }
        private static decimal _AgentPrice = 9900;//代理价格
        /// <summary>
        /// 代理价格
        /// </summary>
        public static decimal AgentPrice//代理价格
        {
            get
            {
                return _AgentPrice;
            }
        }

        private const string domain = "DOMAIN";//域名
        private static string _DOMAIN;//
        public static string DOMAIN
        {
            get
            {
                return _DOMAIN;
            }

            set
            {
                _DOMAIN = value;
            }
        }

        private const string pollingMaxTime = "PollingMaxTime";//长轮询最大等待时间
        private static int _PollingMaxTime = 10;//默认10秒
        public static int PollingMaxTime
        {
            get
            {
                return _PollingMaxTime;
            }

            set
            {
                _PollingMaxTime = value;
            }
        }
        #region 阿里推送配置
        private const string regionID = "REGIONID";//
        private static string _REGIONID = "cn-hangzhou";//默认
        public static string REGIONID
        {
            get
            {
                return _REGIONID;
            }

            set
            {
                _REGIONID = value;
            }
        }
        private const string accessKeyId = "AccessKeyId";//
        private static string _AccessKeyId;
        public static string AccessKeyId
        {
            get
            {
                return _AccessKeyId;
            }

            set
            {
                _AccessKeyId = value;
            }
        }
        private const string accessKeySecret = "AccessKeySecret";//
        private static string _AccessKeySecret;
        public static string AccessKeySecret
        {
            get
            {
                return _AccessKeySecret;
            }

            set
            {
                _AccessKeySecret = value;
            }
        }
        private const string aliPushAppKey_Android = "ALiPushAppKey_Android";//
        private static long _ALiPushAppKey_Android;
        public static long ALiPushAppKey_Android
        {
            get
            {
                return _ALiPushAppKey_Android;
            }

            set
            {
                _ALiPushAppKey_Android = value;
            }
        }
        private const string aliPushAppKey_IOS = "ALiPushAppKey_IOS";//
        private static long _ALiPushAppKey_IOS;
        public static long ALiPushAppKey_IOS
        {
            get
            {
                return _ALiPushAppKey_IOS;
            }

            set
            {
                _ALiPushAppKey_IOS = value;
            }
        }
        private const string _IOSEnvironment = "IOSEnvironment";//
        private static string __IOSEnvironment;
        public static string IOSEnvironment
        {
            get
            {
                return __IOSEnvironment;
            }

            set
            {
                __IOSEnvironment = value;
            }
        }
        #endregion

        public void InitConfig()
        {
            SysConfigList = GetAllConfig();//可用的系统配置拿出来
            _userHelp = GetAllUserHelp();
            #region default
            //特殊配置需要单独处理出来
            var k = SysConfigList.FirstOrDefault(m => m.Name == MD5KEY);
            if (k != null)
                _md5key = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == SMSURL);
            if (k != null)
                _SMSUrl = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == SMSUSER);
            if (k != null)
                _SMSUser = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == SMSPSW);
            if (k != null)
                _SMSPsw = k.Value;

            k = SysConfigList.FirstOrDefault(m => m.Name == "SMSSignName");
            if (k != null)
                _SMSSignName = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == "SMSTemplateCode");
            if (k != null)
                _SMSTemplateCode = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == "SMSTemplateCode2");
            if (k != null)
                _SMSTemplateCode2 = k.Value;

            k = SysConfigList.FirstOrDefault(m => m.Name == EMAILUSER);
            if (k != null)
                _EmailUser = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == EMAILPSW);
            if (k != null)
                _EmailPsw = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == EMAILSERVER);
            if (k != null)
                _EmailServer = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == POUNDAGE);
            if (k != null)
                _Poundage = decimal.Parse(k.Value);
            k = SysConfigList.FirstOrDefault(m => m.Name == TRANSOUT);
            if (k != null)
                _Transout = decimal.Parse(k.Value);

            k = SysConfigList.FirstOrDefault(m => m.Name == "SystemAchieveSend");
            if (k != null)
                _SystemAchieveSend = decimal.Parse(k.Value);

            //微信接口参数
            k = SysConfigList.FirstOrDefault(m => m.Name == WXAPPID);
            if (k != null)
                Services.WX_Services.WxPayData.wxAppid = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == WX_APPID);
            if (k != null)
                Services.WX_Services.WxPayData.Appid = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == WXMCHID);
            if (k != null)
                Services.WX_Services.WxPayData.wxMCHID = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == WXKEY);
            if (k != null)
                Services.WX_Services.WxPayData.wxKey = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == WXPAYKEY);
            if (k != null)
                Services.WX_Services.WxPayData.wxPayKey = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == WXPAYNOTIFY);
            if (k != null)
                Services.WX_Services.WxPayData.wxPayNotifyURL = k.Value;
            //支付宝接口参数
            k = SysConfigList.FirstOrDefault(m => m.Name == ALIPAYKEY);
            if (k != null)
                Services.AliPay.APPID = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == ALIPAY_APP_PRIVATE_KEY);
            if (k != null)
                //Services.AliPay.APP_PRIVATE_KEY = k.Value.Replace(" ","+");
                Services.AliPay.APP_PRIVATE_KEY = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/alipaykey/rsa_private_key.pem"));
            k = SysConfigList.FirstOrDefault(m => m.Name == ALIPAY_PUBLIC_KEY);
            if (k != null)
                //Services.AliPay.ALIPAY_PUBLIC_KEY = k.Value.Replace(" ", "+");
                Services.AliPay.ALIPAY_PUBLIC_KEY = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/alipaykey/rsa_public_key.pem"));
            k = SysConfigList.FirstOrDefault(m => m.Name == ALIPAYNOTIFY);
            if (k != null)
                Services.AliPay.aliPayNotifyURL = k.Value;
            //威富通接口参数（第三方）
            k = SysConfigList.FirstOrDefault(m => m.Name == WFT_APPID);
            if (k != null)
                Services.weifutong_pay.APPID = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == WFT_MCHID);
            if (k != null)
                Services.weifutong_pay.MCHID = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == WFT_KEY);
            if (k != null)
                Services.weifutong_pay.KEY = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == "MessageCount");
            if (k != null)
            {
                messageCount = int.Parse(k.Value);
            }
            k = SysConfigList.FirstOrDefault(m => m.Name == domain);
            if (k != null)
                DOMAIN = k.Value;
            k = SysConfigList.FirstOrDefault(m => m.Name == pollingMaxTime);
            if (k != null)
            {
                try
                {
                    PollingMaxTime = int.Parse(k.Value);
                }
                catch (Exception)
                {
                    _PollingMaxTime = 10;
                }
            }
            #endregion

            ReflashSystemMoney(null);//刷新系统总业绩

            #region 阿里推送配置
            k = SysConfigList.FirstOrDefault(m => m.Name == regionID);
            if (k != null)
            {
                REGIONID = k.Value;
            }
            k = SysConfigList.FirstOrDefault(m => m.Name == accessKeyId);
            if (k != null)
            {
                AccessKeyId = k.Value;
            }
            k = SysConfigList.FirstOrDefault(m => m.Name == accessKeySecret);
            if (k != null)
            {
                AccessKeySecret = k.Value;
            }
            k = SysConfigList.FirstOrDefault(m => m.Name == aliPushAppKey_Android);
            if (k != null)
            {
                ALiPushAppKey_Android = long.Parse(k.Value);
            }
            k = SysConfigList.FirstOrDefault(m => m.Name == aliPushAppKey_IOS);
            if (k != null)
            {
                ALiPushAppKey_IOS = long.Parse(k.Value);
            }
            k = SysConfigList.FirstOrDefault(m => m.Name == _IOSEnvironment);
            if (k != null)
            {
                __IOSEnvironment = k.Value;
            }
            #endregion
        }

        /// <summary>
        /// 刷新系统福包池
        /// </summary>
        public static void ReflashSystemMoney(decimal? addPrice)
        {
            lock (lockSystemMoney)
            {
                using (RelexBarEntities db = new RelexBarEntities())
                {
                    var model = db.SysConfig.FirstOrDefault(m => m.Name == SYSTEMMONEY);
                    if (model == null)
                    {
                        new SysConfigBLL().Insert(SYSTEMMONEY, "", "系统总福包池", (int)Common.enStatus.Enabled);
                        model = db.SysConfig.FirstOrDefault(m => m.Name == SYSTEMMONEY);
                    }
                    if (addPrice.HasValue)
                    {
                        _SystemMoney += addPrice.Value;
                    }
                    else
                    {
                        var agentCount = db.Users.Where(m => m.UserType == (int)Common.enUserType.Agent).Count();
                        var shopCount = db.Users.Where(m => m.UserType == (int)Common.enUserType.Shop).Count();
                        var totalfb = db.RedPacket.Where(m => m.RedType == (int)Common.enRedType.Single).Sum(m => (decimal?)m.TotalPrice);//个人发的红包
                        _SystemMoney = agentCount * AgentPrice + shopCount * ShopPrice;
                        if (totalfb.HasValue)
                        {
                            _SystemMoney += totalfb.Value;
                        }
                    }
                    model.Value = _SystemMoney.ToString();
                    db.SaveChanges();
                }
            }
        }

        public List<SysConfig> GetAllConfig(string keyword, int pagesize, int pageinex, out int count)
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                var q = db.SysConfig.OrderBy(m => m.ID).Where(m => 1 == 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    q = q.Where(m => m.Name.Contains(keyword) || m.Value.Contains(keyword) || m.Descrition.Contains(keyword));
                }
                return BaseBll.GetPagedList2(q, pagesize, pageinex, out count);
            }
        }

        public List<SysConfig> GetAllConfig()
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                var q = db.SysConfig.Where(m => m.Status == (int)Common.enStatus.Enabled);
                return q.ToList();
            }
        }

        public List<UserHelp> GetAllUserHelp()
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                return db.UserHelp.ToList();
            }
        }

        public int Insert(string name, string value, string desc, int status)
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                if (db.SysConfig.FirstOrDefault(m => m.Name == name) != null)//存在，不能加入
                {
                    return 0;
                }
                SysConfig config = new SysConfig();
                config.Name = name;
                config.Value = value;
                config.Descrition = desc;
                config.Status = status;
                config.CreateTime = config.UpdateTime = DateTime.Now;

                db.SysConfig.Add(config);

                int i = db.SaveChanges();
                if (i > 0)
                {
                    
                    InitConfig();
                }
                return i;
            }
        }

        public int Update(int ID, string value, string desc, int? status)
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                var model = db.SysConfig.FirstOrDefault(m => m.ID == ID);
                if (model == null)//不存在
                {
                    return 0;
                }
                model.Value = value;
                model.Descrition = desc;
                if (status != null)
                    model.Status = status.Value;
                model.UpdateTime = DateTime.Now;

                int i = db.SaveChanges();
                if (i > 0)
                {
                    LogsBLL bll = new LogsBLL();
                    bll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作修改系统设置成功,系统设置描述:{1}", (HttpContext.Current.Session["admin"] as AdminUser).Name, model.Descrition),Common.enLogType.Admin);

                    InitConfig();
                }
                return i;
            }
        }
        public int UpdateStatus(int ID, Common.enStatus status)
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                var model = db.SysConfig.FirstOrDefault(m => m.ID == ID);
                if (model == null)//不存在
                {
                    return 0;
                }
                model.Status = (int)status;
                model.UpdateTime = DateTime.Now;

                int i = db.SaveChanges();
                if (i > 0)
                {
                    InitConfig();
                }
                return i;
            }
        }
        public SysConfig Details(int ID)
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                var model = db.SysConfig.FirstOrDefault(m => m.ID == ID);
                return model;
            }
        }

        public int ChangeStatus(int ID, Common.enStatus status)
        {
            using (RelexBarEntities db = new RelexBarEntities())
            {
                var model = db.SysConfig.FirstOrDefault(m => m.ID == ID);
                if (model == null)//不存在
                {
                    return 0;
                }
                model.Status = (int)status;
                model.UpdateTime = DateTime.Now;

                return db.SaveChanges();
            }
        }
        public string Get(string key)
        {
            //SysConfig conf= SysConfigList.Where(m => m.Name == key).FirstOrDefault();
            //return conf == null ? null : conf.Value;
            using (RelexBarEntities entity = new RelexBarEntities())
            {
                SysConfig c = entity.SysConfig.Where(m => m.Name == key).FirstOrDefault();
                return c == null ? null : c.Value;
            }
        }

        private static List<UserHelp> _userHelp = new List<RelexBarDLL.UserHelp>();
        /// <summary>
        /// 获取用户帮助
        /// </summary>
        /// <returns></returns>
        public static List<UserHelp> UserHelp
        {
            get
            {
                return _userHelp;
            }
        }

        #region 
        /// <summary>
        /// 获取消息最大数量
        /// </summary>
        private static int? messageCount;

        public static int MessageCount
        {
            get
            {
                if (messageCount == null)
                {
                    messageCount = 50;
                }
                return messageCount.Value;
            }

            set
            {
                messageCount = value;
            }
        }

        #endregion
    }
}
