namespace EA.Weee.Core.DataReturns
{
    using System;
    using DataReturns;

    public class DataReturnSubmissionsHistoryResult
    {
        public Guid SchemeId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid DataReturnUploadId { get; set; }

        public int ComplianceYear { get; set; }

        public QuarterType Quarter { get; set; }

        public DateTime SubmissionDateTime { get; set; }
        
        public string SubmittedBy { get; set; }

        public string FileName { get; set; }
    }
}
