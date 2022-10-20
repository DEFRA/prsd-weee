namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using System.Collections.Generic;
    using Domain.Evidence;

    public class EvidenceNoteWithCriteriaMapper : EvidenceNoteWitheCriteriaMapperBase
    {
        public bool IncludeTonnage { get; set; }

        public bool IncludeHistory { get; set; }

        public DateTime? SystemDateTime { get; set; }

        public EvidenceNoteWithCriteriaMapper(Note note) : base(note)
        {
            CategoryFilter = new List<int>();
        }
    }
}
