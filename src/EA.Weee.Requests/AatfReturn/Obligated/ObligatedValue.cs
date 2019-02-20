namespace EA.Weee.Requests.AatfReturn.Obligated
{
    public class ObligatedValue
    {
        public int CategoryId { get; private set; }

        public decimal? HouseholdTonnage { get; private set; }

        public decimal? NonHouseholdTonnage { get; private set; }
        
        public ObligatedValue(int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }
    }
}
