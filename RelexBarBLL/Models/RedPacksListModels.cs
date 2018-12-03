using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
   public class RedPacksListModels
    {

        public Nullable<System.Guid> UID { get; set; }
        //发布红包者
     //   public string FName { get; set; }
        //抢红包者
        public string Name { get; set; }

        public string HeadImg { get; set; }
        public string TrueName { get; set; }
        public string Phone { get; set; }

        public string CardNumber { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
        public int RealCheck { get; set; }


        public string title { get; set; }

        public int RedType { get; set; }

        public System.Guid RLID { get; set; }
        public Nullable<System.Guid> RID { get; set; }
        public decimal Money { get; set; }
        public int C_index { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> BeginTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string Number { get; set; }
    }
}
