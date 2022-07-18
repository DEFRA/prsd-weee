namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using System.Collections.Generic;
    using Domain.Evidence;

    public class EvidenceNoteWithCriteriaMap
    {
        public Note Note { get; }

        public List<int> CategoryFilter { get; set; }

        public bool IncludeTonnage { get; set; }

        public DateTime? SystemDateTime { get; set; }

        public EvidenceNoteWithCriteriaMap(Note note)
        {
            Note = note;
            CategoryFilter = new List<int>();
        }
    }
}
