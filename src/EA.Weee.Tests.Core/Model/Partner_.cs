//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EA.Weee.Tests.Core.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Partner_
    {
        public System.Guid Id { get; set; }
        public byte[] RowVersion { get; set; }
        public string Name { get; set; }
        public System.Guid PartnershipId { get; set; }
    
        public virtual Partnership Partnership { get; set; }
    }
}
