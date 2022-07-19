namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetEvidenceNoteTransfersForInternalUserRequest : IRequest<TransferEvidenceNoteData>
    {
        public Guid EvidenceNoteId { get; private set; }

        public GetEvidenceNoteTransfersForInternalUserRequest(Guid evidenceNoteId)
        {
            Guard.ArgumentNotDefaultValue(() => evidenceNoteId, evidenceNoteId);

            EvidenceNoteId = evidenceNoteId;
        }
    }
}
