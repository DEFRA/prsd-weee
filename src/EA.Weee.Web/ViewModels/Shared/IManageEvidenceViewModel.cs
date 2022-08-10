namespace EA.Weee.Web.ViewModels.Shared
{
    using Core.Shared.Paging;

    public interface IManageEvidenceViewModel
    {
        PagedList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }
    }
}