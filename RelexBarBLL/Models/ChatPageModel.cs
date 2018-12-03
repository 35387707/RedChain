using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class ChatPageModel
    {
        public Guid UID { get; set; }
        public string Name { get; set; }
        public string lastMessage { get; set; }
        public DateTime Time { get; set; }
    }
}
