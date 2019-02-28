namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Linq.Expressions;
    using EA.Weee.Domain.AatfReturn;

    public class WeeeReusedByReturnIdSpecification : Specification<WeeeReused>
    {
        public Guid ReturnId { get; private set; }

        public WeeeReusedByReturnIdSpecification(Guid returnId)
        {
            ReturnId = returnId;
        }

        public override Expression<Func<WeeeReused, bool>> ToExpression()
        {
            return weeeReused => weeeReused.ReturnId == ReturnId;
        }
    }
}
