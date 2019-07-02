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

        public async Task<List<Guid>> Submit(List<ReturnScheme> schemes)
        {
            context.ReturnScheme.AddRange(schemes);

            await context.SaveChangesAsync();

            return schemes.Select(s => s.Id).ToList();
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

        public async Task RemoveReturnScheme(List<Guid> schemeIds, Guid returnId)
        {
            foreach (var schemeId in schemeIds)
            {
                List<WeeeReceived> weeeReceived = await context.WeeeReceived.Where(p => p.SchemeId == schemeId && p.ReturnId == returnId).ToListAsync();

                foreach (WeeeReceived weee in weeeReceived)
                {
                    List<WeeeReceivedAmount> weeeReceivedData = await context.WeeeReceivedAmount.Where(p => p.WeeeReceived.Id == weee.Id).ToListAsync();

                    context.WeeeReceivedAmount.RemoveRange(weeeReceivedData);

                    context.WeeeReceived.Remove(weee);
                }

                var scheme = await context.ReturnScheme.FirstOrDefaultAsync(p => p.SchemeId == schemeId && p.ReturnId == returnId);

                context.ReturnScheme.Remove(scheme);
            }

            await context.SaveChangesAsync();
        }
    }
}
