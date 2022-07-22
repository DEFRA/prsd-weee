namespace EA.Weee.Core.AatfEvidence
{
    using System.Collections.Generic;

    public class EvidenceNoteDataListResult
    {
        public IList<EvidenceNoteData> Results { get; private set; }

        public int NoteCount { get; private set; }

        public EvidenceNoteDataListResult(IList<EvidenceNoteData> results, int noteCount)
        {
            Results = results;
            NoteCount = noteCount;
        }
    }
}
