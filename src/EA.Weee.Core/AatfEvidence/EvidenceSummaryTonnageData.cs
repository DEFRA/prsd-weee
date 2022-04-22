namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using DataReturns;

    [Serializable]
    public class EvidenceSummaryTonnageData : EvidenceTonnageDataBase
    {
        public EvidenceSummaryTonnageData(WeeeCategory categoryId, decimal? received, decimal? reused)
        {
            CategoryId = categoryId;
            Received = received;
            Reused = reused;
        }
    }
}
