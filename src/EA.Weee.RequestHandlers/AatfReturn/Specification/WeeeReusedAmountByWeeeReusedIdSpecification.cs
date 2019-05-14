namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Linq.Expressions;
    using Domain.AatfReturn;

    public class WeeeReusedAmountByWeeeReusedIdSpecification : Specification<WeeeReusedAmount>
    {
        public Guid WeeeReusedId { get; private set; }

        public WeeeReusedAmountByWeeeReusedIdSpecification(Guid weeeReusedId)
        {
            WeeeReusedId = weeeReusedId;
        }

        public override Expression<Func<WeeeReusedAmount, bool>> ToExpression()
        {
            return @weeeReused => @weeeReused.WeeeReused.Id == WeeeReusedId;
        }
    }
}