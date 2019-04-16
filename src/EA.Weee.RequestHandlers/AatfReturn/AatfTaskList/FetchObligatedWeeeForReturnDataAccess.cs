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

        public async Task<List<WeeeReceivedAmount>> FetchObligatedWeeeReceivedForReturn(Guid returnId)
        {
            return await context.WeeeReceivedAmount.Where(a => a.WeeeReceived.ReturnId == returnId)
                .Include(a => a.WeeeReceived.Aatf)
                .Include(a => a.WeeeReceived.Scheme)
                .Select(a => a).ToListAsync();
        }

        public async Task<List<WeeeReusedAmount>> FetchObligatedWeeeReusedForReturn(Guid returnId)
        {
            return await context.WeeeReusedAmount.Where(a => a.WeeeReused.ReturnId == returnId)
                .Include(a => a.WeeeReused.Aatf)
                .Select(a => a).ToListAsync();
        }

        public async Task<List<WeeeSentOnAmount>> FetchObligatedWeeeSentOnForReturn(Guid weeeSentOnId)
        {
            return await context.WeeeSentOnAmount.Where(a => a.WeeeSentOnId == weeeSentOnId).ToListAsync();
        }

        public async Task<List<WeeeSentOnAmount>> FetchObligatedWeeeSentOnForReturnByReturn(Guid returnId)
        {
            return await context.WeeeSentOnAmount.Where(a => a.WeeeSentOn.ReturnId == returnId)
                .Include(s => s.WeeeSentOn)
                .Include(s => s.WeeeSentOn.Aatf)
                .ToListAsync();
        }
    }
}