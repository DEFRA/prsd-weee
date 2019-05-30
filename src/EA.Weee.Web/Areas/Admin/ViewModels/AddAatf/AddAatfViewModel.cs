namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System.ComponentModel.DataAnnotations;

    public class AddAatfViewModel : AddFacilityViewModelBase
    {
        private string aatfName;

        [Display(Name = "Name of AATF")]
        public override string Name
        {
            get => aatfName;

            set
            {
                aatfName = value;
                SiteAddressData.Name = value;
            }
        }

        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/ATF", ErrorMessage = "Approval number is not in correct format")]
        public override string ApprovalNumber { get; set; }
    }
}