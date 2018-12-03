using RelaxBarWeb_MVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace RelaxBarWeb_MVC.Models
{
    public class OfflineMessage
    {
        private Guid ID;
        private Timer timer = new Timer();
        public DateTime ConnectTime { get; set; }
        /// <summary>
        /// 用户是否在线，设置为false之后
        /// </summary>
        private bool online;

        public bool Online
        {
            get
            {
                return online;
            }

            set
            {
                
                online = value;
            }
        }
        public Guid GetID() {
            return ID;
        }
        public void SetID(Guid ID) {
            this.ID = ID;
        }
        /// <summary>
        /// 离线消息链表
        /// </summary>
        public MyLinkedList<RelexBarDLL.ChatMessage> MessageList = new MyLinkedList<RelexBarDLL.ChatMessage>();
        public OfflineMessage(Guid ID) {
            this.ID = ID;
            this.online = true;
            timer.Interval = 1000 * 60 * 10;//十分钟
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!online) {
                RelaxBarWeb_MVC.Controllers.AjaxIMController.OfflineMessage.Remove(ID);
                timer.Stop();
            }
        }
    }
}