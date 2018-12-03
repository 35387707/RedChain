using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    public partial class CartBLL : BaseBll
    {
        public List<Cart> List(Guid Uid)
        {
            using (DBContext)
            {
                ////EF 执行SQL语句
                //DBContext.Database.ExecuteSqlCommand("");
                return DBContext.Cart.Where(m => m.UID == Uid).ToList();
            }
        }
    }
}
