namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class AllOtherManageEvidenceNotesViewModel : ManageManageEvidenceNoteOverviewViewModel
    {
        public AllOtherManageEvidenceNotesViewModel()
        : base(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)
        {
        }
    }
}
