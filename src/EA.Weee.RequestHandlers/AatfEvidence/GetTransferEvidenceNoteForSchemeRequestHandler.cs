namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
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
        private readonly ISchemeDataAccess schemeDataAccess;

        public GetTransferEvidenceNoteForSchemeRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper, 
            ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<TransferEvidenceNoteData> HandleAsync(GetTransferEvidenceNoteForSchemeRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(request.EvidenceNoteId);

            var transferredScheme = await schemeDataAccess.GetSchemeOrDefaultByOrganisationId(evidenceNote.OrganisationId);

            Condition.Requires(transferredScheme).IsNotNull();

            var allowedAccess = authorization.CheckOrganisationAccess(evidenceNote.OrganisationId) || authorization.CheckSchemeAccess(evidenceNote.RecipientId);

            if (!allowedAccess)
            {
                throw new SecurityException($"The user does not have access to the organisation or scheme with note ID {request.EvidenceNoteId}");
            }

            if (!allowedAccess)
            {
                throw new SecurityException($"The user does not have access to the organisation or scheme with note ID {request.EvidenceNoteId}");
            }
            var transferNote = mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(new TransferNoteMapTransfer(transferredScheme, evidenceNote));

            return transferNote;
        }
    }
}