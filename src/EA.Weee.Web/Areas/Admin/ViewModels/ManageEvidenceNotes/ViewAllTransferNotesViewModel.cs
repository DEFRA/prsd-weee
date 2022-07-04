namespace EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;

    public class ViewAllTransferNotesViewModel : ManageEvidenceNotesViewModel
    {
        public ViewAllTransferNotesViewModel()
           : base(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers)
        {
        }
    }
}