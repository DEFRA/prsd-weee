namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Threading.Tasks;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;

    public class GetTransferEvidenceNoteForSchemeRequestHandler : IRequestHandler<GetTransferEvidenceNoteForSchemeRequest, TransferEvidenceNoteData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetTransferEvidenceNoteForSchemeRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess,
            IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<TransferEvidenceNoteData> HandleAsync(GetTransferEvidenceNoteForSchemeRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await evidenceDataAccess.GetNoteById(request.EvidenceNoteId);

            //TODO: this will be moved to a mapper when doing the view transfer note story
            return new TransferEvidenceNoteData()
            {
                Id = evidenceNote.Id,
                Reference = evidenceNote.Reference,
                Type = (Core.AatfEvidence.NoteType)evidenceNote.NoteType.Value,
                Status = (Core.AatfEvidence.NoteStatus)evidenceNote.Status.Value,
            };
        }
    }
}