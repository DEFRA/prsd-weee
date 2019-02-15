namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Linq.Expressions;
    using Domain.AatfReturn;

    public class OperatorByOrganisationIdSpecification : Specification<Operator>
    {
        private readonly Guid organisationId;

        public OperatorByOrganisationIdSpecification(Guid organisationId)
        {
            this.organisationId = organisationId;
        }

        public override Expression<Func<Operator, bool>> ToExpression()
        {
            return @operator => @operator.Organisation.Id == organisationId;
        }
    }
}
