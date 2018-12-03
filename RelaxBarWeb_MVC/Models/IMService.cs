using RelaxBarWeb_MVC.Controllers;
using RelexBarBLL;
using RelexBarBLL.EnumCommon;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RelaxBarWeb_MVC.Models
{
    public class IMService
    {
        //public Guid ID { get; set; }
        public string ID { get; set; }//此为用户轮询token
        public Guid UID { get; set; }
        public DateTime ConnectTime { get; set; }//最后轮询连接时间
        public bool HasMessage { get; set; }
        public string DEVICE { get; set; }//设备号
        public bool onLine { get; set; }
        private object sync = new object();
        public Common.enOSType? osType { get; set; }
        //private List<PushModel> _pushmsg = new List<PushModel>();
        public IMService(string ID, Guid UID)
        {
            this.ID = ID;
            this.UID = UID;
            ConnectTime = DateTime.Now;
            HasMessage = false;
            onLine = true;
        }
        public IMService(string ID, Guid UID, string DEVICE, int? osType)
        {
            this.ID = ID;
            this.UID = UID;
            this.DEVICE = DEVICE;
            ConnectTime = DateTime.Now;
            HasMessage = false;
            onLine = true;
            switch (osType)
            {
                case 0:
                    this.osType = Common.enOSType.Android;
                    break;
                case 1:
                    this.osType = Common.enOSType.IOS;
                    break;
                default:
                    break;
            }
        }
        public void AddPushMsg(PushModel pushModel)
        {
            if (pushModel != null)
                lock (sync)
                {
                    DateTime currenTime = DateTime.Now.AddSeconds(-5);
                    if (!string.IsNullOrEmpty(this.DEVICE) && currenTime > this.ConnectTime)
                    {
                        //this.HasMessage = false;
                        switch (this.osType)
                        {
                            case Common.enOSType.Android:
                                APPController.PushToAndroid(this.UID, this.DEVICE, pushModel.Title, pushModel.Content);
                                break;
                            case Common.enOSType.IOS:
                                APPController.PushToIOS(this.UID, this.DEVICE, pushModel.Title, pushModel.Content);
                                break;
                            default:
                                break;
                        }
                    }
                }
        }
    }
}