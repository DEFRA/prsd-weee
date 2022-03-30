namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    public class EditDraftReturnedNotesViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public int ReferenceId { get; set; }

        public string Recipient { get; set; }

        public string TypeOfWaste { get; set; }

        public string Status { get; set; }

        public EditDraftReturnedNotesViewModel()
         : base(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)
        {
        }
    }
}
