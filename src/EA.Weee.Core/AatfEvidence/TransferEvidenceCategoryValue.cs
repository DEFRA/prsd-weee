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

        [TonnageValue(nameof(CategoryId), "The transfer received in tonnes", true)]
        [TonnageCompareValue(nameof(CategoryId), nameof(AvailableReceived), ErrorMessage = "The transfer received in tonnes must be equivalent or lower than the total received available")]
        public sealed override string Received { get; set; }

        [TonnageValue(nameof(CategoryId), "The transfer reused in tonnes", true)]
        [TonnageCompareValue(nameof(CategoryId), nameof(Received), ErrorMessage = "The transfer reused in tonnes must be equivalent or lower than the transfer received for this category")]
        [TonnageCompareValue(nameof(CategoryId), nameof(AvailableReused), ErrorMessage = "The transfer reused in tonnes must be equivalent or lower than the total reused available")]
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
