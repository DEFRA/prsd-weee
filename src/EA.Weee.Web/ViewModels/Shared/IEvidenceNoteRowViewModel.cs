namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Collections.Generic;
    using Areas.Aatf.ViewModels;

    public interface IEvidenceNoteRowViewModel
    {
        IList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }
    }
}