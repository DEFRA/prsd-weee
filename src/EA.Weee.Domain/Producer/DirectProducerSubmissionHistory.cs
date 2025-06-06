﻿namespace EA.Weee.Domain.Producer
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

        public virtual DirectProducerSubmission DirectProducerSubmission { get; set; }

        public virtual Guid? ContactId { get; private set; }

        public virtual Guid? BusinessAddressId { get; private set; }

        public virtual Guid? ContactAddressId { get; set; }

        public virtual Guid? BrandNameId { get; private set; }

        public virtual Guid? AuthorisedRepresentativeId { get; private set; }

        public virtual Address ServiceOfNoticeAddress { get; private set; }

        public virtual Contact AppropriateSignatory { get; private set; }

        public virtual Address ContactAddress { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Address BusinessAddress { get; set; }

        public virtual BrandName BrandName { get; set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; set; }

        public virtual EeeOutputReturnVersion EeeOutputReturnVersion { get; set; }

        public virtual int? SellingTechniqueType { get; set; }

        public DirectProducerSubmissionHistory()
        {
        }

        public DirectProducerSubmissionHistory(DirectProducerSubmission directProducerSubmission)
        {
            DirectProducerSubmission = directProducerSubmission;
        }

        public DirectProducerSubmissionHistory(DirectProducerSubmission directProducerSubmission, BrandName brandName, Address businessAddress, AuthorisedRepresentative authorisedRepresentative = null)
        {
            DirectProducerSubmission = directProducerSubmission;
            BusinessAddress = businessAddress;
            BrandName = brandName;
            AuthorisedRepresentativeId = authorisedRepresentative?.Id;
        }

        public void AddOrUpdateBusinessAddress(Address address)
        {
            Guard.ArgumentNotNull(() => address, address);

            BusinessAddress = address.OverwriteWhereNull(BusinessAddress);
        }

        public void AddOrUpdateAuthorisedRepresentative(AuthorisedRepresentative authorisedRepresentative)
        {
            Guard.ArgumentNotNull(() => authorisedRepresentative, authorisedRepresentative);

            AuthorisedRepresentative = authorisedRepresentative.OverwriteWhereNull(AuthorisedRepresentative);
        }

        public void AddOrUpdateBrandName(BrandName brandName)
        {
            Guard.ArgumentNotNull(() => brandName, brandName);

            BrandName = brandName.OverwriteWhereNull(BrandName);
        }

        public void AddOrUpdateServiceOfNotice(Address address)
        {
            Guard.ArgumentNotNull(() => address, address);

            ServiceOfNoticeAddress = address.OverwriteWhereNull(ServiceOfNoticeAddress);
        }

        public void AddOrUpdateContact(Contact contact)
        {
            Guard.ArgumentNotNull(() => contact, contact);

            Contact = contact.OverwriteWhereNull(Contact);
        }

        public void AddOrUpdateContactAddress(Address address)
        {
            Guard.ArgumentNotNull(() => address, address);

            ContactAddress = address.OverwriteWhereNull(ContactAddress);
        }

        public void AddOrUpdateAppropriateSignatory(Contact contact)
        {
            Guard.ArgumentNotNull(() => contact, contact);

            AppropriateSignatory = contact.OverwriteWhereNull(AppropriateSignatory);
        }

        public void RemoveBrandName()
        {
            BrandName = null;
            BrandNameId = null;
        }
    }
}
