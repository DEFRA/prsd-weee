namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;
    using DataAccess.Specification;

    public class WeeeReusedAmountByWeeeReusedIdSpecification : Specification<WeeeReusedAmount>
    {
        public Guid WeeeReusedId { get; private set; }

        public WeeeReusedAmountByWeeeReusedIdSpecification(Guid weeeReusedId)
        {
            WeeeReusedId = weeeReusedId;
        }

        public override Expression<Func<WeeeReusedAmount, bool>> ToExpression()
        {
            return weeeReceivedAmount => weeeReceivedAmount.WeeeReused.Id == WeeeReusedId;
        }
    }
}