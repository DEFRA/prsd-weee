namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin;
    using EA.Weee.Domain.Evidence;

    public class GetAllNotesMap : IMap<ListOfNotesMap, ListOfAdminEvidenceNoteDataMap>
    {
        private readonly IMapper mapper;

        public GetAllNotesMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ListOfAdminEvidenceNoteDataMap Map(ListOfNotesMap source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ListOfAdminEvidenceNoteDataMap();

            if (source.ListOfNotes.Any())
            {
                foreach (var note in source.ListOfNotes)
                {
                    var evidenceNoteData = mapper.Map<Note, AdminEvidenceNoteData>(note);
                    model.ListOfAdminEvidenceNoteData.Add(evidenceNoteData);
                }
            }
            return model;
        }
    }
}
