namespace EA.Weee.Web.Areas.Aatf.Requests
{
    using ViewModels;
    using Weee.Requests.AatfEvidence;

    public interface ICreateEvidenceNoteRequestCreator
    {
        CreateEvidenceNoteRequest ViewModelToRequest(EvidenceNoteViewModel viewModel);
    }
}