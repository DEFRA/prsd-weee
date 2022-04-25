namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.ViewModels;

    public interface ICreateTransferEvidenceNoteRequestCreator
    {
        TransferEvidenceNoteRequest ViewModelToRequest(TransferEvidenceNoteDataViewModel viewModel);
    }
}
