namespace EA.Weee.Core.AatfEvidence
{
    using System.Collections.Generic;
    using Shared.Paging;

    public class EvidenceNoteSearchDataResult
    {
        public IList<EvidenceNoteData> Results { get; set; }

        public int NoteCount { get; set; }

        public EvidenceNoteSearchDataResult(IList<EvidenceNoteData> results, int noteCount)
        {
            Results = results;
            NoteCount = noteCount;
        }

        public EvidenceNoteSearchDataResult()
        {
            Results = new PagedList<EvidenceNoteData>();
        }
    }
}
