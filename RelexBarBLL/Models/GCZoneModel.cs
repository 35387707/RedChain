using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class GCZoneModel
    {
        public System.Guid ID { get; set; }
        public System.Guid UID { get; set; }
        public string UserName { get; set; }
        public string HeadImg { get; set; }
        public string Content { get; set; }
        public int IsDel { get; set; }
        public int Status { get; set; }
        public string Location { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime UpdateTime { get; set; }

        public virtual ICollection<GCZoneCommentModel> GCZoneComment { get; set; }
        public virtual ICollection<GCZoneLikeModel> IsLikeList { get; set; }

    }
}
