namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.DataReturns;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;

    public class EvidenceNoteMap : IMap<EvidenceNoteMappingTransfer, EvidenceNoteData>
    {
        private readonly IMap<Address, AddressData> addressMap;
        private readonly IMapper mapper;

        public EvidenceNoteMap(IMap<Address, AddressData> addressMap, IMapper mapper)
        {
            this.addressMap = addressMap;
            this.mapper = mapper;
        }

        public EvidenceNoteData Map(EvidenceNoteMappingTransfer source)
        {
            return new EvidenceNoteData(source.SchemeData, source.AatfData)
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
                SchemeData  = mapper.Map<Scheme, SchemeData>(source.Note.Recipient),
                OrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Organisation)
            };
        }
    }
}
