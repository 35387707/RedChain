using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class InfomationsModel
    {
        public string Name { get; set; }

        public string HeadImg { get; set; }
        public string TrueName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
        public string CardNumber { get; set; }

        public int RealCheck { get; set; }

        public System.Guid IID { get; set; }
        public System.Guid UID { get; set; }
        public int Type { get; set; }
        public string title { get; set; }
        public string imglist { get; set; }
        public string LinkTo { get; set; }
        public int ViewCount { get; set; }
        public int GoodCount { get; set; }
        public Nullable<decimal> AreaX { get; set; }
        public Nullable<decimal> AreaY { get; set; }
        public Nullable<int> AreaLimit { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> BeginTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
    }
}
