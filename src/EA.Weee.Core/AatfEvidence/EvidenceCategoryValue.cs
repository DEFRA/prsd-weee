namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using AatfReturn;
    using DataReturns;
    using Validation;

    [Serializable]
    public class EvidenceCategoryValue : CategoryValue
    {
        [TonnageValue(nameof(CategoryId), "The tonnage value", "household", true)]
        public string Received { get; set; }

        [TonnageValue(nameof(CategoryId), "The tonnage value", "non-household", true)]
        [TonnageCompareValue(nameof(CategoryId), nameof(Received))]
        public string Reused { get; set; }

        public EvidenceCategoryValue()
        {
        }

        public EvidenceCategoryValue(string received, string reused)
        {
            Received = received;
            Reused = reused;
        }

        public EvidenceCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}
