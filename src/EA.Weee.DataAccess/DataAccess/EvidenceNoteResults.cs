namespace EA.Weee.DataAccess.DataAccess
{
    using System.Collections.Generic;
    using Domain.Evidence;

    public class EvidenceNoteResults
    {
        public IList<Note> Notes { get; private set; }

        public int NumberOfResults { get; private set; }

        public EvidenceNoteResults(IList<Note> notes, int numberOfResults)
        {
            Notes = notes;
            NumberOfResults = numberOfResults;
        }
    }
}
