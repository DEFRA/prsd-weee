﻿namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.AatfEvidence;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.AatfEvidence;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NoteType = Domain.Evidence.NoteType;

    public class GetAatfNotesRequestHandler : IRequestHandler<GetAatfNotesRequest, EvidenceNoteSearchDataResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;

        public GetAatfNotesRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
        }

        public async Task<EvidenceNoteSearchDataResult> HandleAsync(GetAatfNotesRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var wasteTypeFilter = new List<Domain.Evidence.WasteType>();
            if (message.ObligationTypeFilterList != null)
            {
                foreach (var wasteType in message.ObligationTypeFilterList)
                {
                    wasteTypeFilter.Add((Domain.Evidence.WasteType)wasteType);
                }
            }

            var filter = new NoteFilter(message.ComplianceYear, message.PageSize, message.PageNumber)
            {
                AatfId = message.AatfId,
                NoteTypeFilter = new List<NoteType>() { NoteType.EvidenceNote },
                OrganisationId = message.OrganisationId,
                AllowedStatuses = message.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList(),
                SearchRef = message.SearchRef,
                RecipientId = message.RecipientId,
                WasteTypeFilter = wasteTypeFilter,
                NoteStatusId = (int?)message.NoteStatusFilter,
                StartDateSubmitted = message.StartDateSubmitted,
                EndDateSubmitted = message.EndDateSubmitted,
                ComplianceYear = message.ComplianceYear,
            };

            var noteData = await noteDataAccess.GetAllNotes(filter);

            var mappedNotes = mapper.Map<List<Note>, List<EvidenceNoteData>>(noteData.Notes.OrderByDescending(n => n.CreatedDate).ToList());

            return new EvidenceNoteSearchDataResult(mappedNotes, noteData.NumberOfResults);
        }
    }
}