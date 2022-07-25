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
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NoteType = Domain.Evidence.NoteType;

    public class GetEvidenceNoteByPbsOrganisationRequestHandler : IRequestHandler<GetEvidenceNoteByPbsOrganisationRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteByPbsOrganisationRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetEvidenceNoteByPbsOrganisationRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            var filter = new NoteFilter(request.ComplianceYear)
            {
                NoteTypeFilter = request.NoteTypeFilterList.Select(x => x.ToDomainEnumeration<NoteType>()).ToList(),
                OrganisationId = request.OrganisationId,
                AllowedStatuses = request.AllowedStatuses
                    .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList()
            };

            var notes = await noteDataAccess.GetAllNotes(filter);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList(), false)).ListOfEvidenceNoteData;
        }
    }
}
