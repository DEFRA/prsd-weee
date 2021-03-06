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
    
    public partial class RegisteredProducer
    {
        public RegisteredProducer()
        {
            this.ProducerSubmissions = new HashSet<ProducerSubmission>();
            this.EeeOutputAmounts = new HashSet<EeeOutputAmount>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] RowVersion { get; set; }
        public string ProducerRegistrationNumber { get; set; }
        public System.Guid SchemeId { get; set; }
        public int ComplianceYear { get; set; }
        public Nullable<System.Guid> CurrentSubmissionId { get; set; }
        public bool Removed { get; set; }
        public Nullable<System.DateTime> RemovedDate { get; set; }
    
        public virtual ICollection<ProducerSubmission> ProducerSubmissions { get; set; }
        public virtual ProducerSubmission CurrentSubmission { get; set; }
        public virtual ICollection<EeeOutputAmount> EeeOutputAmounts { get; set; }
        public virtual Scheme Scheme { get; set; }
    }
}
