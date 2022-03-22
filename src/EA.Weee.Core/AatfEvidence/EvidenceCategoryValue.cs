namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using AatfReturn;
    using DataReturns;
    using Validation;

    [Serializable]
    public class EvidenceCategoryValue : CategoryValue
    {
        [TonnageValue(nameof(CategoryId), "total received")]
        public string Received { get; set; }

        [TonnageValue(nameof(CategoryId), "reused as whole appliances")]
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
