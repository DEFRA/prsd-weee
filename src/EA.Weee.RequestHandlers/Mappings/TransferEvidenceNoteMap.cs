namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Organisations;
    using Core.Scheme;
    using CuttingEdge.Conditions;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;
    using Scheme = Domain.Scheme.Scheme;

    public class TransferEvidenceNoteMap : IMap<TransferNoteMapTransfer, TransferEvidenceNoteData>
    {
        private readonly IMapper mapper;

        public TransferEvidenceNoteMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TransferEvidenceNoteData Map(TransferNoteMapTransfer source)
        {
            Condition.Requires(source.Note).IsNotNull();
            Condition.Requires(source.Scheme).IsNotNull();

            return new TransferEvidenceNoteData
            {
                Id = source.Note.Id,
                Reference = source.Note.Reference,
                Type = (NoteType)source.Note.NoteType.Value,
                Status = (NoteStatus)source.Note.Status.Value,
                ComplianceYear = source.Note.ComplianceYear,
                TransferredOrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Organisation),
                RecipientOrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Recipient.Organisation),
                RecipientSchemeData = mapper.Map<Scheme, SchemeData>(source.Note.Recipient),
                TransferredSchemeData = mapper.Map<Scheme, SchemeData>(source.Scheme),
                TransferEvidenceNoteTonnageData = source.Note.NoteTransferTonnage.Select(nt => new TransferEvidenceNoteTonnageData()
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
