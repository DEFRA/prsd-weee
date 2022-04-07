namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetSubmittedEvidenceNotesByOrganisationIdRequestHandler : IRequestHandler<GetSubmittedEvidenceNotesByOrganisationIdRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMapper mapper;

        public GetSubmittedEvidenceNotesByOrganisationIdRequestHandler(IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetSubmittedEvidenceNotesByOrganisationIdRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var allowedStatuses = new List<NoteStatus>() { NoteStatus.Submitted };

            var notes = await aatfDataAccess
                .GetAllSubmittedNotesByOrgId(message.OrganisationId, allowedStatuses.Select(x => (int)x)
                .ToList());

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes)).ListOfEvidenceNoteData;
        }
    }
}
