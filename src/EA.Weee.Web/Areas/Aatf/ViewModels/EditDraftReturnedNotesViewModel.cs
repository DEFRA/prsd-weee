namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class EditDraftReturnedNotesViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public EditDraftReturnedNotesViewModel()
         : base(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)
        {
        }
    }
}
