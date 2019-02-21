namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System.Collections.Generic;
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
    }
}
