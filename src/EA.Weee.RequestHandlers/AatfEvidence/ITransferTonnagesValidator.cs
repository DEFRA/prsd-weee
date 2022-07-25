namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Requests.AatfEvidence;

    public interface ITransferTonnagesValidator
    {
        Task Validate(List<TransferTonnageValue> transferValues, Guid? existingTransferNoteId = null);
    }
}