namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;
    using DataAccess.Specification;

    public class WeeeReusedByAatfIdAndReturnIdSpecification : Specification<WeeeReused>
    {
        public Guid AatfId { get; private set; }
        public Guid ReturnId { get; private set; }

        public WeeeReusedByAatfIdAndReturnIdSpecification(Guid aatfId, Guid returnId)
        {
            AatfId = aatfId;
            ReturnId = returnId;
        }

        public override Expression<Func<WeeeReused, bool>> ToExpression()
        {
            return @weeeReused => @weeeReused.Aatf.Id == AatfId && @weeeReused.ReturnId == ReturnId;
        }
    }
}
