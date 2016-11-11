namespace EA.Weee.Core.DataReturns
{
    using System;

    public class DataReturnSubmissionsHistoryData
    {
        public Guid SchemeId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid DataReturnUploadId { get; set; }

        public Guid DataReturnVersionId { get; set; }

        public Guid? PreviousSubmissionDataReturnVersionId { get; set; }

        public int ComplianceYear { get; set; }

        public QuarterType Quarter { get; set; }

        public DateTime SubmissionDateTime { get; set; }

        public string SubmittedBy { get; set; }

        public string FileName { get; set; }

        public decimal? WeeeCollectedB2c { get; set; }

        public decimal? WeeeCollectedB2b { get; set; }

        public decimal? WeeeDeliveredB2c { get; set; }

        public decimal? WeeeDeliveredB2b { get; set; }

        public decimal? EeeOutputB2c { get; set; }

        public decimal? EeeOutputB2b { get; set; }

        public bool EeeDataChanged { get; set; }
    }
}
