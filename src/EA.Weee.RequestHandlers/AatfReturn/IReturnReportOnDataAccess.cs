namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

    public interface IReturnReportOnDataAccess
    {
        Task Submit(IEnumerable<ReturnReportOn> reportOptions);
    }
}
