namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using AatfReturn;
    using DataReturns;
    using Validation;

    [Serializable]
    public class EvidenceCategoryValue : CategoryValue
    {
        [TonnageValue("CategoryId", "Total received")]
        public string Received { get; set; }

        [TonnageValue("CategoryId", "Reused as whole appliances")]
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
