namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
    using Requests.Scheme;

    public interface ITransferTonnagesValidator
    {
        Task Validate(TransferEvidenceNoteRequest request);
    }
}