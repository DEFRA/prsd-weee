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

    public class GetEvidenceNoteByPBSOrganisationRequestHandler : IRequestHandler<GetEvidenceNoteByPBSOrganisationRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteByPBSOrganisationRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetEvidenceNoteByPBSOrganisationRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.OrganisationId);

            Guid? organisationId = request.OrganisationId;

            var filter = new NoteFilter(DateTime.Now.Year)
            {
                NoteTypeFilter = request.NoteTypeFilterList.Select(x => x.ToDomainEnumeration<NoteType>()).ToList(),
                OrganisationId = organisationId,
                AllowedStatuses = request.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList(),
                ComplianceYear = request.ComplianceYear
            };

            var notes = await noteDataAccess.GetAllNotes(filter);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList(), false)).ListOfEvidenceNoteData;
        }
    }
}
