namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Core.Admin;
    using Weee.Requests.Admin.Aatf;

    public class UpdateApprovalDateViewModelMapTransfer
    {
        public AatfData AatfData { get; set; }

        public CanApprovalDateBeChangedFlags CanApprovalDateBeChangedFlags { get; set; }

        public EditAatfDetails Request { get; set; }
    }
}