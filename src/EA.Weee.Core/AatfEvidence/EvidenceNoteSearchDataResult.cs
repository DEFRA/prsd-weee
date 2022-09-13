namespace EA.Weee.Core.AatfEvidence
{
    using System.Collections.Generic;
    using Shared.Paging;

    public class EvidenceNoteSearchDataResult
    {
        public IList<EvidenceNoteData> Results { get; set; }

        public int NoteCount { get; set; }

        public bool HasApprovedEvidenceNotes { get; set; }

        public EvidenceNoteSearchDataResult(IList<EvidenceNoteData> results, int noteCount)
        {
            Results = results;
            NoteCount = noteCount;
        }

        public EvidenceNoteSearchDataResult(IList<EvidenceNoteData> results, int noteCount, bool hasApprovedEvidenceNotes)
        {
            Results = results;
            NoteCount = noteCount;
            HasApprovedEvidenceNotes = hasApprovedEvidenceNotes;
        }

        public EvidenceNoteSearchDataResult()
        {
            Results = new PagedList<EvidenceNoteData>();
        }
    }
}
