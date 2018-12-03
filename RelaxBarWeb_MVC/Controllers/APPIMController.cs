using RelaxBarWeb_MVC.Models;
using RelexBarBLL;
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
    public class APPIMController : BaseController
    {
        private static int PollingCount = SysConfigBLL.PollingMaxTime * 2;
        private static object sync = new object();
        private static Task task;
        public APPIMController() : base()
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
        }
        public void TaskHandle()
        {
            while (true)
            {
                Thread.Sleep(1000 * 60 * 60);
                DateTime time = DateTime.Now.AddDays(-5);
                List<Guid> rmKey = new List<Guid>();
                foreach (var item in CONNECT_POOL)
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
                        CONNECT_POOL.Remove(rmKey[i]);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public static Dictionary<Guid, IMService> CONNECT_POOL = new Dictionary<Guid, IMService>();
        public static IMService Get(Guid key)
        {
            if (CONNECT_POOL.ContainsKey(key))
            {
                return CONNECT_POOL[key];
            }
            return null;
        }

        public static IMService Login(Guid id, string token, bool firstLogin)
        {
            IMService im = Get(id);
            if (im == null)
            {
                im = new IMService(token, id);
                CONNECT_POOL.Add(id, im);//不存在，添加
            }
            else
            {
                //强行踢人下线
                if (firstLogin)
                {
                    im.ID = token;
                }
                else
                {
                    if (im.ID != token)//如果token相等则登陆，否则踢出去
                    {
                        im = null;
                    }
                }
            }
            return im;
        }

        /// <summary>
        /// 开始连接IM(轮询时间60秒)
        /// </summary>
        /// <returns></returns>
        public void Connet()
        {
            Guid id = UserInfo.ID;
            var im = Login(id, Request.Cookies["token"].Value.ToString(), false);
            if (im == null)
            {
                UserInfo = null;
                if (Response.Cookies["token"] != null)
                {
                    Response.Cookies["token"].Expires = DateTime.Now.AddDays(-1);
                }
                Response.Write("{\"code\":\"" + (int)Common.ErrorCode.重新登陆 + "\",\"msg\":\"您的账户已在别处登陆，请重新登陆\"}");
                return;
            }

            LogsBLL.InsertAPILog("/APPIMController/Connet", id, string.Format("conneting......imtoken={0}", im.ID));
            im.ConnectTime = DateTime.Now;

            int result = 0;
            var sleepcount = 2;
            while (sleepcount < PollingCount)
            {
                if (im.HasMessage)//如果有最新消息，立即返回
                {
                    result = 1;
                    im.HasMessage = false;
                    break;
                }
                Thread.Sleep(500);//暂停500毫秒
                sleepcount++;
            }

            Response.Write(result);
        }

        //private EventHandler<object> test;
        //private async Task<int> StandByNewMsg(IMService im)
        //{
        //    if(1)
        //    test.BeginInvoke(im);
        //    await be
        //    return 1;
        //}

        public static void Logout(Guid ID)
        {
            try
            {
                CONNECT_POOL.Remove(ID);
            }
            catch
            { }
        }
    }
}