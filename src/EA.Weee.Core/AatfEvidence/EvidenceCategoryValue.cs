namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using AatfReturn;
    using DataReturns;
    using Validation;

    [Serializable]
    public class EvidenceCategoryValue : CategoryValue
    {
        [TonnageValue(nameof(CategoryId), "The total amount received in tonnes")]
        public string Received { get; set; }

        [TonnageValue(nameof(CategoryId), "The amount in tonnes that has been re-used as whole appliances")]
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
