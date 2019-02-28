namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class WeeeReusedAmount : ObligatedAmount, IObligatedAmount
    {
        public virtual WeeeReused WeeeReused { get; private set; }

        public int CategoryId { get; set; }

        public WeeeReusedAmount(WeeeReused aatfWeeeReused, int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            Guard.ArgumentNotNull(() => aatfWeeeReused, aatfWeeeReused);

            WeeeReused = aatfWeeeReused;
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }

        protected WeeeReusedAmount()
        {
        }
    }
}
