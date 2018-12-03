using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Push.Model.V20160801;
using RelaxBarWeb_MVC.Models;
using RelexBarBLL;
using RelexBarBLL.EnumCommon;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RelaxBarWeb_MVC.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Filter.CheckLogin]
    public class APPController : Controller
    {
        public static Dictionary<Guid, IMService> serviceList = new Dictionary<Guid, IMService>();
        private static ChatMessageBLL cbll = new ChatMessageBLL();
        private static object sync = new object();
        private static Task task;
        private static int PollingCount = SysConfigBLL.PollingMaxTime;
        public APPController() : base()
        {
            lock (sync)
            {
                if (task == null || task.Status != TaskStatus.Running)
                {
                    if (task != null)
                    {
                        task.Dispose();
                    }
                    task = new Task(TaskHandle);
                    task.Start();
                }
            }
            //CreateThread();
        }
        public void TaskHandle()
        {
            while (true)
            {
                Thread.Sleep(1000 * 60 * 60);
                DateTime time = DateTime.Now.AddDays(-5);
                List<Guid> rmKey = new List<Guid>();
                foreach (var item in serviceList)
                {
                    if (!item.Value.onLine && item.Value.ConnectTime < time)
                    {
                        rmKey.Add(item.Key);
                    }
                }
                try
                {
                    for (int i = 0; i < rmKey.Count; i++)
                    {
                        serviceList.Remove(rmKey[i]);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public string CreateImtoken(Users user)
        {
            return Common.MD5(user.ID + user.LastLoginTime.ToString("yyyyMMddHHmmss"));
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <returns></returns>
        public JsonResult Init()
        {
            try
            {
                Users user = Session["user"] as Users;
                if (user == null)
                {
                    return Json(new { code = "-10000", msg = "重新登陆" });
                }
                LogsBLL.InsertAPILog("/APP/Init", user.ID, "");
                //Guid serviceID = Guid.NewGuid();
                string serviceID = CreateImtoken(user);
                if (serviceList.ContainsKey(user.ID))
                {
                    serviceList[user.ID].ID = serviceID;
                }
                else
                {
                    serviceList.Add(user.ID, new IMService(serviceID, user.ID, user.DEVICE, user.OS));
                }
                LogsBLL.InsertAPILog("/APP/Init", user.ID, string.Format("code=1,msg={0}", serviceID));
                return Json(new { code = "1", msg = serviceID });
            }
            catch (Exception ex)
            {
                LogsBLL.InsertAPILog("/APP/Init", Guid.Empty, "初始化失败：" + ex.Message);
                return Json(new { code = "-1", msg = "初始化失败" });
            }

        }

        /// <summary>
        /// 用户退出登陆
        /// </summary>
        /// <param name="RID">接收者id</param>
        /// <returns></returns>
        public int LoginOut(Guid RID)
        {
            try
            {
                ChatMessage message = new ChatMessage();
                message.MID = Guid.NewGuid();
                message.FUID = Guid.Empty;
                message.TUID = RID;
                message.TType = (int)MessageTType.System;
                message.MType = (int)MessageType.LoginOut;
                message.Content = "请重新登陆";
                message.HadRead = 0;
                message.IsDel = 0;
                message.Status = 1;
                message.CreateTime = DateTime.Now;
                message.UpdateTime = DateTime.Now;
                int i = cbll.Add(message);
                if (i > 0)
                {
                    IMService s = serviceList[RID];
                    if (s != null)
                    {
                        s.onLine = false;
                        s.ID = string.Empty;
                        s.DEVICE = "";
                        serviceList.Remove(RID);
                    }
                }
                return i;
            }
            catch (Exception ex)
            {
                LogsBLL.InsertAPILog("/APP/LoginOut", Guid.Empty, "loginout方法调用异常：" + ex.Message);
                return 0;
            }

        }
        /// <summary>
        /// 客户端轮询方法
        /// </summary>
        /// <returns></returns>
        public JsonResult IM(string imtoken)
        {
            try
            {
                LogsBLL.InsertAPILog("/APP/IM", Guid.Empty, "请求参数：" + imtoken);
                Users user = Session["user"] as Users;//有过滤器，直接转换用户
                if (user == null)
                {
                    return Json(new { code = "-10000" });
                }
                LogsBLL.InsertAPILog("/APP/IM", user.ID, string.Format("imtoken={0}", imtoken));
                if (string.IsNullOrEmpty(imtoken))
                {
                    LogsBLL.InsertAPILog("/APP/IM", Guid.Empty, "返回参数：code=-10000");
                    return Json(new { code = "-10000" });
                }

                IMService service;
                if (serviceList.ContainsKey(user.ID))
                {
                    service = serviceList[user.ID];
                    if (!service.onLine)
                    {
                        service.onLine = true;
                    }
                }
                else
                {//如果用户没有离线消息池，添加
                 //service = new IMService(imtoken.Value,user.ID);
                 //serviceList.Add(user.ID, service);
                 //LoginOut(user.ID);//未调用初始化init方法
                    if (CreateImtoken(user) == imtoken)
                    {
                        service = new IMService(imtoken, user.ID, user.DEVICE, user.OS);
                        serviceList.Add(user.ID, service);
                    }
                    else
                    {
                        LogsBLL.InsertAPILog("/APP/IM", Guid.Empty, "返回参数：code=-10000");
                        return Json(new { code = "-10000" });
                    }
                }
                DateTime now;
                service.ConnectTime = now = DateTime.Now;
                if (service.ID != imtoken)
                {
                    LoginOut(user.ID);
                    service.HasMessage = false;
                    LogsBLL.InsertAPILog("/APP/IM", Guid.Empty, "返回参数：code=1");
                    return Json(new { code = 1 });
                }
                int count = 0;//检查消息次数
                List<ChatMessage> responselist = new List<ChatMessage>();

                while (count < PollingCount)
                {
                    if (imtoken == service.ID && service.ConnectTime == now)
                    {
                        if (!service.HasMessage)//没有消息
                        {
                            Thread.Sleep(500);//500毫秒检查一下消息池
                            count++;
                        }
                        else
                        {
                            service.ConnectTime = DateTime.Now;
                            service.HasMessage = false;
                            LogsBLL.InsertAPILog("/APP/IM", Guid.Empty, "返回参数：code=1");
                            return Json(new { code = 1 });
                        }
                    }
                    else
                    {
                        return Json(new { code = "-10000", msg = "重新登陆" });
                    }
                }
                service.ConnectTime = DateTime.Now;
                LogsBLL.InsertAPILog("/APP/IM", Guid.Empty, "返回参数：code=0");
                return Json(new { code = 0 });
            }
            catch (Exception ex)
            {
                LogsBLL.InsertAPILog("/APP/IM", Guid.Empty, "返回参数：-10000 异常：" + ex.Message);
                return Json(new { code = "-10000" });
            }
        }
        public static void PushToAndroid(Guid UID, string DEVICE, string title, string content)
        {
            Push(UID, DEVICE, title, content, Common.enOSType.Android);
        }
        public static void PushToIOS(Guid UID, string DEVICE, string title, string content)
        {
            Push(UID, DEVICE, title, content, Common.enOSType.IOS);
        }
        public static void Push(Guid UID, string DEVICE, string title, string content, Common.enOSType type)
        {
            //IClientProfile clientProfile = DefaultProfile.GetProfile("cn-hangzhou", "LTAIow1pCeNb3iwU", "z3kEKpfd7QJFM7Q45yyz9rqBvFBJpT");
            IClientProfile clientProfile = DefaultProfile.GetProfile(SysConfigBLL.REGIONID, SysConfigBLL.AccessKeyId, SysConfigBLL.AccessKeySecret);
            DefaultAcsClient client = new DefaultAcsClient(clientProfile);
            PushRequest request = new PushRequest();
            // 推送目标
            request.AppKey = type == Common.enOSType.Android ? SysConfigBLL.ALiPushAppKey_Android : SysConfigBLL.ALiPushAppKey_IOS;
            //request.AppKey = 24740259;//appkey
            //推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送
            request.Target = "DEVICE";
            //根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备有一次最多100个的限制)
            request.TargetValue = DEVICE;
            //消息类型 MESSAGE NOTICE
            request.PushType = "NOTICE";
            //设备类型 ANDROID iOS ALL.
            request.DeviceType = "ALL";
            // 消息的标题
            request.Title = title;
            // 消息的内容
            request.Body = content;
            // 推送配置: iOS
            // iOS应用图标右上角角标
            request.IOSBadge = 1;
            // iOS通知声音
            request.IOSMusic = "default";
            //iOS10通知副标题的内容
            //request.IOSSubtitle = title;
            //指定iOS10通知Category
            request.IOSNotificationCategory = content;
            //是否允许扩展iOS通知内容
            request.IOSMutableContent = true;
            //iOS的通知是通过APNs中心来发送的，需要填写对应的环境信息。"DEV" : 表示开发环境 "PRODUCT" : 表示生产环境
            request.IOSApnsEnv = SysConfigBLL.IOSEnvironment;
            //消息推送时设备不在线（既与移动推送的服务端的长连接通道不通），则这条推送会做为通知，通过苹果的APNs通道送达一次。注意：离线消息转通知仅适用于生产环境
            request.IOSRemind = true;
            //iOS消息转通知时使用的iOS通知内容，仅当iOSApnsEnv=PRODUCT && iOSRemind为true时有效
            request.IOSRemindBody = "iOSRemindBody";
            //自定义的kv结构,开发者扩展用 针对iOS设备
            request.IOSExtParameters = "{\"code\":1}";
            // 推送配置: Android
            // 通知的提醒方式 "VIBRATE" : 震动 "SOUND" : 声音 "BOTH" : 声音和震动 NONE : 静音
            request.AndroidNotifyType = "BOTH";
            //通知栏自定义样式0-100
            request.AndroidNotificationBarType = 3;
            //通知栏显示优先级
            request.AndroidNotificationBarPriority = 0;
            //点击通知后动作 "APPLICATION" : 打开应用 "ACTIVITY" : 打开AndroidActivity "URL" : 打开URL "NONE" : 无跳转
            request.AndroidOpenType = "ACTIVITY";
            //Android收到推送后打开对应的url,仅当AndroidOpenType="URL"有效
            request.AndroidOpenUrl = "http://www.baidu.com";
            //设定通知打开的activity，仅当AndroidOpenType="Activity"有效
            request.AndroidActivity = "com.juns.wechat.MainActivity";
            //Android通知音乐
            request.AndroidMusic = "default";
            //设置该参数后启动辅助弹窗功能，此处指定通知点击后跳转的Activity（辅助弹窗的前提条件：1. 集成第三方辅助通道；2. storeOffline设为true)
            request.AndroidPopupActivity = "com.ali.demo.PopupActivity";
            //辅助弹窗标题
            request.AndroidPopupTitle = title;
            //辅助弹窗内容
            request.AndroidPopupBody = content;
            // 设定android类型设备通知的扩展属性
            request.AndroidExtParameters = "{\"code\":1}";
            // 推送控制
            //String pushTime = DateTime.UtcNow.AddSeconds(3).ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
            //request.PushTime = pushTime;//延迟3秒发送
            String expireTime = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
            request.ExpireTime = expireTime;//设置过期时间为2天
            request.StoreOffline = false;
            try
            {
                PushResponse response = client.GetAcsResponse(request);
            }
            catch (ServerException e)
            {
                LogsBLL.InsertAPILog("/APP/Push", UID, string.Format("ErrorCode:{0},ErrorMessage:{1}", e.ErrorCode, e.ErrorMessage));
            }
            catch (ClientException e)
            {
                LogsBLL.InsertAPILog("/APP/Push", UID, string.Format("ErrorCode:{0},ErrorMessage:{1}", e.ErrorCode, e.ErrorMessage));
            }
        }
    }
}