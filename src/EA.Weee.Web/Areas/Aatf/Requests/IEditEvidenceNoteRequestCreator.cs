namespace EA.Weee.Web.Areas.Aatf.Requests
{
    using ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;

    public interface IEditEvidenceNoteRequestCreator
    {
        EditEvidenceNoteRequest ViewModelToRequest(EditEvidenceNoteViewModel viewModel);
    }
}