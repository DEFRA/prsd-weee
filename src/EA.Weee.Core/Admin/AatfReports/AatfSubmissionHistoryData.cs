namespace EA.Weee.Core.Admin.AatfReports
{
    using System;
    using DataReturns;
    using Shared;

    public class AatfSubmissionHistoryData
    {
        public virtual Guid ReturnId { get; set; }

        public QuarterType Quarter { get; set; }

        public int ComplianceYear { get; set; }

        public decimal? WeeeReceivedHouseHold { get; set; }

        public decimal? WeeeReceivedNonHouseHold { get; set; }

        public decimal? WeeeReusedHouseHold { get; set; }

        public decimal? WeeeReusedNonHouseHold { get; set; }

        public decimal? WeeeSentOnHouseHold { get; set; }

        public decimal? WeeeSentOnNonHouseHold { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime SubmittedDate { get; set; }
    }
}
