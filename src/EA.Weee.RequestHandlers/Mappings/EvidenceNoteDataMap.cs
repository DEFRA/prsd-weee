namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain.Evidence;
    using System.Linq;
    using Scheme = Domain.Scheme.Scheme;

    public class EvidenceNoteDataMap : IMap<ListOfNotesMap, ListOfEvidenceNoteDataMap>
    {
        private readonly IMapper mapper;

        public EvidenceNoteDataMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ListOfEvidenceNoteDataMap Map(ListOfNotesMap source)
        {
            var model = new ListOfEvidenceNoteDataMap();

            if (source != null && source.ListOfNotes.Any())
            {
                foreach (var note in source.ListOfNotes)
                { 
                    var schemeData = mapper.Map<Scheme, SchemeData>(note.Recipient);
                    var evidenceNoteData = mapper.Map<Note, EvidenceNoteData>(note);
                    evidenceNoteData.SchemeData = schemeData;
                    evidenceNoteData.SubmittedBy = note.Aatf != null ? note.Aatf.Organisation.Name : string.Empty;
                    model.ListOfEvidenceNoteData.Add(evidenceNoteData);
                }
            }
            return model;
        }
    }
}
