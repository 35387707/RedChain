using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace RelexBarBLL.Services
{
    /// <summary>
    /// 环信聊天接口
    /// </summary>
    public class HuanXinIM
    {
        #region Fields

        string CreateUser_URL = "https://im.fbddd.com/reg_user";
        string DeleteUser_URL = "https://im.fbddd.com/del_user";
        string DisconnectUser_URL = "https://im.fbddd.com/disconnect";
        string AddFriend_URL = "https://im.fbddd.com/add_friend";
        string DeleteFriend_URL = "https://im.fbddd.com/del_friend";
        string SendMsg_URL = "https://im.fbddd.com/send_message_text";

        #endregion

        #region Function

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="nickName"></param>
        public void CreateUser_Huanxin(Guid UserId, string nickName)
        {
            PostMsgAsync(CreateUser_URL, "user_name=" + UserId.ToString("N") + "&nick_name=" + nickName);
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="nickName"></param>
        public void AddFriend_Huanxin(Guid MainUserID, Guid SecUserID)
        {
            PostMsgAsync(AddFriend_URL, "owner_username=" + MainUserID.ToString("N") + "&friend_username=" + SecUserID.ToString("N"));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="recUID"></param>
        /// <param name="msg"></param>
        /// <param name="ext"></param>
        public void SendMsg_Huanxin(Guid recUID, string msg, string ext)
        {
            PostMsgAsync(SendMsg_URL, "users=" + recUID.ToString("N") + "&message=" + msg + "&send_user=" + recUID.ToString("N") + "&ext=" + ext);
        }

        private string PostMsgAsync(string url, string data)
        {
            try
            {
                LogsBLL.InsertAPILog("PostMsgAsync", Guid.Empty, "url：" + url + ",data：" + data);

                Task.Run(() =>
                {
                    WebClient wb = new WebClient();
                    //wb.DownloadString(new Uri("http://192.168.0.2:10000/pay/PayTest"));//异步发送，不影响本系统相关操作
                    //var result = wb.UploadString(new Uri(url), "POST", data);//异步发送，不影响本系统相关操作
                    wb.DownloadString(url + "?" + data);//异步发送，不影响本系统相关操作
                });
                //return result;
            }
            catch (Exception ex)
            {
                LogsBLL.InsertAPILog("PostMsgAsyncError", Guid.Empty, "url：" + url + ",data：" + data + ",ex:" + ex);
            }
            //return "";
            return string.Empty;
        }

        #endregion

    }
}
