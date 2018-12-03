using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class GCZoneCommentModel
    {
        public System.Guid ID { get; set; }
        public System.Guid GCZID { get; set; }
        public Nullable<System.Guid> ReplyID { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public System.Guid UID { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
    }
}
