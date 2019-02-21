namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Linq.Expressions;
    using Domain.AatfReturn;
    using Prsd.Core.Domain;

    public class AatfsByOrganisationSpecification : Specification<Aatf>
    {
        public Guid OrganisationId { get; private set; }

        public AatfsByOrganisationSpecification(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public override Expression<Func<Aatf, bool>> ToExpression()
        {
            return @operator => @operator.Operator.Organisation.Id == OrganisationId;
        }
    }
}
