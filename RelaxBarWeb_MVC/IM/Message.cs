using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RelexBarBLL.EnumCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RelaxBarWeb_MVC.IM
{
    public class Message
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="api">接口名称</param>
        /// <param name="rid">接收者id</param>
        /// <param name="type">消息类型0文字 1照片 2红包 3转账 4个人名片</param>
        /// <param name="status">状态</param>
        /// <param name="time">时间</param>
        /// <param name="content">内容</param>
        public Message(string api, Guid sid, Guid rid,MessageTType ttype, MessageType type,MessageStatus status, DateTime time, string content, Guid? gid =null)
        {
            this.Api = api;
            this.Sid = sid;
            this.Rid = rid;
            this.Gid = gid;
            this.TType = (int)ttype;
            this.Type = (int)type;
            this.Status = (int)status;
            this.Time = time;
            this.Content = content;
        }
        public string Api { get; set; }//接口名
        public Guid Sid { get; set; }//发送者
        public int TType { get; set; }//1用户，2群，3系统消息，4关注号消息
        public Guid Rid { get; set; }//接收者用户id
        public Guid? Gid { get; set; }//群id
        public Guid? Mid { get; set; }//当前消息在数据库中的id
        public int Type { get; set; }//消息类型 0文字/表情，1图片，6链接，4个人名片，5心跳包，7视频,8语音,2普通红包，9扫雷红包，3转账,10好友申请,9其他等
        public int Status { get; set; }//状态1成功
        public DateTime? Time { get; set; }
        public string Content { get; set; }
        public ArraySegment<byte> toSendMSG() {
            var iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string msg = JsonConvert.SerializeObject(this, iso);
            return new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
        }
    }
    

}