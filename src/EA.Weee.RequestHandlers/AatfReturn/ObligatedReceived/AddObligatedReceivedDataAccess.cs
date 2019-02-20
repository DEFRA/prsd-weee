namespace EA.Weee.RequestHandlers.AatfReturn.Obligated
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class AddObligatedReceivedDataAccess : IAddObligatedReceivedDataAccess
    {
        private readonly WeeeContext context;

        public AddObligatedReceivedDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(IEnumerable<WeeeReceivedAmount> aatfWeeeReceivedAmounts)
        {
            foreach (var aatfWeeeReceived in aatfWeeeReceivedAmounts)
            {
                context.WeeeReceivedAmount.Add(aatfWeeeReceived);
            }

            return context.SaveChangesAsync();
        }

        public async Task<Guid> GetSchemeId(Guid organisationId)
        {
            return (await context.Schemes.FirstOrDefaultAsync(s => s.OrganisationId == organisationId)).Id;
        }

        public async Task<Guid> GetAatfId(Guid organisationId)
        {
            return (await context.Aatfs
                    .Include(c => c.Operator)
                .FirstOrDefaultAsync(a => a.Operator.Organisation.Id == organisationId)).Id;
        }
    }
}
