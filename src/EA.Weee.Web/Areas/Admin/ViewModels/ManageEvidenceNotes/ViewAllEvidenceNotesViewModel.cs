namespace EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using Web.ViewModels.Shared;

    public class ViewAllEvidenceNotesViewModel : ManageEvidenceNotesViewModel
    {
        public ViewAllEvidenceNotesViewModel()
           : base(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes)
        {
        }
    }
}