namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System;
    using System.ComponentModel;
    using Web.ViewModels.Shared;

    public class ReviewEvidenceNoteViewModel
    {
        public Guid? SelectedId { get; set; }

        public ViewEvidenceNoteViewModel ViewEvidenceNoteViewModel { get; set; }

        public EvidenceNoteApprovalOptionsViewModel EvidenceNoteApprovalOptionsViewModel { get; set; }

        public ReviewEvidenceNoteViewModel()
        {
            EvidenceNoteApprovalOptionsViewModel = new EvidenceNoteApprovalOptionsViewModel();
        }
    }
}