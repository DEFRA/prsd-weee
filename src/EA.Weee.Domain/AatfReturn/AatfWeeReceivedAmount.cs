namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core.Domain;

    public class AatfWeeReceivedAmount : Entity
    {
        public Guid WeeReceivedId { get; private set; }

        public int CategoryId { get; set; }

        public decimal? HouseholdTonnage { get; set; }

        public decimal? NonHouseholdTonnage { get; set; }
        
        public AatfWeeReceivedAmount(int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }
    }
}
