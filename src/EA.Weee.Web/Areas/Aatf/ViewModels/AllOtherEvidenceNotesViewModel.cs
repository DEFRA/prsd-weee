namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;

    public class AllOtherEvidenceNotesViewModel : ManageEvidenceNoteOverviewViewModel
    {
        public AllOtherEvidenceNotesViewModel()
        : base(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)
        {
            ListOfNotes = new List<EvidenceNoteRowViewModel>();
        }
    }
}
