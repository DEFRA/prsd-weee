namespace EA.Weee.Requests.AatfEvidence
{
    using System;

    [Serializable]
    public class GetEvidenceNoteForSchemeRequest : GetEvidenceNoteBaseRequest
    {
        public GetEvidenceNoteForSchemeRequest(Guid evidenceNoteId) : base(evidenceNoteId)
        {
        }
    }
}
