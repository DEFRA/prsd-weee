namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
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

    public class GetEvidenceNoteHandler : IRequestHandler<GetEvidenceNoteRequest, EvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IUserContext userContext;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IAatfDataAccess aatfDataAccess, 
            IUserContext userContext,
            IEvidenceDataAccess evidenceDataAccess) 
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.userContext = userContext;
            this.evidenceDataAccess = evidenceDataAccess;
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