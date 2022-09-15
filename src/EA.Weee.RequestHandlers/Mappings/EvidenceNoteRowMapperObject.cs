namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using Domain.Evidence;

    public class EvidenceNoteRowMapperObject
    {
        public Note Note { get; }

        public List<int> CategoryFilter { get; set; }

        public bool IncludeTotal { get; set; }

        public EvidenceNoteRowMapperObject(Note note)
        {
            Note = note;
        }
    }
}
