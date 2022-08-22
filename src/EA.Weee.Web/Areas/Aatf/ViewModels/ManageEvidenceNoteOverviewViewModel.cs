namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using Core.Shared.Paging;
    using Web.ViewModels.Shared;

    public abstract class ManageManageEvidenceNoteOverviewViewModel : IManageEvidenceViewModel
    {
        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public PagedList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        protected ManageManageEvidenceNoteOverviewViewModel(ManageEvidenceOverviewDisplayOption activeOverviewDisplayOption)
        {
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
            EvidenceNotesDataList = new PagedList<EvidenceNoteRowViewModel>();
        }

        public ManageEvidenceOverviewDisplayOption ActiveOverviewDisplayOption { get; private set; }
    }
}
