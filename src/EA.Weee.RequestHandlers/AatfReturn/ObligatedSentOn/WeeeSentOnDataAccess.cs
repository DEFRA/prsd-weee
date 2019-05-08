namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class WeeeSentOnDataAccess : IWeeeSentOnDataAccess
    {
        private readonly WeeeContext context;

        public WeeeSentOnDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(WeeeSentOn weeeSentOn)
        {
            context.WeeeSentOn.Add(weeeSentOn);

            return context.SaveChangesAsync();
        }

        public Task UpdateWithOperatorAddress(WeeeSentOn weeeSentOn, AatfAddress @operator)
        {
            weeeSentOn.UpdateWithOperatorAddress(@operator);

            return context.SaveChangesAsync();
        }

        public async Task<AatfAddress> GetWeeeSentOnOperatorAddress(Guid id)
        {
            return await context.WeeeSentOn.Where(w => w.Id == id).Select(w => w.OperatorAddress).SingleOrDefaultAsync();
        }

        public async Task<AatfAddress> GetWeeeSentOnSiteAddress(Guid id)
        {
            return await context.WeeeSentOn.Where(w => w.Id == id).Select(w => w.SiteAddress).SingleOrDefaultAsync();
        }

        public async Task<List<WeeeSentOn>> GetWeeeSentOnByReturnAndAatf(Guid aatfId, Guid returnId)
        {
            return await context.WeeeSentOn.Where(w => w.AatfId == aatfId && w.ReturnId == returnId).ToListAsync();
        }

        public async Task<WeeeSentOn> GetWeeeSentOnById(Guid weeeSentOnId)
        {
            return await context.WeeeSentOn.Where(w => w.Id == weeeSentOnId).SingleAsync();
        }
    }
}