using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class APPFriendModel
    {
        public Guid ID { get; set; }
        public Guid UID { get; set; }
        public Guid FriendID { get; set; }
        public string Remark { get; set; }
        public Guid? LastMID { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public Guid? LastReadID { get; set; }
        public string HeadImg { get; set; }
        public int IsTop { get; set; }
        public int Ignore { get; set; }
        public string CardNumber { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int? Sex { get; set; }
        public DateTime LastLoginTime { get; set; }
    }
}
