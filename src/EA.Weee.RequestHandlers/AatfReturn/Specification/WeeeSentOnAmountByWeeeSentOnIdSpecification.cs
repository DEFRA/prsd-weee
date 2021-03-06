﻿namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;

    public class WeeeSentOnAmountByWeeeSentOnIdSpecification : Specification<WeeeSentOnAmount>
    {
        public Guid WeeeSentOnId { get; private set; }

        public WeeeSentOnAmountByWeeeSentOnIdSpecification(Guid weeeReceivedId)
        {
            WeeeSentOnId = weeeReceivedId;
        }

        public override Expression<Func<WeeeSentOnAmount, bool>> ToExpression()
        {
            return weeeSentOnAmount => weeeSentOnAmount.WeeeSentOn.Id == WeeeSentOnId;
        }
    }
}