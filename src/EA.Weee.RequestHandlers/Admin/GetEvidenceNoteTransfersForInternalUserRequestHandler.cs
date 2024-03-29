﻿namespace EA.Weee.RequestHandlers.Admin
{
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

        public GetEvidenceNoteTransfersForInternalUserRequestHandler(IWeeeAuthorization authorization,
          IEvidenceDataAccess evidenceDataAccess,
          IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<TransferEvidenceNoteData> HandleAsync(GetEvidenceNoteTransfersForInternalUserRequest request)
        {
            authorization.EnsureCanAccessInternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(request.EvidenceNoteId);

            return mapper.Map<TransferNoteMapTransfer, TransferEvidenceNoteData>(new TransferNoteMapTransfer(evidenceNote));
        }
    }
}
