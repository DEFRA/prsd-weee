namespace EA.Weee.Web.Areas.Admin.ViewModels.Charge
{
    using Core.Charges;
    using System.Collections.Generic;

    public class ManagePendingChargesViewModel
    {
        public IList<PendingCharge> PendingCharges { get; set; }

        public bool CanUserIssueCharges { get; set; }
    }
}