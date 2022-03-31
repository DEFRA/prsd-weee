namespace EA.Weee.Core.AatfEvidence
{
    using System;

    [Serializable]
    public class EvidenceTonnageData
    {
        public decimal? Received { get; private set; }

        public decimal? Reused { get; private set; }

        public int CategoryId { get; private set; }

        public Guid Id { get; private set; }

        public EvidenceTonnageData(Guid id, int categoryId, decimal? received, decimal? reused)
        {
            Id = id;
            CategoryId = categoryId;
            Received = received;
            Reused = reused;
        }
    }
}
