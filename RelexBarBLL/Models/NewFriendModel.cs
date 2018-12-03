using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class NewFriendModel
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string HeadImg { get; set; }
        public Guid MID { get; set; }
        public string Content { get; set; }
        public int HadRead { get; set; }
        public string Phone { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
