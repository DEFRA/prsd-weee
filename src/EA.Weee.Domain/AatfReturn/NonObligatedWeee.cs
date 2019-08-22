namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;

    public partial class NonObligatedWeee : ReturnEntity, IReturnOption
    {
        public NonObligatedWeee(Return aatfReturn, int categoryId, bool dcf, decimal? tonnage)
        {
            Guard.ArgumentNotNull(() => aatfReturn, aatfReturn);
            Return = aatfReturn;
            Dcf = dcf;
            Tonnage = tonnage;
            CategoryId = categoryId;
        }

        public NonObligatedWeee()
        {
        }

        public virtual void UpdateTonnage(decimal? tonnage)
        {
            Tonnage = tonnage;
        }

        public int CategoryId { get; set; }

        public bool Dcf { get; set; }

        public decimal? Tonnage { get; set; }
    }
}
