namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Linq.Expressions;
    using Domain.AatfReturn;

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