namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;
    using System;

    public class WeeeSentOnAmount : ObligatedAmount, IObligatedAmount
    {
        public virtual WeeeSentOn WeeeSentOn { get; private set; }

        public int CategoryId { get; set; }

        public WeeeSentOnAmount(WeeeSentOn aatfWeeeSentOn, int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            Guard.ArgumentNotNull(() => aatfWeeeSentOn, aatfWeeeSentOn);

            WeeeSentOn = aatfWeeeSentOn;
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }

        protected WeeeSentOnAmount()
        {
        }
    }
}
