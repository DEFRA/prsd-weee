namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;
    using DataAccess.Specification;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class AatfsByOrganisationAndFacilityTypeSpecification : Specification<Aatf>
    {
        public Guid OrganisationId { get; private set; }

        public FacilityType FacilityType { get; set; }

        public AatfsByOrganisationAndFacilityTypeSpecification(Guid organisationId, FacilityType facilityType)
        {
            OrganisationId = organisationId;
            FacilityType = facilityType;
        }

        public override Expression<Func<Aatf, bool>> ToExpression()
        {
            return aatf => aatf.Organisation.Id == OrganisationId && aatf.FacilityType.Value.Equals((int)FacilityType);
        }
    }
}
