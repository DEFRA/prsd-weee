namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System;
    using System.ComponentModel;

    public class ReviewEvidenceNoteViewModel
    {
        public Guid? SelectedId { get; set; }

        public ViewEvidenceNoteViewModel ViewEvidenceNoteViewModel { get; set; }

        [DisplayName("Date Submitted")]
        public string SubmittedDate { get; set; }

        public EvidenceNoteApprovalOptionsViewModel EvidenceNoteApprovalOptionsViewModel { get; set; }
    }
}