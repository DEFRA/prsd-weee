namespace EA.Weee.Domain.Producer
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;
    using System;
    using System.Collections.Generic;

    public partial class DirectRegistrant : Entity
    {
        public DirectRegistrant()
        {
            DirectProducerSubmissions = new List<DirectProducerSubmission>();
        }

        public DirectRegistrant(Organisation organisation, BrandName brandName, Contact contactDetails, Address contactAddress, AuthorisedRepresentative authorisedRepresentative, List<AdditionalCompanyDetails> additionalCompanyDetails)
        {
            Condition.Requires(organisation).IsNotNull();

            Organisation = organisation;
            BrandName = brandName;
            Contact = contactDetails;
            Address = contactAddress;
            AuthorisedRepresentative = authorisedRepresentative;
            AdditionalCompanyDetails = additionalCompanyDetails;
            DirectProducerSubmissions = new List<DirectProducerSubmission>();
        }

        public DirectRegistrant(Organisation organisation)
        {
            Condition.Requires(organisation).IsNotNull();

            Organisation = organisation;
            DirectProducerSubmissions = new List<DirectProducerSubmission>();
        }

        public virtual Guid OrganisationId { get; private set; }

        public virtual Guid? SICCodeId { get; private set; }

        public virtual Guid? BrandNameId { get; private set; }

        public virtual Guid? AuthorisedRepresentativeId { get; private set; }

        public virtual Guid? ContactId { get; private set; }

        public virtual Guid? AddressId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual SICCode SICCode { get; private set; }

        public virtual BrandName BrandName { get; private set; }

        public virtual Contact Contact { get; private set; }

        public virtual Address Address { get; private set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; private set; }

        public virtual ICollection<DirectProducerSubmission> DirectProducerSubmissions { get; set; }

        public virtual ICollection<AdditionalCompanyDetails> AdditionalCompanyDetails { get; set; }

        public static DirectRegistrant CreateDirectRegistrant(Organisation organisation, BrandName brandName, Contact contactDetails, Address contactAddress, AuthorisedRepresentative representingCompany, List<AdditionalCompanyDetails> additionalCompanyDetails)
        {
            return new DirectRegistrant(organisation, brandName, contactDetails, contactAddress, representingCompany, additionalCompanyDetails);
        }
    }
}
