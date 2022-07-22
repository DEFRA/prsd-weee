namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using Mappings;
    using Prsd.Core;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NoteType = Domain.Evidence.NoteType;

    public class GetEvidenceNotesByOrganisationRequestHandler : IRequestHandler<GetEvidenceNotesByOrganisationRequest, EvidenceNoteSearchDataResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly ISchemeDataAccess schemeDataAccess;

        public GetEvidenceNotesByOrganisationRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper, 
            ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<EvidenceNoteSearchDataResult> HandleAsync(GetEvidenceNotesByOrganisationRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            var scheme = await schemeDataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId);

            Guard.ArgumentNotNull(() => scheme, scheme, $"Scheme not found for organisation with id {request.OrganisationId}");

            authorization.EnsureSchemeAccess(scheme.Id);

            Guid? organisationId = null;
            Guid? recipientId = request.OrganisationId;
            
            if (request.TransferredOut)
            {
                organisationId = request.OrganisationId;
                recipientId = null;
            }

            var filter = new NoteFilter(request.ComplianceYear, int.MaxValue, 1)
            {
                NoteTypeFilter = request.NoteTypeFilterList.Select(x => x.ToDomainEnumeration<NoteType>()).ToList(),
                RecipientId = recipientId,
                OrganisationId = organisationId,
                AllowedStatuses = request.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList()
            };

            var notes = await noteDataAccess.GetAllNotes(filter);

            var mappedResults = mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList(), false)).ListOfEvidenceNoteData;

            return new EvidenceNoteSearchDataResult(mappedResults, mappedResults.Count);
        }
    }
}
