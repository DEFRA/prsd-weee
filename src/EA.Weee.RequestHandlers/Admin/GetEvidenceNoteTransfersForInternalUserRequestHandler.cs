namespace EA.Weee.RequestHandlers.Admin
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Admin;
    using Mappings;
    using Prsd.Core.Mediator;
    using Security;
    using System.Threading.Tasks;

    public class GetEvidenceNoteTransfersForInternalUserRequestHandler : IRequestHandler<GetEvidenceNoteTransfersForInternalUserRequest, TransferEvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly ISchemeDataAccess schemeDataAccess;

        public GetEvidenceNoteTransfersForInternalUserRequestHandler(IWeeeAuthorization authorization,
          IEvidenceDataAccess evidenceDataAccess,
          IMapper mapper,
          ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
            this.schemeDataAccess = schemeDataAccess;
        }
        public async Task<TransferEvidenceNoteData> HandleAsync(GetEvidenceNoteTransfersForInternalUserRequest request)
        {
            authorization.EnsureCanAccessInternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(request.EvidenceNoteId);

            var transferredScheme = await schemeDataAccess.GetSchemeOrDefaultByOrganisationId(evidenceNote.OrganisationId);

            Condition.Requires(transferredScheme).IsNotNull();

            return mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(new TransferNoteMapTransfer(transferredScheme, evidenceNote));
        }
    }
}
