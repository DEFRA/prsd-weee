namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetEvidenceNoteRequest : IRequest<EvidenceNoteData>
    {
        public Guid EvidenceNoteId { get; private set; }
        
        public GetEvidenceNoteRequest(Guid evidenceNoteId)
        {
            Guard.ArgumentNotDefaultValue(() => evidenceNoteId, evidenceNoteId);

            EvidenceNoteId = evidenceNoteId;
        }
    }
}
