namespace EA.Weee.Core.AatfReturn
{
    using System;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;

    [Serializable]
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
