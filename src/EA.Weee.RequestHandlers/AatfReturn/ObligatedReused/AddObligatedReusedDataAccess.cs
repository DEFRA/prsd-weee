namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
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
    }
}
