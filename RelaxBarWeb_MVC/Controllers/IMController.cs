using Newtonsoft.Json;

using RelaxBarWeb_MVC.IM;
using RelaxBarWeb_MVC.Utils;
using RelexBarBLL;
using RelexBarBLL.EnumCommon;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace RelaxBarWeb_MVC.Controllers
{
    public class IMController : BaseController
    {
        private static Dictionary<Guid, WebSocket> CONNECT_POOL = new Dictionary<Guid, WebSocket>();//用户连接池
        public static WebSocket Get(Guid key)
        {
            if (CONNECT_POOL.ContainsKey(key))
            {
                return CONNECT_POOL[key];
            }
            return null;
        }

        public void WSConnet()
        {
            //检查 查询是否是WebSocket请求
            if (HttpContext.IsWebSocketRequest)
            {//如果是，我们附加异步处理程序
                HttpContext.AcceptWebSocketRequest(ProcessWSChat);
            }
        }
        private async Task ProcessWSChat(AspNetWebSocketContext content)
        {
            if (string.IsNullOrEmpty(content.QueryString["id"]))
                return;
            Guid id;
            if (!Guid.TryParse(content.QueryString["id"].ToUpper(), out id))
            {
                return;
            }
            var socket = content.WebSocket;
            try
            {
                //string token =  content.Cookies["token"].Value;
                string token = "test";

                //链接用户不存在连接池
                if (Get(id) == null)
                {
                    CONNECT_POOL.Add(id, socket);//不存在，添加
                    await socket.SendAsync(new Message("login", Guid.Empty, id, MessageTType.System, MessageType.Text, MessageStatus.Success, DateTime.Now, "连接成功").toSendMSG(), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    //通知当前在线方，让他下线
                    try
                    {
                        var s = Get(id);
                        if (s.State == WebSocketState.Open)
                        {
                            s.SendAsync(new Message("logout", Guid.Empty, id, MessageTType.System, MessageType.LoginOut, MessageStatus.Error, DateTime.Now, "您的账户已在其他地方登陆，请重新登陆").toSendMSG(), WebSocketMessageType.Text, true, CancellationToken.None);
                            //s.Abort();
                            //s.Dispose();
                        }
                    }
                    catch
                    {

                    }
                    await socket.SendAsync(new Message("login", Guid.Empty, id, MessageTType.System, MessageType.Text, MessageStatus.Success, DateTime.Now, "连接成功").toSendMSG(), WebSocketMessageType.Text, true, CancellationToken.None);

                    CONNECT_POOL[id] = socket;
                }
                while (socket.State == WebSocketState.Open)
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[10240]);

                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);//接收消息
                                                                                                              //#region 关闭Socket处理，删除连接池
                                                                                                              //if (socket.State != WebSocketState.Open)//连接关闭
                                                                                                              //{
                                                                                                              //    DisposeWS();
                                                                                                              //    return;
                                                                                                              //}
                                                                                                              //#endregion

                    string userMsg = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);//发送过来的消息
                    Message message = JsonConvert.DeserializeObject<Message>(userMsg);
                    if (message != null)
                    {
                        //对消息进行处理
                        message.Sid = id;
                        DoMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                DisposeWS(id);
            }
        }
        private void DoMessage(Message message)
        {
            message.Mid = Guid.NewGuid();//消息在服务器中的ID

            switch ((MessageType)message.Type)
            {
                case MessageType.Text://发送消息
                case MessageType.Image://发送消息
                    sendToUser(message);
                    break;
                case MessageType.Red://普通红包消息
                case MessageType.SayHello://打招呼红包消息
                    SendRedPacket(message);
                    break;
                case MessageType.FriendRequest:
                    SendFriendRequest(message);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 发送聊天红包消息
        /// </summary>
        /// <param name="message"></param>
        public void SendRedPacket(Message message)
        {
            //聊天红包格式  金额||内容
            if (string.IsNullOrEmpty(message.Content))
                return;
            try
            {
                FriendBLL bll = new FriendBLL();
                decimal price;
                var c = CommonClass.ChangeData.ExchangeDataType(message.Content);
                if (!decimal.TryParse(c["price"].ToString(), out price))
                {
                    return;
                }
                RedPacksBLL rpb = new RedPacksBLL();
                int result = rpb.SendRed(message.Sid, message.Mid.Value, price);
                if (result < 0)
                {
                    var m = new Message("sendToUser", Guid.Empty, message.Sid, MessageTType.System, MessageType.Red, MessageStatus.Success, DateTime.Now, "打招呼失败：" + ((Common.ErrorCode)result).ToString());
                    sendToUser(m);
                }
                else
                {
                    if (bll.isFriend(message.Sid, message.Rid))//如果已经是好友，则直接发送到对方
                    {
                        message.Type = (int)MessageType.Red;//发红包
                        sendToUser(message);
                    }
                    else
                    {
                        //发送打招呼红包
                        message.Type = (int)MessageType.SayHello;//打招呼
                        sendToUser(message);
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 发送好友请求
        /// </summary>
        /// <param name="message"></param>
        public void SendFriendRequest(Message message)
        {
            FriendBLL bll = new FriendBLL();
            Message m = null;
            if (bll.isFriend(message.Sid, message.Rid))
            {
                m = new Message("sendToUser", Guid.Empty, message.Sid, MessageTType.System, MessageType.FriendRequest, MessageStatus.Success, DateTime.Now, "你们已经是好友了");
            }
            if (!bll.CanAddFriend(message.Sid))
            {
                m = new Message("sendToUser", Guid.Empty, message.Sid, MessageTType.System, MessageType.FriendRequest, MessageStatus.Success, DateTime.Now, "已经达到好友上限");
            }
            if (bll.haveFriendRequest(message.Sid, message.Rid))
            {
                m = new Message("sendToUser", Guid.Empty, message.Sid, MessageTType.System, MessageType.FriendRequest, MessageStatus.Success, DateTime.Now, "已经发送过好友请求");
            }
            else
            {
                m = new Message("sendToUser", message.Sid, message.Rid, MessageTType.System, MessageType.FriendRequest, MessageStatus.Success, DateTime.Now, message.Content);
            }
            sendToUser(m);
        }

        /// <summary>
        /// 发送消息给别人
        /// </summary>
        /// <param name="message"></param>
        public static void sendToUser(Message message)
        {
            try
            {
                if (CONNECT_POOL.ContainsKey(message.Rid))
                {
                    message.Status = 1;
                    sendMessage(message);
                }
                else
                {
                    if (message.Type == (int)MessageType.FriendRequest || message.Type == (int)MessageType.SayHello)//如果是好友申请/打招呼，不需要他在线
                    {
                        message.Status = 0;
                        sendMessage(message, false);
                    }
                    else
                    {
                        Message m = new Message("sendToUser", Guid.Empty, message.Sid, MessageTType.System, MessageType.Text, MessageStatus.NotOnline, DateTime.Now, "对方未在线");
                        sendMessage(m);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 记录消息
        /// </summary>
        /// <param name="message"></param>
        private static void sendMessage(Message message, bool send = true)
        {
            if (message.Rid == Guid.Empty)//如果接收人是系统，拿就不用处理了！
                return;
            ChatMessage msg = new ChatMessage();
            msg.MID = message.Mid.HasValue ? message.Mid.Value : Guid.NewGuid();
            msg.FUID = message.Sid;
            msg.TUID = message.Rid;
            msg.TType = message.TType;
            msg.MType = message.Type;
            msg.Content = encodeHtml(message.Content);
            msg.HadRead = message.Sid == Guid.Empty ? (int)Common.enStatus.Enabled : (int)Common.enStatus.Unabled;//如果是系统消息，则自动默认该消息为已读
            msg.IsDel = 0;
            msg.Status = message.Status;
            msg.UpdateTime = msg.CreateTime = DateTime.Now;
            ChatsBLL bll = new ChatsBLL();
            int i = bll.AddChatMessage(msg);
            if (send)
            {
                var s = Get(message.Rid);
                if (s != null && s.State == WebSocketState.Open)
                    s.SendAsync(message.toSendMSG(), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public static void DisposeWS(Guid ID)
        {
            try
            {
                var socket = Get(ID);
                CONNECT_POOL.Remove(ID);
                if (socket != null)
                {
                    socket.Abort();
                    socket = null;
                }
            }
            catch
            { }
        }
    }
}