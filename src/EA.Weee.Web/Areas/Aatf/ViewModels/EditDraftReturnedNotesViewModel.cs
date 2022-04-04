namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;

    public class EditDraftReturnedNotesViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public int ReferenceId { get; set; }

        public Guid Recipient { get; set; }

        public string TypeOfWaste { get; set; }

        public string Status { get; set; }

        public EditDraftReturnedNotesViewModel()
         : base(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)
        {
        }
    }
}
