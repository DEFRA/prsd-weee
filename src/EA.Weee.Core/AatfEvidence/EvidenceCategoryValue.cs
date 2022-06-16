namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using DataReturns;
    using EA.Weee.Core.Shared;
    using Validation;

    [Serializable]
    public class EvidenceCategoryValue : CategoryValue, IEvidenceCategoryValue
    {
        [TonnageValue(nameof(CategoryId), "The tonnage value", true)]
        public virtual string Received { get; set; }

        [TonnageValue(nameof(CategoryId), "The tonnage value", true)]
        [TonnageCompareValue(nameof(CategoryId), nameof(Received), "The reused tonnage for category {0} must be equivalent or lower than the received tonnage")]
        public virtual string Reused { get; set; }

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
