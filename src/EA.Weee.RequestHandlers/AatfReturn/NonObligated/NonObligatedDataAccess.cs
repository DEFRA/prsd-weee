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

        public async Task AddUpdateAndClean(Guid returnId, IEnumerable<NonObligatedWeee> nonObligated)
        {
            var allAssociatedWithReturn = await FetchAllNonObligatedWeeeForReturn(returnId);
            var toAdd = new List<NonObligatedWeee>();
            var toRemove = allAssociatedWithReturn.Where(a => !nonObligated.Any(n => n.Id == a.Id));

            // Update
            foreach (var n in nonObligated)
            {
                var existing = allAssociatedWithReturn.FirstOrDefault(e => e.Id == n.Id);
                if (existing == null)
                {
                    toAdd.Add(n);
                }
                else
                {
                    existing.UpdateTonnage(n.Tonnage);
                }
            }

            // Add missing
            context.NonObligatedWeee.AddRange(toAdd);

            // Remove unwanted
            context.NonObligatedWeee.RemoveRange(toRemove);

            // Save
            await context.SaveChangesAsync();

            return;
        }

        public async Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId)
        {
            return EnsureNoDuplicateData(await FetchAllNonObligatedWeeeForReturn(returnId));
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

        private async Task<List<NonObligatedWeee>> FetchAllNonObligatedWeeeForReturn(Guid returnId)
        {
            return await context.NonObligatedWeee.Where(now => now.ReturnId == returnId).Select(now => now).ToListAsync();
        }
    }
}
