namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class AllOtherEvidenceNotesViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public AllOtherEvidenceNotesViewModel()
        : base(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)
        {
            EvidenceNotesDataList = new List<EvidenceNoteRowViewModel>();
        }
    }
}
