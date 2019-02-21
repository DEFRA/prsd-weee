namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class AddObligatedReusedDataAccess : IAddObligatedReusedDataAccess
    {
        private readonly WeeeContext context;

        public AddObligatedReusedDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(IEnumerable<WeeeReusedAmount> aatfWeeeReusedAmounts)
        {
            foreach (var aatfWeeeReused in aatfWeeeReusedAmounts)
            {
                context.WeeeReusedAmount.Add(aatfWeeeReused);
            }

            return context.SaveChangesAsync();
        }
        
        public async Task<Guid> GetAatfId(Guid organisationId)
        {
            return (await context.Aatfs
                    .Include(c => c.Operator)
                .FirstOrDefaultAsync(a => a.Operator.Organisation.Id == organisationId)).Id;
        }
    }
}
