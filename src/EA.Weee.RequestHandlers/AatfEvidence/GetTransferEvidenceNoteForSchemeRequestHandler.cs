namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using System.Security;
    using System.Threading.Tasks;

    public class GetTransferEvidenceNoteForSchemeRequestHandler : IRequestHandler<GetTransferEvidenceNoteForSchemeRequest, TransferEvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetTransferEvidenceNoteForSchemeRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper,
            IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<TransferEvidenceNoteData> HandleAsync(GetTransferEvidenceNoteForSchemeRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(request.EvidenceNoteId);
            var organisation = await organisationDataAccess.GetById(evidenceNote.OrganisationId);

            Condition.Requires(organisation).IsNotNull();

            authorization.EnsureOrganisationAccess(organisation.Id);

            var transferNote = mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(new TransferNoteMapTransfer(evidenceNote));

            return transferNote;
        }
    }
}