namespace EA.Weee.Web.Areas.Admin.ViewModels.Charge
{
    using System.Collections.Generic;
    using Core.Charges;

    public class ManagePendingChargesViewModel
    {
        public IList<PendingCharge> PendingCharges { get; set; }

        public bool CanUserIssueCharges { get; set; }
    }
}