﻿namespace EA.Weee.Core.AatfEvidence
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
                var availble = (Received - (TransferredReceived.HasValue ? TransferredReceived : 0));
                return availble < 0 ? 0 : availble; 
            }
        }

        public decimal? AvailableReused
        { 
            get 
            {
                var availble = (Reused - (TransferredReused.HasValue ? TransferredReused : 0));
                return availble < 0 ? 0 : availble;
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
