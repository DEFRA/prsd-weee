namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetTransferEvidenceNoteForSchemeRequest : IRequest<TransferEvidenceNoteData>
    {
        public Guid EvidenceNoteId { get; private set; }

        public GetTransferEvidenceNoteForSchemeRequest(Guid evidenceNoteId)
        {
            Guard.ArgumentNotDefaultValue(() => evidenceNoteId, evidenceNoteId);

            EvidenceNoteId = evidenceNoteId;
        }
    }
}
