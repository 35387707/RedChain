using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class SLGameRecordModel
    {
        public Guid GID { get; set; }
        public Guid UID { get; set; }
        public string Nick { get; set; }
        public string HeadImg { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public string SName { get; set; }//发送者名称
    }
}
