namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Producer;
    using System;

    public class AdditionalCompanyDetails : Entity
    {
        //public Guid DirectRegistrantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public OrganisationAdditionalDetailsType Type { get; set; }

        public virtual DirectRegistrant DirectRegistrant { get; set; }
    }
}
