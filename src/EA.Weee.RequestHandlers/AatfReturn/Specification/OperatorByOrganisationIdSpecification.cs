namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

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
