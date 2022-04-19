namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System;
    using System.Collections.Generic;

    public class ReviewSubmittedEvidenceNotesViewModel : ManageEvidenceNoteViewModel
    {
        public Guid? SelectedId { get; set; }

        public IList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        public ReviewSubmittedEvidenceNotesViewModel()
            : base(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence)
        {
            EvidenceNotesDataList = new List<EvidenceNoteRowViewModel>();
        }
    }
}