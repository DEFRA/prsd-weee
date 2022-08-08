namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;

    public class EvidenceNoteHistoryDataViewModelMapTransfer
    {
        public IList<EvidenceNoteHistoryData> NoteHistoryData { get; set; }

        public EvidenceNoteHistoryDataViewModelMapTransfer(IList<EvidenceNoteHistoryData> noteHistoryData)
        {
            NoteHistoryData = noteHistoryData;
        }
    }
}