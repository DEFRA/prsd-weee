namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;

    public class NonObligatedDataAccess : INonObligatedDataAccess
    {
        private readonly WeeeContext context;

        public NonObligatedDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(IEnumerable<NonObligatedWeee> nonObligated)
        {
            foreach (var nonObligatedWeee in nonObligated)
            {
                context.NonObligatedWeee.Add(nonObligatedWeee);
            }

            return context.SaveChangesAsync();
        }

        public Task UpdateAmount(NonObligatedWeee amount, decimal? tonnage)
        {
            amount.UpdateTonnage(tonnage);

            return context.SaveChangesAsync();
        }

        public async Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId)
        {
            return await context.NonObligatedWeee.Where(now => now.ReturnId == returnId).Select(now => now).ToListAsync();
        }

        public async Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf)
        {
            return await context.NonObligatedWeee.Where(now => now.ReturnId == returnId).Where(now => now.Dcf == dcf).Select(now => (decimal?)now.Tonnage).ToListAsync();
        }
    }
}
