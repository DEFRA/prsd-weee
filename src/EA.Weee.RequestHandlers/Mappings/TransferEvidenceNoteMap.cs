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

    public class TransferEvidenceNoteMap : EvidenceNoteDataMapBase<TransferEvidenceNoteData>, IMap<TransferNoteMapTransfer, TransferEvidenceNoteData>
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

            var data = MapCommonProperties(source.Note);

            data.TransferredOrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Organisation);
            data.RecipientOrganisationData =
                mapper.Map<Organisation, OrganisationData>(source.Note.Recipient.Organisation);
            data.RecipientSchemeData = mapper.Map<Scheme, SchemeData>(source.Note.Recipient);
            data.TransferredSchemeData = mapper.Map<Scheme, SchemeData>(source.Scheme);
            data.TransferEvidenceNoteTonnageData = source.Note.NoteTransferTonnage.Select(nt =>
                new TransferEvidenceNoteTonnageData()
                {
                    EvidenceTonnageData = new EvidenceTonnageData(nt.Id,
                        (WeeeCategory)nt.NoteTonnage.CategoryId,
                        nt.NoteTonnage.Received,
                        nt.NoteTonnage.Reused,
                        nt.Received,
                        nt.Reused)
                    {
                        OriginatingNoteTonnageId = nt.NoteTonnage.Id
                    },
                    OriginalAatf = mapper.Map<Aatf, AatfData>(nt.NoteTonnage.Note.Aatf),
                    Type = (NoteType)nt.NoteTonnage.Note.NoteType.Value,
                    OriginalReference = nt.NoteTonnage.Note.Reference,
                    OriginalNoteId = nt.NoteTonnage.Note.Id
                }).ToList();

            return data;
        }
    }
}
