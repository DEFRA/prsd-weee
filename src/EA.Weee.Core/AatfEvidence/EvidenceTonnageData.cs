namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using DataReturns;

    [Serializable]
    public class EvidenceTonnageData : EvidenceTonnageDataBase
    {
        public EvidenceTonnageData(Guid id, WeeeCategory categoryId, decimal? received, decimal? reused)
        {
            Id = id;
            CategoryId = categoryId;
            Received = received;
            Reused = reused;
        }
    }
}
