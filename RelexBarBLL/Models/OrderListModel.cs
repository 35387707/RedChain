using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
   public class OrderListModel
    {
        public System.Guid ID { get; set; }
        public Guid? UID { get; set; }
        public string Name { get; set; }

        public string TrueName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
     //   public Guid? OUID { get; set; }
        public string RecName { get; set; }
        
        public string RecAddress { get; set; }
        public string RecPhone { get; set; }
        public string Number { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }

        public DateTime? PayTime { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? RecTime { get; set; }

        public DateTime? FinishTime { get; set; }
        public DateTime? CreateTime { get; set; }

        public int OCount { get; set; }

        public decimal OPrice { get; set; }
        public string OName { get; set; }
   
        public string ProductSpecificationStr { get; set; }
    }
}
