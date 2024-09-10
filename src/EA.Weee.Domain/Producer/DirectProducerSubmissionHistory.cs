namespace EA.Weee.Domain.Producer
{
    using EA.Weee.Domain.Audit;
    using EA.Weee.Domain.Organisation;
    using System;
    using EA.Weee.Domain.DataReturns;

    public class DirectProducerSubmissionHistory : AuditableEntity
    {
        public virtual Guid DirectProducerSubmissionId { get; set; }

        public virtual Guid? ServiceOfNoticeAddressId { get; set; }

        public virtual Guid? AppropriateSignatoryId { get; set; }

        public virtual Guid? EeeOutputReturnVersionId { get; set; }

        public virtual DateTime? SubmittedDate { get; set; }

        public virtual string CompanyName { get; set; }

        public virtual string TradingName { get; set; }

        public virtual string CompanyRegistrationNumber { get; set; }

        public virtual DirectProducerSubmissionStatus DirectProducerSubmissionStatus { get; set; }

        public virtual DirectProducerSubmission DirectProducerSubmission { get; set; }

        public virtual Guid? ContactId { get; private set; }

        public virtual Guid? BusinessAddressId { get; private set; }

        public virtual Guid? ContactAddressId { get; private set; }

        public virtual Guid? BrandNameId { get; private set; }

        public virtual Address ServiceOfNoticeAddress { get; set; }

        public virtual Contact AppropriateSignatory { get; set; }

        public virtual Address ContactAddress { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Address BusinessAddress { get; set; }

        public virtual BrandName BrandName { get; set; }

        public virtual EeeOutputReturnVersion EeeOutputReturnVersion { get; set; }

        public DirectProducerSubmissionHistory()
        {
        }

        public DirectProducerSubmissionHistory(DirectProducerSubmission directProducerSubmission)
        {
            DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete;
            DirectProducerSubmission = directProducerSubmission;
        }
    }
}
