using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RelexBarBLL.EnumCommon
{
    public class EnumCommon
    {
    }
    public enum MessageTType {
        /// <summary>
        /// 用户消息
        /// </summary>
        User=1,
        /// <summary>
        /// 群组消息
        /// </summary>
        Group=2,
        /// <summary>
        /// 系统消息
        /// </summary>
        System=3,
        /// <summary>
        /// 关注号消息
        /// </summary>
        Gzh=4,
    }
    public enum MessageType
    {
        /// <summary>
        /// 被挤下线提醒
        /// </summary>
        LoginOut=-3,
        /// <summary>
        /// 红包信息提醒
        /// </summary>
        RedMsg=-2,
        /// <summary>
        /// 撤销
        /// </summary>
        Revoke=-1,
        /// <summary>
        /// 文字
        /// </summary>
        Text = 0,
        /// <summary>
        /// 照片
        /// </summary>
        Image = 1,
        /// <summary>
        /// 红包
        /// </summary>
        Red = 2,
        /// <summary>
        /// 转账
        /// </summary>
        Transfer = 3,
        /// <summary>
        /// 个人名片
        /// </summary>
        MyCard = 4,
        /// <summary>
        /// 心跳包
        /// </summary>
        HeartBeat = 5,
        /// <summary>
        /// 连接
        /// </summary>
        Link=6,
        /// <summary>
        /// 视频
        /// </summary>
        Video=7,
        /// <summary>
        /// 语音
        /// </summary>
        Voice=8,
        /// <summary>
        /// 扫雷红包
        /// </summary>
        SLRed=9,
        /// <summary>
        /// 好友申请
        /// </summary>
        FriendRequest=10,
        /// <summary>
        /// 好友申请通知
        /// </summary>
        FriendRequestNotice = 11,
        /// <summary>
        /// 删除群
        /// </summary>
         DropGroup=12,
         /// <summary>
         /// 添加到群组通知
         /// </summary>
         JoinGroupNotice=13,
         /// <summary>
         /// 用户退出群组通知
         /// </summary>
         ExitGroupNotice=14,
         /// <summary>
         /// 用户被移出群
         /// </summary>
         RemoveGroupUser=15,
         /// <summary>
         /// 收款成功
         /// </summary>
         ShouKuanOK=16,
         /// <summary>
         /// 删除好友通知
         /// </summary>
         DeleteFriend=17,
         /// <summary>
         /// 转账成功通知
         /// </summary>
         TransferSuccess=18,
        /// <summary>
        /// 打招呼消息(发红包)
        /// </summary>
        SayHello = 19,
    }
    public enum MessageStatus
    {
        /// <summary>
        /// 错误
        /// </summary>
        Error = -1,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 未在线
        /// </summary>
        NotOnline = -2
    }
    public enum GType {
        /// <summary>
        /// 聊天群
        /// </summary>
        ChatGroup=1,
        /// <summary>
        /// 游戏群
        /// </summary>
        GameGroup=2,

    }
}