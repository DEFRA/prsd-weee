namespace EA.Weee.RequestHandlers.Admin
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;

    public class GetEvidenceNoteForInternalUserRequestHandler : IRequestHandler<GetEvidenceNoteForInternalUserRequest, EvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteForInternalUserRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<EvidenceNoteData> HandleAsync(GetEvidenceNoteForInternalUserRequest message)
        {
            authorization.EnsureCanAccessInternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(message.EvidenceNoteId);

            var evidenceNoteData = mapper.Map<Note, EvidenceNoteData>(evidenceNote);

            return evidenceNoteData;
        }
    }
}
