﻿namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NoteType = Domain.Evidence.NoteType;

    public class GetEvidenceNotesByRequestHandler : IRequestHandler<GetEvidenceNotesByRequest, EvidenceNoteSearchDataResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetEvidenceNotesByRequestHandler(IWeeeAuthorization authorization, 
            IEvidenceDataAccess noteDataAccess, 
            IMapper mapper, 
            IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<EvidenceNoteSearchDataResult> HandleAsync(GetEvidenceNotesByRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisation = await organisationDataAccess.GetById(request.OrganisationId);

            authorization.EnsureOrganisationAccess(organisation.Id);

            Guid? organisationId = null;
            Guid? recipientId = request.OrganisationId;
            var hasApprovedEvidenceNotes = false;

            if (request.TransferredOut)
            {
                organisationId = request.OrganisationId;
                recipientId = request.ReceivedId;
            }
            else
            {
                hasApprovedEvidenceNotes = await noteDataAccess.HasApprovedWasteHouseHoldEvidence(recipientId.Value, request.ComplianceYear);
            }

            var wasteTypeFilter = new List<Domain.Evidence.WasteType>();
            foreach (var wasteType in request.ObligationTypeFilterList)
            {
                wasteTypeFilter.Add((Domain.Evidence.WasteType)wasteType);
            }

            var filter = new NoteFilter(request.ComplianceYear, request.PageSize, request.PageNumber)
            {
                NoteTypeFilter = request.NoteTypeFilterList.Select(x => x.ToDomainEnumeration<NoteType>()).ToList(),
                RecipientId = recipientId,
                OrganisationId = organisationId,
                AllowedStatuses = request.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList(),
                WasteTypeFilter = wasteTypeFilter
            };

            var noteData = await noteDataAccess.GetAllNotes(filter);

            var mappedNotes = mapper.Map<List<Note>, List<EvidenceNoteData>>(noteData.Notes.OrderByDescending(n => n.CreatedDate).ToList());

            return new EvidenceNoteSearchDataResult(mappedNotes, noteData.NumberOfResults, hasApprovedEvidenceNotes);
        }
    }
}
