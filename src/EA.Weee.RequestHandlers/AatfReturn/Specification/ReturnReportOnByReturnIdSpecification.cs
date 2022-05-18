namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;
    using DataAccess.Specification;

    public class ReturnReportOnByReturnIdSpecification : Specification<ReturnReportOn>
    {
        public Guid ReturnId { get; private set; }

        public ReturnReportOnByReturnIdSpecification(Guid returnId)
        {
            ReturnId = returnId;
        }

        public override Expression<Func<ReturnReportOn, bool>> ToExpression()
        {
            return @returnReportOn => @returnReportOn.ReturnId == ReturnId;
        }
    }
}
