namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.Evidence;
    using System.Linq;

    public class EvidenceNoteDataMap : IMap<ListOfNotesMap, ListOfEvidenceNoteDataMap>
    {
        private readonly IMapper mapper;

        public EvidenceNoteDataMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ListOfEvidenceNoteDataMap Map(ListOfNotesMap source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ListOfEvidenceNoteDataMap();

            if (source.ListOfNotes.Any())
            {
                foreach (var note in source.ListOfNotes)
                { 
                    var evidenceNoteData = mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(new EvidenceNoteWithCriteriaMap(note)
                    {
                        CategoryFilter = source.CategoryFilter,
                        IncludeTonnage = source.IncludeTonnage
                    });
                    model.ListOfEvidenceNoteData.Add(evidenceNoteData);
                }
            }
            return model;
        }
    }
}
