namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Aatf;

    public class TransferTonnageValue : TonnageValues
    {
        public virtual Guid TransferTonnageId { get; private set; }

        public TransferTonnageValue(Guid id, int categoryId, decimal? firstTonnage, decimal? secondTonnage, Guid transferTonnageId) : 
            base(id, categoryId, firstTonnage, secondTonnage)
        {
            TransferTonnageId = transferTonnageId;
        }
    }
}
