namespace EA.Weee.Core.Charges
{
    using System.Collections.Generic;

    public class ManagePendingCharges
    {
        public IList<PendingCharge> PendingCharges { get; set; }

        public bool CanUserIssueCharges { get; set; }
    }
}
