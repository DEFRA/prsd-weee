namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using System.Collections.Generic;
    using Domain.Evidence;

    public class EvidenceNoteRowMapperObject
    {
        public Note Note { get; }

        public EvidenceNoteRowMapperObject(Note note)
        {
            Note = note;
        }
    }
}
