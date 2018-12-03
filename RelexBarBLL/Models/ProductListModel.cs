using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class ProductListModel
    {
        public Nullable<System.Guid> ID { get; set; }
        public Nullable<System.Guid> ShopID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Img { get; set; }
        public string ImgList { get; set; }
        public string Descrition { get; set; }
        public decimal RealPrice { get; set; }
        public int PriceType { get; set; }
        public decimal Price { get; set; }
        public decimal FootQuanPrice { get; set; }
        public decimal Stock { get; set; }
        public int OrderID { get; set; }
        public int Type { get; set; }
        public Nullable<System.DateTime> BeginTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public int CompleteCount { get; set; }
        public int GoodCount { get; set; }
        public int ViewCount { get; set; }
        public int CPoints { get; set; }

        public string ShopName { get; set; }
        public string ShopTrueName { get; set; }

        public int ShopStatus { get; set; }
        public string ShopAddress { get; set; }
        public string ShopPhone { get; set; }

        public string CategoryName { get; set; }

        public Nullable<int> CategoryShow { get; set; }

    }
}
