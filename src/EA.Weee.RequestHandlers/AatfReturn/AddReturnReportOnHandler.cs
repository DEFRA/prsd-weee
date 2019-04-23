namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.AatfReturn;

    public class AddReturnReportOnHandler : IRequestHandler<AddReturnReportOn, bool>
    {
        public Task<bool> HandleAsync(AddReturnReportOn message)
        {
            throw new NotImplementedException();
        }
    }
}
