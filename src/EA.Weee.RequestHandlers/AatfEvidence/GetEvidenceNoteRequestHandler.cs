namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using Mappings;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;

    internal class GetEvidenceNoteRequestHandler : IRequestHandler<GetEvidenceNoteForAatfRequest, EvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<EvidenceNoteData> HandleAsync(GetEvidenceNoteForAatfRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(message.EvidenceNoteId);

            authorization.EnsureOrganisationAccess(evidenceNote.OrganisationId);

            var currentDateTime = await systemDataDataAccess.GetSystemDateTime();

            var evidenceNoteData = mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(new EvidenceNoteWithCriteriaMap(evidenceNote)
            {
                IncludeTonnage = true,
                SystemDateTime = currentDateTime,
                IncludeHistory = false
            });

            return evidenceNoteData;
        }
    }
}