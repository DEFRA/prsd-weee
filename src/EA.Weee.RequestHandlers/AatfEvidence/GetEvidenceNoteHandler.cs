namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
    using Core.Scheme;
    using Domain.Scheme;
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

    internal class GetEvidenceNoteHandler : IRequestHandler<GetEvidenceNoteRequest, EvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess,
            IAatfDataAccess aatfDataAccess, 
            ISchemeDataAccess schemeDataAccess,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.schemeDataAccess = schemeDataAccess;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<EvidenceNoteData> HandleAsync(GetEvidenceNoteRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var evidenceNote = await evidenceDataAccess.GetNoteById(message.EvidenceNoteId);
            
            var schemeData = mapper.Map<Scheme, SchemeData>(evidenceNote.Recipient);

            var transfer = new EvidenceNoteMappingTransfer() { Note = evidenceNote, SchemeData = schemeData};

            var evidenceNoteData = mapper.Map<EvidenceNoteMappingTransfer, EvidenceNoteData>(transfer);

            return evidenceNoteData;
        }
    }
}