namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public class ReturnSchemeDataAccess : IReturnSchemeDataAccess
    {
        private readonly WeeeContext context;

        public ReturnSchemeDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> Submit(ReturnScheme scheme)
        {
            context.ReturnScheme.Add(scheme);

            await context.SaveChangesAsync();

            return scheme.Id;
        }

        public async Task<List<ReturnScheme>> GetSelectedSchemesByReturnId(Guid returnId)
        {
            return await context.ReturnScheme.Where(now => now.ReturnId == returnId).Include(s => s.Scheme).ToListAsync();
        }

        public async Task<Organisation> GetOrganisationByReturnId(Guid returnId)
        {
            var @return = await context.Returns.FirstOrDefaultAsync(r => r.Id == returnId);

            return @return?.Organisation;
        }

        public async Task RemoveReturnScheme(Guid schemeId)
        {
            List<WeeeReceived> weeeReceived = await context.WeeeReceived.Where(p => p.SchemeId == schemeId).ToListAsync();

            foreach (WeeeReceived weee in weeeReceived)
            {
                List<WeeeReceivedAmount> weeeReceivedData = await context.WeeeReceivedAmount.Where(p => p.WeeeReceived.Id == weee.Id).ToListAsync();

                context.WeeeReceivedAmount.RemoveRange(weeeReceivedData);

                context.WeeeReceived.Remove(weee);
            }

            var scheme = await context.ReturnScheme.FirstOrDefaultAsync(p => p.SchemeId == schemeId);

            context.ReturnScheme.Remove(scheme);

            await context.SaveChangesAsync();
        }
    }
}
