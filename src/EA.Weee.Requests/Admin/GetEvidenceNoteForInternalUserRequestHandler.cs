namespace EA.Weee.Requests.Admin
{
    using System;
    using EA.Weee.Requests.AatfEvidence;

    public class GetEvidenceNoteForInternalUserRequest : GetEvidenceNoteBaseRequest
    {
        public GetEvidenceNoteForInternalUserRequest(Guid evidenceNoteId) : base(evidenceNoteId)
        {
        }
    }
}
