namespace EA.Weee.Requests.AatfReturn.ObligatedReceived
{
    public class ObligatedReceivedValue
    {
        public int CategoryId { get; private set; }

        public decimal? Tonnage { get; private set; }

        public bool IsHousehold { get; private set; }

        public ObligatedReceivedValue(int categoryId, decimal? tonnage, bool isHousehold)
        {
            CategoryId = categoryId;
            Tonnage = tonnage;
            IsHousehold = isHousehold;
        }
    }
}
