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
    
    public partial class ChatMessage
    {
        public System.Guid MID { get; set; }
        public System.Guid FUID { get; set; }
        public System.Guid TUID { get; set; }
        public int TType { get; set; }
        public int MType { get; set; }
        public string Content { get; set; }
        public int HadRead { get; set; }
        public int IsDel { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
    }
}
