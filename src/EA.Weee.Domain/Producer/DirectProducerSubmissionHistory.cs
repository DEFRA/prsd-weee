namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core;
    using EA.Weee.Domain.Audit;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer.Classfication;
    using System;

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

        public virtual Guid? AuthorisedRepresentativeId { get; private set; }

        public virtual Address ServiceOfNoticeAddress { get; private set; }

        public virtual Contact AppropriateSignatory { get; private set; }

        public virtual Address ContactAddress { get; private set; }

        public virtual Contact Contact { get; private set; }

        public virtual Address BusinessAddress { get; private set; }

        public virtual BrandName BrandName { get; private set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; set; }

        public virtual EeeOutputReturnVersion EeeOutputReturnVersion { get; set; }

        public virtual SellingTechniqueType SellingTechniqueType { get; set; }

        public DirectProducerSubmissionHistory()
        {
        }

        public DirectProducerSubmissionHistory(DirectProducerSubmission directProducerSubmission)
        {
            DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete;
            DirectProducerSubmission = directProducerSubmission;
        }

        public DirectProducerSubmissionHistory(DirectProducerSubmission directProducerSubmission, BrandName brandName, Address businessAddress)
        {
            DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete;
            DirectProducerSubmission = directProducerSubmission;
            BusinessAddress = businessAddress;
            BrandName = brandName;
        }

        public void AddOrUpdateBusinessAddress(Address address)
        {
            Guard.ArgumentNotNull(() => address, address);

            BusinessAddress = address.OverwriteWhereNull(BusinessAddress);
        }

        public void AddOrUpdateBrandName(BrandName brandName)
        {
            Guard.ArgumentNotNull(() => brandName, brandName);

            BrandName = brandName.OverwriteWhereNull(brandName);
        }
    }
}
