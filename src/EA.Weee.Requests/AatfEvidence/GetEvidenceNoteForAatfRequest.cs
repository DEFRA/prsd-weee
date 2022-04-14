namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetEvidenceNoteForAatfRequest : GetEvidenceNoteBaseRequest
    {
        public GetEvidenceNoteForAatfRequest(Guid evidenceNoteId) : base(evidenceNoteId)
        {
        }
    }
}
