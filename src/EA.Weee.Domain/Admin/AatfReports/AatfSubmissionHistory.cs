namespace EA.Weee.Domain.Admin.AatfReports
{
    using System;

    public class AatfSubmissionHistory
    {
        public virtual Guid ReturnId { get; set; }

        public int Quarter { get; set; }

        public int ComplianceYear { get; set; }

        public decimal? WeeeReceivedHouseHold { get; set; }

        public decimal? WeeeReceivedNonHouseHold { get; set; }

        public decimal? WeeeReusedHouseHold { get; set; }

        public decimal? WeeeReusedHouseNonHold { get; set; }

        public decimal? WeeeSentOnHouseHold { get; set; }

        public decimal? WeeeSentOnNonHouseHold { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime SubmittedDate { get; set; }
    }
}
