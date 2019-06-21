namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Linq.Expressions;
    using Domain.AatfReturn;

    public class OperatorByOrganisationIdSpecification : Specification<Operator>
    {
        public Guid OrganisationId { get; private set; }

        public OperatorByOrganisationIdSpecification(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public override Expression<Func<Operator, bool>> ToExpression()
        {
            return @operator => @operator.Organisation.Id == OrganisationId;
        }
    }
}
