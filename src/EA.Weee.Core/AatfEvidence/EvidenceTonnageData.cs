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
                var availble = (Received - TransferredReceived);
                return availble < 0 ? 0 : availble; 
            }
            protected set 
            {
                AvailableReceived = value; 
            }
        }

        public decimal? AvailableReused
        { 
            get 
            {
                var availble = (Reused - TransferredReused);
                return availble < 0 ? 0 : availble;
            }
            protected set 
            {
                AvailableReused = value; 
            }
        }

        public decimal? TransferredReceived { get; protected set; }

        public decimal? TransferredReused { get; protected set; }

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
