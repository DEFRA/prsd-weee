namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

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
