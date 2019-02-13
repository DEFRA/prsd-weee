namespace EA.Weee.Requests.AatfReturn.ObligatedReceived
{
    public class ObligatedReceivedValue
    {
        public int CategoryId { get; private set; }

        public decimal? HouseholdTonnage { get; private set; }

        public decimal? NonHouseholdTonnage { get; private set; }
        
        public ObligatedReceivedValue(int categoryId, decimal? householdTonnage, decimal? nonHouseholdTonnage)
        {
            CategoryId = categoryId;
            HouseholdTonnage = householdTonnage;
            NonHouseholdTonnage = nonHouseholdTonnage;
        }
    }
}
