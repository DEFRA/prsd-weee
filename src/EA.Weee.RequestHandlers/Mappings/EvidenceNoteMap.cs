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

    public class EvidenceNoteMap : EvidenceNoteDataMapBase<EvidenceNoteData>, IMap<EvidenceNoteWithCriteriaMap, EvidenceNoteData>
    {
        private readonly IMapper mapper;

        public EvidenceNoteMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EvidenceNoteData Map(EvidenceNoteWithCriteriaMap source)
        {
            var excludedStatuses = new List<Domain.Evidence.NoteStatus>() { Domain.Evidence.NoteStatus.Rejected, Domain.Evidence.NoteStatus.Void };

            var data = MapCommonProperties(source.Note);

            var noteTonnage = new List<NoteTonnage>();
            if (source.IncludeTonnage)
            {
                noteTonnage = source.CategoryFilter.Any() ? source.Note.FilteredNoteTonnage(source.CategoryFilter).ToList() : source.Note.NoteTonnage.ToList();
            }
            
            data.StartDate = source.Note.StartDate;
            data.EndDate = source.Note.EndDate;
            data.Protocol = source.Note.Protocol.HasValue ? (Protocol?)source.Note.Protocol.Value : null;
            data.EvidenceTonnageData = noteTonnage.Select(t =>
                new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused,
                    t.NoteTransferTonnage?.Where(ntt => !excludedStatuses.Contains(ntt.TransferNote.Status))
                        .Select(ntt => ntt.Received).Sum(),
                    t.NoteTransferTonnage?.Where(ntt => !excludedStatuses.Contains(ntt.TransferNote.Status))
                        .Select(ntt => ntt.Reused).Sum())).ToList();
            data.RecipientSchemeData = mapper.Map<Scheme, SchemeData>(source.Note.Recipient);
            data.OrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Organisation);
            data.AatfData = mapper.Map<Aatf, AatfData>(source.Note.Aatf);
            data.RecipientOrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Recipient.Organisation);
            if (source.Note.Organisation.Schemes != null && source.Note.Organisation.Schemes.Any())
            {
                data.OrganisationSchemaData = mapper.Map<Scheme, SchemeData>(source.Note.Organisation.Schemes.ElementAt(0));
            }
            data.RecipientId = source.Note.Recipient.Id;

            return data;
        }
    }
}
