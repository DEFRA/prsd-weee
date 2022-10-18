namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using Domain.Evidence;

    public class EvidenceNoteRowCriteriaMapper : EvidenceNoteWitheCriteriaMapperBase
    {
        public EvidenceNoteRowCriteriaMapper(Note note) : base(note)
        {
            CategoryFilter = new List<int>();
        }
    }
}
