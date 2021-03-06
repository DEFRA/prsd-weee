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
    
    public partial class Return
    {
        public Return()
        {
            this.NonObligatedWeees = new HashSet<NonObligatedWeee>();
            this.Return1 = new HashSet<Return>();
            this.WeeeReceiveds = new HashSet<WeeeReceived>();
            this.ReturnReportOns = new HashSet<ReturnReportOn>();
            this.ReturnSchemes = new HashSet<ReturnScheme>();
            this.WeeeReuseds = new HashSet<WeeeReused>();
            this.WeeeSentOns = new HashSet<WeeeSentOn>();
        }
    
        public System.Guid Id { get; set; }
        public int ComplianceYear { get; set; }
        public int Quarter { get; set; }
        public int ReturnStatus { get; set; }
        public byte[] RowVersion { get; set; }
        public string CreatedById { get; set; }
        public string SubmittedById { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public System.Guid OrganisationId { get; set; }
    
        public virtual ICollection<NonObligatedWeee> NonObligatedWeees { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<Return> Return1 { get; set; }
        public virtual Return Return2 { get; set; }
        public virtual ICollection<WeeeReceived> WeeeReceiveds { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual ICollection<ReturnReportOn> ReturnReportOns { get; set; }
        public virtual ICollection<ReturnScheme> ReturnSchemes { get; set; }
        public virtual ICollection<WeeeReused> WeeeReuseds { get; set; }
        public virtual ICollection<WeeeSentOn> WeeeSentOns { get; set; }
    }
}
