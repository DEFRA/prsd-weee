namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class WeeeReceivedAmount : Entity
    {
        public virtual WeeeReceived WeeeReceived { get; private set; }

        public int CategoryId { get; set; }

        public decimal? HouseholdTonnage { get; set; }

        public decimal? NonHouseholdTonnage { get; set; }
        
        public WeeeReceivedAmount(WeeeReceived aatfWeeeReceived, int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            Guard.ArgumentNotNull(() => aatfWeeeReceived, aatfWeeeReceived);

            WeeeReceived = aatfWeeeReceived;
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }

        protected WeeeReceivedAmount()
        {
        }
    }
}
