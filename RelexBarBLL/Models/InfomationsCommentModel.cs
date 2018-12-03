using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class InfomationsCommentModel
    {
        public System.Guid UID { get; set; }

        public string Name { get; set; }
        public string TrueName { get; set; }
        public string Phone { get; set; }

        public string HeadImg1 { get; set; }

        public string Content { get; set; }
        public System.DateTime UpdateTime { get; set; }
    }
}
