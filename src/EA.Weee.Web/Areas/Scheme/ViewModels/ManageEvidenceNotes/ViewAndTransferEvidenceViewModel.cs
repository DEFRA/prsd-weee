namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System;
    using System.Collections.Generic;

    public class ViewAndTransferEvidenceViewModel : ManageEvidenceNoteViewModel
    {
        public Guid? SelectedId { get; set; }

        public IList<EditDraftReturnedNote> EvidenceNotesDataList { get; set; }

        public ViewAndTransferEvidenceViewModel()
            : base(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence)
        {
            EvidenceNotesDataList = new List<EditDraftReturnedNote>();
        }
    }
}