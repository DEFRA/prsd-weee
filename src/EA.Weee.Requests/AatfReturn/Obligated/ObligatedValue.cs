namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;

    public class ObligatedValue
    {
        public Guid Id { get; private set; }

        public int CategoryId { get; private set; }

        public decimal? HouseholdTonnage { get; private set; }

        public decimal? NonHouseholdTonnage { get; private set; }
        
        public ObligatedValue(Guid id, int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            Id = id;
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }
    }
}
