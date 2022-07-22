namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Collections.Generic;

    public interface IManageEvidenceViewModel
    {
        IList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }
    }
}