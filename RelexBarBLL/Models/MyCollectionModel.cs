using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class MyCollectionModel: MyCollection
    {
        public ChatMessage message { get; set; }
    }
}
