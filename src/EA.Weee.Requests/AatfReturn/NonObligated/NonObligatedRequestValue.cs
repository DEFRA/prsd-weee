namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    public class NonObligatedRequestValue
    {
        public int CategoryId { get; private set; }

        public decimal? Tonnage { get; private set; }

        public bool Dcf { get; private set; }

        public NonObligatedRequestValue(int categoryId, decimal? tonnage, bool dcf)
        {
            CategoryId = categoryId;
            Tonnage = tonnage;
            Dcf = dcf;
        }
    }
}
