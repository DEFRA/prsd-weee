namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;
    using System;
    using CuttingEdge.Conditions;

    public class DirectRegistrant : Entity
    {
        public DirectRegistrant()
        {
        }

        public DirectRegistrant(Organisation organisation, BrandName brandName)
        {
            Condition.Requires(organisation).IsNotNull();

            Organisation = organisation;
            BrandName = brandName;
        }

        public virtual Guid OrganisationId { get; private set; }
        public virtual Guid? SICCodeId { get; private set; }
        public virtual Guid? BrandNameId { get; private set; }
        public virtual Guid? RepresentingCompanyId { get; private set; }

        public virtual Organisation Organisation { get; private set; }
        public virtual SICCode SICCode { get; private set; }
        public virtual BrandName BrandName { get; private set; }
        public virtual ProducerBusiness RepresentingCompany { get; private set; }

        public static DirectRegistrant CreateDirectRegistrant(Organisation organisation, BrandName brandName)
        {
            return new DirectRegistrant(organisation, brandName);
        }
    }
}
