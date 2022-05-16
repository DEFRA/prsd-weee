namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;
    using DataAccess.Specification;

    public class WeeeReceivedAmountByWeeeReceivedIdSpecification : Specification<WeeeReceivedAmount>
    {
        public Guid WeeeReceivedId { get; private set; }

        public WeeeReceivedAmountByWeeeReceivedIdSpecification(Guid weeeReceivedId)
        {
            WeeeReceivedId = weeeReceivedId;
        }

        public override Expression<Func<WeeeReceivedAmount, bool>> ToExpression()
        {
            return @weeeReceivedAmount => @weeeReceivedAmount.WeeeReceived.Id == WeeeReceivedId;
        }
    }
}