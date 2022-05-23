namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class EditDraftReturnedNotesViewModel : ManageManageEvidenceNoteOverviewViewModel
    {
        public EditDraftReturnedNotesViewModel()
         : base(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)
        {
        }
    }
}
