namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.Evidence;
    using System.Linq;
    using EA.Weee.Core.Helpers;

    public class ListOfEvidenceNoteDataMap : IMap<List<Note>, List<EvidenceNoteData>>
    {
        private readonly IMapper mapper;

        public ListOfEvidenceNoteDataMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<EvidenceNoteData> Map(List<Note> source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return source.Select(s =>
                mapper.Map<EvidenceNoteRowCriteriaMapper, EvidenceNoteData>(new EvidenceNoteRowCriteriaMapper(s)
                {
                    IncludeTotal = true,
                    CategoryFilter = s.NoteTonnage.Select(c => c.CategoryId.ToInt()).ToList()
                })).ToList();
        }
    }
}
