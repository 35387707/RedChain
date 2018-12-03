using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class BillModel
    {
        public string Remark { get; set; }
        public decimal Val { get; set; }
        public int InOut { get; set; }
        public int FromTo { get; set; }
        public DateTime Createtime { get; set; }
        public string headimg { get; set; }
        public string PayName { get; set; }//交易方名字
    }
}
