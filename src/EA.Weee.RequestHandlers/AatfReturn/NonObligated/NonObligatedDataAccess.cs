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

        public async Task InsertNonObligatedWeee(Guid returnId, IEnumerable<NonObligatedWeee> nonObligatedWeees)
        {
            var allAssociatedWithReturn = await FetchNonObligatedWeeeForReturn(returnId);

            foreach (var noWeee in nonObligatedWeees)
            {
                var group = allAssociatedWithReturn.Where(e => e.Dcf == noWeee.Dcf && e.CategoryId == noWeee.CategoryId);
                if (group.Count() == 0)
                {
                    context.NonObligatedWeee.Add(noWeee);
                }
                else
                {
                    group.First().UpdateTonnage(noWeee.Tonnage);
                    context.NonObligatedWeee.RemoveRange(group.Skip(1)); // Remove any additional ones
                }
            }

            await context.SaveChangesAsync();

            return;
        }

        public async Task UpdateNonObligatedWeeeAmounts(Guid returnId, IEnumerable<Tuple<Guid, decimal?>> amounts)
        {
            var allAssociatedWithReturn = await FetchAllNonObligateWeeeForReturn(returnId);

            var toRemove = new List<NonObligatedWeee>();

            foreach (var amount in amounts)
            {
                var existing = allAssociatedWithReturn.FirstOrDefault(n => n.Id == amount.Item1);
                if (existing == null)
                {
                    throw new Exception("Unable to find a non-obligated WEEE with ID:" + amount.Item1.ToString() + " for Return: " + returnId.ToString());
                }
                existing.UpdateTonnage(amount.Item2);
                toRemove.AddRange(allAssociatedWithReturn.Where(n => n.CategoryId == existing.CategoryId && n.Dcf == existing.Dcf && n != existing));
            }

            context.NonObligatedWeee.RemoveRange(toRemove);
            await context.SaveChangesAsync();
            return;
        }

        public async Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturn(Guid returnId)
        {
            var notDcf = await FetchNonObligatedWeeeForReturnWithoutDuplicates(returnId, false);
            var isDcf = await FetchNonObligatedWeeeForReturnWithoutDuplicates(returnId, true);
            return notDcf.Concat(isDcf).ToList();
        }

        public async Task<List<decimal?>> FetchNonObligatedWeeeForReturn(Guid returnId, bool dcf)
        {
            return (await FetchNonObligatedWeeeForReturnWithoutDuplicates(returnId, dcf)).Select(n => n.Tonnage).ToList();
        }

        public async Task<List<NonObligatedWeee>> FetchAllNonObligateWeeeForReturn(Guid returnId)
        {
            return await context.NonObligatedWeee.Where(now => now.ReturnId == returnId).ToListAsync();
        }

        public async Task<List<NonObligatedWeee>> FetchNonObligatedWeeeForReturnWithoutDuplicates(Guid returnId, bool dcf)
        {
            return await context.NonObligatedWeee
                .Where(now => now.ReturnId == returnId && now.Dcf == dcf)
                .GroupBy(n => n.CategoryId)
                .Select(g => g.FirstOrDefault())
                .Where(n => n != null)
                .ToListAsync();
        }
    }
}
