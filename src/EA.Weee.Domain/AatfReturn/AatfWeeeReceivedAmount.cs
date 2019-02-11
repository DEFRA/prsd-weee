namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class AatfWeeeReceivedAmount : Entity
    {
        public virtual AatfWeeeReceived AatfWeeeReceived { get; private set; }

        public int CategoryId { get; set; }

        public decimal? HouseholdTonnage { get; set; }

        public decimal? NonHouseholdTonnage { get; set; }
        
        public AatfWeeeReceivedAmount(AatfWeeeReceived aatfWeeeReceived, int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            Guard.ArgumentNotNull(() => aatfWeeeReceived, aatfWeeeReceived);

            AatfWeeeReceived = aatfWeeeReceived;
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }
    }
}
