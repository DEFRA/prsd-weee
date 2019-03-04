namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    using System;

    public class NonObligatedValue
    {
        public Guid Id { get; set; }

        public int CategoryId { get; private set; }

        public decimal? Tonnage { get; private set; }

        public bool Dcf { get; private set; }

        public NonObligatedValue(int categoryId, decimal? tonnage, bool dcf, Guid id)
        {
            CategoryId = categoryId;
            Tonnage = tonnage;
            Dcf = dcf;
            Id = id;
        }
    }
}
