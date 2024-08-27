namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;
    using System;

    public class DirectRegistrant : Entity
    {
        public virtual Guid OrganisationId { get; set; }
        public virtual Guid? SICCodeId { get; set; }
        public virtual Guid? BrandNameId { get; set; }
        public virtual Guid? RepresentingCompanyId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual SICCode SICCode { get; set; }
        public virtual BrandName BrandName { get; set; }
        public virtual ProducerBusiness RepresentingCompany { get; set; }
    }
}
