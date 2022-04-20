namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;

    public class EditDraftReturnedNotesViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public EditDraftReturnedNotesViewModel()
         : base(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)
        {
            ListOfNotes = new List<EvidenceNoteRowViewModel>();
        }
    }
}
