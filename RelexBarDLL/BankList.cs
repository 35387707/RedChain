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
    
    public partial class BankList
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> UID { get; set; }
        public string BankName { get; set; }
        public string BankZhiHang { get; set; }
        public string BankAccount { get; set; }
        public string BankUser { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public bool isDefault { get; set; }
    }
}
