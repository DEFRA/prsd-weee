namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;

    public class ViewAndTransferEvidenceViewModel : ManageEvidenceNoteViewModel
    {
        public Guid? SelectedId { get; set; }

        public ViewAndTransferEvidenceViewModel()
            : base(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence)
        {
        }
    }
}