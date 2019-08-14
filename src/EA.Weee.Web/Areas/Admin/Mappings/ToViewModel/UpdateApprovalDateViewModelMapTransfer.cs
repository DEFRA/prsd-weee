namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System;
    using Core.AatfReturn;
    using Core.Admin;
    using ViewModels.Aatf;
    using Weee.Requests.Admin.Aatf;

    public class UpdateApprovalDateViewModelMapTransfer
    {
        public AatfData AatfData { get; set; }

        public CanApprovalDateBeChangedFlags CanApprovalDateBeChangedFlags { get; set; }

        public EditAatfDetails Request { get; set; }
    }
}