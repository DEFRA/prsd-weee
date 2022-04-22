namespace EA.Weee.Requests.AatfEvidence
{
    using System;

    [Serializable]
    public class GetEvidenceNoteForAatfRequest : GetEvidenceNoteBaseRequest
    {
        public GetEvidenceNoteForAatfRequest(Guid evidenceNoteId) : base(evidenceNoteId)
        {
        }
    }
}
