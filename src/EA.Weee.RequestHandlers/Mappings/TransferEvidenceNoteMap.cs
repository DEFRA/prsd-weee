namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Organisations;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;
    using Scheme = Domain.Scheme.Scheme;

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
                ComplianceYear = source.ComplianceYear,
                TransferredOrganisation = mapper.Map<Organisation, OrganisationData>(source.Organisation),
                RecipientOrganisationData = mapper.Map<Organisation, OrganisationData>(source.Recipient.Organisation),
                RecipientSchemeData = mapper.Map<Scheme, SchemeData>(source.Recipient),
                TransferEvidenceNoteTonnageData = source.NoteTransferTonnage.Select(nt => new TransferEvidenceNoteTonnageData()
                {
                    EvidenceTonnageData = new EvidenceTonnageData(nt.Id, 
                        (WeeeCategory)nt.NoteTonnage.CategoryId, 
                        nt.NoteTonnage.Received,
                        nt.NoteTonnage.Reused,
                        nt.Received,
                        nt.Reused),
                    OriginalAatf = mapper.Map<Aatf, AatfData>(nt.NoteTonnage.Note.Aatf),
                    Type = (NoteType)nt.NoteTonnage.Note.NoteType.Value,
                    Reference = nt.NoteTonnage.Note.Reference
                }).ToList()
            };
        }
    }
}
