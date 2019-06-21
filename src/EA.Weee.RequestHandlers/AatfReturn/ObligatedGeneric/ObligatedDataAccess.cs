namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;

    public abstract class ObligatedDataAccess<T> : IObligatedDataAccess<T> where T : ObligatedAmount
    {
        private readonly WeeeContext context;

        protected ObligatedDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(IEnumerable<T> amounts)
        {
            context.Set<T>().AddRange(amounts);

            return context.SaveChangesAsync();
        }

        public Task UpdateAmounts(T receivedAmount, decimal? houseHoldTonnage, decimal? nonHouseHoldTonnage)
        {
            receivedAmount.UpdateTonnages(houseHoldTonnage, nonHouseHoldTonnage);

            return context.SaveChangesAsync();
        }
    }
}
