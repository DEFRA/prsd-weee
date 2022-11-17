namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using AatfEvidence;
    using Core.AatfEvidence;

    [Serializable]
    public class GetAllAatfsForComplianceYearRequest : EvidenceEntityIdDisplayNameDataBase
    {
        private static readonly List<NoteStatus> AllowedStatusList = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };

        public GetAllAatfsForComplianceYearRequest(int complianceYear) : base(complianceYear, AllowedStatusList)
        {
        }
    }
}
