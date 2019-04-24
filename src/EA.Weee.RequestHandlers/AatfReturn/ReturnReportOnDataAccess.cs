namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class ReturnReportOnDataAccess : IReturnReportOnDataAccess
    {
        private readonly WeeeContext context;

        public ReturnReportOnDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(IEnumerable<ReturnReportOn> reportOptions)
        {
            throw new NotImplementedException();
        }
    }
}
