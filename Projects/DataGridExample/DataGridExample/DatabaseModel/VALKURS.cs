//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataGridExample.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class VALKURS
    {
        public string DIG_CODE { get; set; }
        public System.DateTime BYDATE { get; set; }
        public Nullable<decimal> FIXING { get; set; }
        public Nullable<decimal> CASH_SALE { get; set; }
        public Nullable<decimal> CASH_BUY { get; set; }
        public Nullable<decimal> ACC_SALE { get; set; }
        public Nullable<decimal> ACC_BUY { get; set; }
    
        public virtual VAL VAL { get; set; }
    }
}