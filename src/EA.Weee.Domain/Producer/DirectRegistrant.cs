namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;
    using System;
    using CuttingEdge.Conditions;

    public partial class DirectRegistrant : Entity
    {
        public DirectRegistrant()
        {
        }

        public DirectRegistrant(Organisation organisation, BrandName brandName, AuthorisedRepresentative authorisedRepresentative)
        {
            Condition.Requires(organisation).IsNotNull();

            Organisation = organisation;
            BrandName = brandName;
            AuthorisedRepresentative = authorisedRepresentative;
        }

        public DirectRegistrant(Organisation organisation)
        {
            Condition.Requires(organisation).IsNotNull();

            Organisation = organisation;
        }

        public virtual Guid OrganisationId { get; private set; }

        public virtual Guid? SICCodeId { get; private set; }

        public virtual Guid? BrandNameId { get; private set; }

        public virtual Guid? RepresentingCompanyId { get; private set; }

        public virtual Guid? ContactId { get; private set; }

        public virtual Guid? AddressId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual SICCode SICCode { get; private set; }

        public virtual BrandName BrandName { get; private set; }

        public virtual Contact Contact { get; private set; }

        public virtual Address Address { get; private set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; private set; }

        public static DirectRegistrant CreateDirectRegistrant(Organisation organisation, BrandName brandName, AuthorisedRepresentative representingCompany)
        {
            return new DirectRegistrant(organisation, brandName, representingCompany);
        }
    }
}
