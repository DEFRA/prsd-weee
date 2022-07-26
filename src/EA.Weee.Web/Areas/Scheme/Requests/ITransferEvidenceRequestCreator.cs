namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using ViewModels;
    using Weee.Requests.Scheme;

    public interface ITransferEvidenceRequestCreator
    {
        TransferEvidenceNoteRequest SelectCategoriesToRequest(TransferEvidenceNoteCategoriesViewModel viewModel, TransferEvidenceNoteRequest existingEvidenceNoteRequest = null);

        TransferEvidenceNoteRequest SelectTonnageToRequest(TransferEvidenceNoteRequest request, TransferEvidenceTonnageViewModel viewModel);

        EditTransferEvidenceNoteRequest EditSelectTonnageToRequest(TransferEvidenceNoteRequest request,
            TransferEvidenceTonnageViewModel viewModel);
    }
}