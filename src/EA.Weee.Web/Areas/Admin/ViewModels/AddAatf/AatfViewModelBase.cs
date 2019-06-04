namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System.ComponentModel.DataAnnotations;
    using Core.AatfReturn;

    public abstract class AatfViewModelBase : FacilityViewModelBase
    {
        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/ATF", ErrorMessage = "Approval number is not in correct format")]
        public override string ApprovalNumber { get; set; }

        public AatfViewModelBase()
        {
            FacilityType = FacilityType.Aatf;
        }
    }
}