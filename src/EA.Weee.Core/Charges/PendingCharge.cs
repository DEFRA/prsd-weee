namespace EA.Weee.Core.Charges
{
    using System;

    /// <summary>
    /// Provides summary details about a pending charge for a scheme and compliance year.
    /// </summary>
    public class PendingCharge
    {
        public string SchemeName { get; set; }

        public string SchemeApprovalNumber { get; set; }

        public int ComplianceYear { get; set; }

        public decimal TotalGBP { get; set; }

        public DateTime SubmittedDate { get; set; }
    }
}
