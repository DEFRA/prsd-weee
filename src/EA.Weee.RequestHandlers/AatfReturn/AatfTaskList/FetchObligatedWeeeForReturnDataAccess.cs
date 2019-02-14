namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    public class FetchObligatedWeeeForReturnDataAccess : IFetchObligatedWeeeForReturnDataAccess
    {
        private readonly WeeeContext context;

        public FetchObligatedWeeeForReturnDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<WeeeReceivedAmount>> FetchObligatedWeeeForReturn(Guid returnId)
        {
            return await context.AatfWeeReceivedAmount.Where(a => a.WeeeReceived.ReturnId == returnId).Select(a => a).ToListAsync();
        }
    }
}