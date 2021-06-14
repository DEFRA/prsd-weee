namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using DataAccess;
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class NonObligatedDataAccess : INonObligatedDataAccess
    {
        private readonly WeeeContext context;

        public NonObligatedDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Remove(IEnumerable<NonObligatedWeee> nonObligated)
        {
            context.NonObligatedWeee.RemoveRange(nonObligated);
            return context.SaveChangesAsync();
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

        public Task UpdateAmountForIds(IEnumerable<Tuple<Guid, decimal?>> amounts)
        {
            foreach (var amount in amounts)
            {
                context.NonObligatedWeee.First(n => n.Id == amount.Item1).UpdateTonnage(amount.Item2);
            }
            return context.SaveChangesAsync();
        }

        public async Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId)
        {
            return EnsureNoDuplicateData(await context.NonObligatedWeee.Where(now => now.ReturnId == returnId).Select(now => now).ToListAsync());
        }

        public async Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf)
        {
            return EnsureNoDuplicateData(await context.NonObligatedWeee.Where(now => now.ReturnId == returnId).Where(now => now.Dcf == dcf).ToListAsync())
                .Select(now => now.Tonnage).ToList();
        }

        private List<NonObligatedWeee> EnsureNoDuplicateData(List<NonObligatedWeee> fetchedData)
        {
            var grouped = fetchedData.GroupBy(n => n.CategoryId);
            if (grouped.Any(g => g.Count() > 1))
            {
                fetchedData = grouped.Select(g => g.OrderByDescending(n => n.Tonnage).First()).ToList();
            }
            return fetchedData;
        }
    }
}
