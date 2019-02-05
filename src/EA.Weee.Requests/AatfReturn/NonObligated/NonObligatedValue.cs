namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    public class NonObligatedValue
    {
        public int CategoryId { get; private set; }

        public decimal? Tonnage { get; private set; }

        public bool Dcf { get; private set; }

        public NonObligatedValue(int categoryId, decimal? tonnage, bool dcf)
        {
            CategoryId = categoryId;
            Tonnage = tonnage;
            Dcf = dcf;
        }
    }
}
