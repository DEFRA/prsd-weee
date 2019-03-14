namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.DataReturns;

    public class PastedValue : CategoryValue
    {
        public string Tonnage { get; set; }

        public PastedValue()
        {
        }

        public PastedValue(string tonnage)
        {
            Tonnage = tonnage;
        }

        public PastedValue(WeeeCategory category) : base(category)
        {
        }
    }
}
