//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace e3Radio.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrackLike
    {
        public int TrackID { get; set; }
        public long UserID { get; set; }
        public bool IsLike { get; set; }
        public System.DateTime DateLiked { get; set; }
    
        public virtual Track Track { get; set; }
        public virtual User User { get; set; }
    }
}
