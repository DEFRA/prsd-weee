namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;

    public class ReviewSubmittedEvidenceNotesViewModel : ManageEvidenceNoteViewModel
    {
        public Guid? SelectedId { get; set; }

        public ReviewSubmittedEvidenceNotesViewModel()
            : base(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence)
        {
        }
    }
}