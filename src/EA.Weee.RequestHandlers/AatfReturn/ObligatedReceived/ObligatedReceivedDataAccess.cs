namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class ObligatedReceivedDataAccess : IObligatedReceivedDataAccess
    {
        private readonly WeeeContext context;
        
        public ObligatedReceivedDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(IEnumerable<WeeeReceivedAmount> aatfWeeeReceivedAmounts)
        {
            context.WeeeReceivedAmount.AddRange(aatfWeeeReceivedAmounts);

            return context.SaveChangesAsync();
        }

        public Task UpdateAmounts(WeeeReceivedAmount receivedAmount, decimal? houseHoldTonnage, decimal? nonHouseHoldTonnage)
        {
            receivedAmount.UpdateTonnages(houseHoldTonnage, nonHouseHoldTonnage);

            return context.SaveChangesAsync();
        }
    }
}
