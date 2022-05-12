namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using DataReturns;
    using Validation;

    [Serializable]
    public class TransferEvidenceCategoryValue : EvidenceCategoryValue
    {
        public decimal? AvailableReceived { get; set; }

        public decimal? AvailableReused { get; set; }

        public Guid TransferTonnageId { get; set; }

        [TonnageValue(nameof(CategoryId), "The tonnage value", true)]
        [TonnageCompareValue(nameof(CategoryId), nameof(AvailableReceived))]
        public sealed override string Received { get; set; }

        [TonnageValue(nameof(CategoryId), "The tonnage value", true)]
        //[TonnageCompareValue(nameof(CategoryId), nameof(Received))]
        public sealed override string Reused { get; set; }

        public TransferEvidenceCategoryValue()
        {
        }

        public TransferEvidenceCategoryValue(WeeeCategory category,
            Guid transferTonnageId,
            decimal? availableReceived, 
            decimal? availableReused, 
            string received, 
            string reused) : base(category)
        {
            TransferTonnageId = transferTonnageId;
            AvailableReceived = availableReceived;
            AvailableReused = availableReused;
            Received = received;
            Reused = reused;
        }
    }
}
