namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;

    public class GetEvidenceNoteForSchemeRequestHandler : IRequestHandler<GetEvidenceNoteForSchemeRequest, EvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetEvidenceNoteForSchemeRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<EvidenceNoteData> HandleAsync(GetEvidenceNoteForSchemeRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            
            var evidenceNote = await evidenceDataAccess.GetNoteById(message.EvidenceNoteId);

            authorization.EnsureProducerBalancingSchemeAccess(evidenceNote.Recipient);
            
            var evidenceNoteData = mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(new EvidenceNoteWithCriteriaMap(evidenceNote)
            {
                IncludeTonnage = true
            });

            return evidenceNoteData;
        }
    }
}