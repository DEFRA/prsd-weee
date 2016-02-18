namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.PcsDetails
{
    using System;

    public class PcsDetailsOverviewViewModel : OverviewViewModel
    {
        public string ApprovalNumber { get; set; }

        public string BillingReference { get; set; }

        public string ObligationType { get; set; }

        public string AppropriateAuthority { get; set; }

        public string Status { get; set; }

        public bool IsRejected { get; set; }

        public bool CanEditPcs { get; set; }

        public PcsDetailsOverviewViewModel() 
            : base(OverviewDisplayOption.PcsDetails)
        {
        }
    }
}