namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System;

    public class ReviewEvidenceNoteViewModel
    {
        public Guid? SelectedId { get; set; }

        public ViewEvidenceNoteViewModel ViewEvidenceNoteViewModel { get; set; }

        public EvidenceNoteApprovalOptionsViewModel EvidenceNoteApprovalOptionsViewModel { get; set; }

        public ReviewEvidenceNoteViewModel()
        {
        }
    }
}