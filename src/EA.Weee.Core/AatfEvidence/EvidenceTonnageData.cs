namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using DataReturns;

    [Serializable]
    public class EvidenceTonnageData
    {
        public decimal? Received { get; private set; }

        public decimal? Reused { get; private set; }

        public WeeeCategory CategoryId { get; private set; }

        public Guid Id { get; private set; }

        public EvidenceTonnageData(Guid id, WeeeCategory categoryId, decimal? received, decimal? reused)
        {
            Id = id;
            CategoryId = categoryId;
            Received = received;
            Reused = reused;
        }
    }
}
