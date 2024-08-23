namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Audit;
    using EA.Weee.Domain.Organisation;

    public class DirectProducerSubmission : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid DirectRegistrantId { get; set; }
        public Guid? ServiceOfNoticeAddressId { get; set; }
        public Guid? AppropriateSignatoryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedById { get; set; }
        public string UpdatedById { get; set; }
        public int? PaymentStatus { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string PaymentReference { get; set; }
        public string PaymentId { get; set; }

        public virtual DirectRegistrant DirectRegistrant { get; set; }
        public virtual Address ServiceOfNoticeAddress { get; set; }
        public virtual Contact AppropriateSignatory { get; set; }
        public virtual User.User CreatedBy { get; set; }
        public virtual User.User UpdatedBy { get; set; }
    }
}
