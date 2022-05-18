namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;

    internal class GetEvidenceNoteRequestHandler : IRequestHandler<GetEvidenceNoteForAatfRequest, EvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<EvidenceNoteData> HandleAsync(GetEvidenceNoteForAatfRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(message.EvidenceNoteId);

            authorization.EnsureOrganisationAccess(evidenceNote.OrganisationId);

            var evidenceNoteData = mapper.Map<Note, EvidenceNoteData>(evidenceNote);

            return evidenceNoteData;
        }
    }
}