namespace EA.Weee.Web.Areas.Aatf.Requests
{
    using ViewModels;
    using Weee.Requests.AatfEvidence;

    public interface IEditEvidenceNoteRequestCreator
    {
        EditEvidenceNoteRequest ViewModelToRequest(EvidenceNoteViewModel viewModel);
    }
}