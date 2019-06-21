namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public class OverriddenFacilityViewModelBase : FacilityViewModelBase
    {
        public override string Name { get; set; }

        public override string ApprovalNumber { get; set; }
    }
}