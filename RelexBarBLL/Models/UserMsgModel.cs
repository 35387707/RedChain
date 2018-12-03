using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class UserMsgModel
    {
        public string UName { get; set; }
        public string FName { get; set; }

        public  System.Guid ID { get; set; }
        public Nullable<System.Guid> UID { get; set; }
        public Nullable<System.Guid> FromUid { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int MessageType { get; set; }
        public string Img { get; set; }
        public string Title { get; set; }
        public int IsShow { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }

       
    }
}
