namespace EA.Weee.Domain.Producer
{
    using EA.Weee.Domain.Audit;
    using EA.Weee.Domain.Organisation;
    using System;

    public class DirectProducerSubmission : AuditableEntity
    {
        public virtual Guid DirectRegistrantId { get; set; }
        public virtual Guid? ServiceOfNoticeAddressId { get; set; }
        public virtual Guid? AppropriateSignatoryId { get; set; }
        public virtual int? PaymentStatus { get; set; }
        public virtual DateTime? SubmittedDate { get; set; }
        public virtual string PaymentReference { get; set; }
        public virtual string PaymentId { get; set; }

        public virtual DirectRegistrant DirectRegistrant { get; set; }
        public virtual Address ServiceOfNoticeAddress { get; set; }
        public virtual Contact AppropriateSignatory { get; set; }
    }
}
