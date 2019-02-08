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
