using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class ShowReceiverModel
    {
        public Guid ID { get; set; }
        public Guid MUID { get; set; }
        public string Remark { get; set; }
        public string Phone { get; set; }
        public string headimg { get; set; }
        public string lastContent { get; set; }
        public DateTime? lastTime { get; set; }
        public int Type { get; set; }//1用户，2群
        public int Gtype { get; set; }//1为群聊，2为游戏群
        public int? MType { get; set; }//--聊天类型，0文字/表情，1图片，6链接，4个人名片，5心跳包，7视频,8语音,2普通红包，9扫雷红包，3转账,10好友申请,9其他等
        public int IsTop { get; set; }//1置顶
        public int WDCount { get; set; }//未读数量
        public string Notice { get; set; }

        public List<APPChatGroupUser> Users { get; set; }
    }
}
