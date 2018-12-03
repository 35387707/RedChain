using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class SearchUser
    {
        public Guid ID { get; set; }
        public string CardNumber { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int? Sex { get; set; }
        public string HeadImg { get; set; }
        public string Email { get; set; }
        public int IsFriend { get; set; }
    }
}
