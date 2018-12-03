using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class AdminPayListModel
    {
        public Guid? UID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid? OUID { get; set; }
        public string OPhone { get; set; }
        public string OName { get; set; }

        public string HeadImg { get; set; }

        public string CardNumber { get; set; }

        public int InOut { get; set; }
        public int FromTo { get; set; }
        public decimal Val { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
        public int PayType { get; set; }
    }
}
