namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;
    using DataAccess.Specification;

    public class AatfsByOrganisationSpecification : Specification<Aatf>
    {
        public Guid OrganisationId { get; private set; }

        public AatfsByOrganisationSpecification(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public override Expression<Func<Aatf, bool>> ToExpression()
        {
            return aatf => aatf.Organisation.Id == OrganisationId;
        }
    }
}
