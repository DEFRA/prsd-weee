namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Organisations;
    using Core.Scheme;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.Organisation;
    using AddressData = Core.Shared.AddressData;
    using Scheme = Domain.Scheme.Scheme;

    public class EvidenceNoteMap : IMap<EvidenceNoteMappingTransfer, EvidenceNoteData>
    {
        private readonly IMapper mapper;

        public EvidenceNoteMap(IMap<Address, AddressData> addressMap, IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EvidenceNoteData Map(EvidenceNoteMappingTransfer source)
        {
            return new EvidenceNoteData
            {
                Reference = source.Note.Reference,
                Type = (NoteType)source.Note.NoteType.Value,
                Status = (NoteStatus)source.Note.Status.Value,
                StartDate = source.Note.StartDate,
                EndDate = source.Note.EndDate,
                Protocol = source.Note.Protocol.HasValue ? (Protocol?)source.Note.Protocol.Value : null,
                WasteType = source.Note.WasteType.HasValue ? (WasteType?)source.Note.WasteType.Value : null,
                EvidenceTonnageData = source.Note.NoteTonnage.Select(t =>
                    new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused)).ToList(),
                SchemeData = mapper.Map<Scheme, SchemeData>(source.Note.Recipient),
                OrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Organisation),
                AatfData = mapper.Map<Aatf, AatfData>(source.Note.Aatf)
            };
        }
    }
}
