namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using Core.DataReturns;

    public class AatfSubmissionHistoryViewModel
    {
        public virtual Guid ReturnId { get; set; }

        public QuarterType Quarter { get; set; }

        public int ComplianceYear { get; set; }

        public string SubmittedBy { get; set; }

        public string SubmittedDate { get; set; }

        public string ObligatedHouseHoldTotal { get; set; }

        public string ObligatedNonHouseHoldTotal { get; set; }
    }
}