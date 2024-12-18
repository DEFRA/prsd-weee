﻿namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
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

            authorization.EnsureOrganisationAccess(evidenceNote.Recipient.Id);
            
            var evidenceNoteData = mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(new EvidenceNoteWithCriteriaMapper(evidenceNote)
            {
                IncludeTonnage = true,
                IncludeHistory = true,
                IncludeTotal = false
            });

            return evidenceNoteData;
        }
    }
}