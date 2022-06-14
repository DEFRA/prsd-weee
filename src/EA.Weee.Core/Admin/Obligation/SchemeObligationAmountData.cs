namespace EA.Weee.Core.Admin.Obligation
{
    using EA.Weee.Core.DataReturns;

    public class SchemeObligationAmountData
    {
        public WeeeCategory Category { get; private set; }

        public decimal? Obligation { get; private set; }

        public SchemeObligationAmountData(WeeeCategory category,
            decimal? obligation)
        {
            Category = category;
            Obligation = obligation;
        }
    }
}
