namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using DataReturns;

    [Serializable]
    public class EvidenceTonnageData : EvidenceTonnageDataBase
    {
        public decimal? AvailableReceived
        {
            get 
            {
                var available = (Received - (TransferredReceived ?? 0));
                return available < 0 ? 0 : available; 
            }
        }

        public decimal? AvailableReused
        { 
            get 
            {
                var available = (Reused - (TransferredReused ?? 0));
                return available < 0 ? 0 : available;
            }
        }

        public decimal? TransferredReceived { get; protected set; }

        public decimal? TransferredReused { get; protected set; }

        public Guid OriginatingNoteTonnageId { get; set; }

        public EvidenceTonnageData(Guid id, WeeeCategory categoryId, decimal? received, decimal? reused, decimal? transferredReceived, decimal? transferredReused)
        {
            Id = id;
            CategoryId = categoryId;
            Received = received;
            Reused = reused;
            TransferredReceived = transferredReceived;
            TransferredReused = transferredReused;
        }
    }
}
