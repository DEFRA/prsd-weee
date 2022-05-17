namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;

    public class ReviewSubmittedManageEvidenceNotesSchemeViewModel : ManageManageEvidenceNoteSchemeViewModel
    {
        public Guid? SelectedId { get; set; }

        public ReviewSubmittedManageEvidenceNotesSchemeViewModel()
            : base(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence)
        {
        }
    }
}