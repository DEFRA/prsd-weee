namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetEvidenceNoteBaseRequest : IRequest<EvidenceNoteData>
    {
        public Guid EvidenceNoteId { get; private set; }

        public GetEvidenceNoteBaseRequest(Guid evidenceNoteId)
        {
            Guard.ArgumentNotDefaultValue(() => evidenceNoteId, evidenceNoteId);

            EvidenceNoteId = evidenceNoteId;
        }
    }
}
