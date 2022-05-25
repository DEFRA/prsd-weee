namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class TransferEvidenceNoteMap : IMap<Note, TransferEvidenceNoteData>
    {
        private readonly IMapper mapper;

        public TransferEvidenceNoteMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TransferEvidenceNoteData Map(Note source)
        {
            return new TransferEvidenceNoteData
            {
                Id = source.Id,
                Reference = source.Reference,
                Type = (NoteType)source.NoteType.Value,
                Status = (NoteStatus)source.Status.Value,
                ComplianceYear = source.ComplianceYear
            };
        }
    }
}
