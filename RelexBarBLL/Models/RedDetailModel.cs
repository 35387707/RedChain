using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class RedDetailModel
    {
        public Guid MID { get; set; }
        public int MType { get; set; }
        public Guid RID { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string HeadImg { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
