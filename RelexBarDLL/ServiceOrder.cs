//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RelexBarDLL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ServiceOrder
    {
        public System.Guid ID { get; set; }
        public System.Guid ServiceID { get; set; }
        public System.Guid UID { get; set; }
        public string OrderNumber { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public decimal Price { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
        public string Return_url { get; set; }
        public string Notify_url { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime UpdateTime { get; set; }
    }
}
