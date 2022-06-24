namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
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
    using Protocol = Core.AatfEvidence.Protocol;
    using Scheme = Domain.Scheme.Scheme;

    public class EvidenceNoteMap : EvidenceNoteDataMapBase<EvidenceNoteData>, IMap<Note, EvidenceNoteData>
    {
        private readonly IMapper mapper;

        public EvidenceNoteMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EvidenceNoteData Map(Note source)
        {
            var excludedStatuses = new List<Domain.Evidence.NoteStatus>() { Domain.Evidence.NoteStatus.Rejected, Domain.Evidence.NoteStatus.Void };

            var data = MapCommonProperties(source);

            data.StartDate = source.StartDate;
            data.EndDate = source.EndDate;
            data.Protocol = source.Protocol.HasValue ? (Protocol?)source.Protocol.Value : null;
            data.EvidenceTonnageData = source.NoteTonnage.Select(t =>
                new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused,
                    t.NoteTransferTonnage?.Where(ntt => !excludedStatuses.Contains(ntt.TransferNote.Status))
                        .Select(ntt => ntt.Received).Sum(),
                    t.NoteTransferTonnage?.Where(ntt => !excludedStatuses.Contains(ntt.TransferNote.Status))
                        .Select(ntt => ntt.Reused).Sum())).ToList();
            data.SchemeData = mapper.Map<Scheme, SchemeData>(source.Recipient);
            data.OrganisationData = mapper.Map<Organisation, OrganisationData>(source.Organisation);
            data.AatfData = mapper.Map<Aatf, AatfData>(source.Aatf);
            data.RecipientOrganisationData = mapper.Map<Organisation, OrganisationData>(source.Recipient.Organisation);
            data.RecipientId = source.Recipient.Id;

            return data;
        }
    }
}
