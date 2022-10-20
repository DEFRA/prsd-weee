namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using Domain.Evidence;

    public abstract class EvidenceNoteWitheCriteriaMapperBase
    {
        public List<int> CategoryFilter { get; set; }

        public bool IncludeTotal { get; set; }

        public Note Note { get; private set; }

        protected EvidenceNoteWitheCriteriaMapperBase(Note note)
        {
            Note = note;
        }
    }
}
