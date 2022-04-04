namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Scheme;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Mappings;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using Scheme = Domain.Scheme.Scheme;

    internal class GetEvidenceNoteHandler : IRequestHandler<GetEvidenceNoteRequest, EvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<EvidenceNoteData> HandleAsync(GetEvidenceNoteRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var evidenceNote = await evidenceDataAccess.GetNoteById(message.EvidenceNoteId);

            var transfer = new EvidenceNoteMappingTransfer() { Note = evidenceNote };

            var evidenceNoteData = mapper.Map<EvidenceNoteMappingTransfer, EvidenceNoteData>(transfer);

            return evidenceNoteData;
        }
    }
}