﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Helpers;
    using Core.Organisations;
    using Core.Scheme;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.Organisation;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;
    using Protocol = Core.AatfEvidence.Protocol;
    using Scheme = Domain.Scheme.Scheme;

    public class EvidenceNoteMap : EvidenceNoteDataMapBase<EvidenceNoteData>, IMap<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>
    {
        private readonly IMapper mapper;

        public EvidenceNoteMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EvidenceNoteData Map(EvidenceNoteWithCriteriaMapper source)
        {
            var data = MapCommonProperties(source.Note);

            var noteTonnage = new List<NoteTonnage>();
            if (source.IncludeTonnage)
            {
                noteTonnage = source.CategoryFilter.Any() ? source.Note.FilteredNoteTonnage(source.CategoryFilter).ToList() : source.Note.NoteTonnage.ToList();
            }

            MapTonnageAvailable(source, data);

            data.StartDate = source.Note.StartDate;
            data.EndDate = source.Note.EndDate;
            data.Protocol = source.Note.Protocol.HasValue ? (Protocol?)source.Note.Protocol.Value : null;
            data.EvidenceTonnageData = noteTonnage.Select(t =>
                new EvidenceTonnageData(t.Id, (WeeeCategory)t.CategoryId, t.Received, t.Reused,
                    t.NoteTransferTonnage?.Where(ntt => !excludedStatus.Contains(ntt.TransferNote.Status))
                        .Select(ntt => ntt.Received).Sum(),
                    t.NoteTransferTonnage?.Where(ntt => !excludedStatus.Contains(ntt.TransferNote.Status))
                        .Select(ntt => ntt.Reused).Sum())).ToList();
            data.RecipientSchemeData = mapper.Map<Scheme, SchemeData>(source.Note.Recipient.Scheme);
            data.OrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Organisation);
            data.AatfData = source.SystemDateTime.HasValue ? mapper.Map<AatfWithSystemDateMapperObject, AatfData>(new AatfWithSystemDateMapperObject(source.Note.Aatf, source.SystemDateTime.Value)) : mapper.Map<AatfSimpleMapObject, AatfData>(new AatfSimpleMapObject(source.Note.Aatf));
            data.RecipientOrganisationData = mapper.Map<Organisation, OrganisationData>(source.Note.Recipient);
            if (source.Note.Organisation.Scheme != null)
            {
                data.OrganisationSchemaData = mapper.Map<Scheme, SchemeData>(source.Note.Organisation.Scheme);
            }
            data.RecipientId = source.Note.Recipient.Id;

            var history = new List<EvidenceNoteHistoryData>();
            if (source.IncludeHistory)
            {
                var transferredNotes = source.Note.NoteTonnage.SelectMany(nt => nt.NoteTransferTonnage)
                    .Select(nt1 => nt1.TransferNote).Distinct().ToList();

                history = transferredNotes.Select(n => new EvidenceNoteHistoryData(n.Id, n.Status.ToCoreEnumeration<NoteStatus>(), n.Reference, n.NoteType.ToCoreEnumeration<NoteType>(), 
                    n.NoteStatusHistory
                    .Where(n1 => n1.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Submitted))
                    .OrderByDescending(n1 => n1.ChangedDate).FirstOrDefault()?.ChangedDate, n.Recipient.Scheme != null ? n.Recipient.Scheme.SchemeName : string.Empty,
                    source.Note.NoteTonnage.SelectMany(nt => nt.NoteTransferTonnage).Distinct().ToList().Where(x => x.TransferNote.Status.ToCoreEnumeration<NoteStatus>().Equals(NoteStatus.Approved) && x.TransferNoteId == n.Id)
                    .Select(y => new EvidenceTonnageData(y.Id, (WeeeCategory)y.NoteTonnage.CategoryId, y.Received, y.Reused, null, null)).ToList())).ToList().OrderByDescending(n => n.Reference).ToList();
            }
            data.EvidenceNoteHistoryData = history;

            return data;
        }
    }
}
