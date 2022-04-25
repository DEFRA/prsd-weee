namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Organisations;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.Organisation;
    using AddressData = Core.Shared.AddressData;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;
    using Protocol = Core.AatfEvidence.Protocol;
    using Scheme = Domain.Scheme.Scheme;
    using WasteType = Core.AatfEvidence.WasteType;

    public class EvidenceNoteMap : IMap<Note, EvidenceNoteData>
    {
        private readonly IMapper mapper;

        public EvidenceNoteMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EvidenceNoteData Map(Note source)
        {
            return new EvidenceNoteData
            {
                Id = source.Id,
                Reference = source.Reference,
                Type = (NoteType)source.NoteType.Value,
                Status = (NoteStatus)source.Status.Value,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Protocol = source.Protocol.HasValue ? (Protocol?)source.Protocol.Value : null,
                WasteType = source.WasteType.HasValue ? (WasteType?)source.WasteType.Value : null,
                EvidenceTonnageData = source.NoteTonnage.Select(t =>
                    new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused)).ToList(),
                SchemeData = mapper.Map<Scheme, SchemeData>(source.Recipient),
                OrganisationData = mapper.Map<Organisation, OrganisationData>(source.Organisation),
                AatfData = mapper.Map<Aatf, AatfData>(source.Aatf),
                RecipientId = source.Recipient.Id,
                SubmittedDate = source.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Submitted))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate
            };
        }
    }
}
