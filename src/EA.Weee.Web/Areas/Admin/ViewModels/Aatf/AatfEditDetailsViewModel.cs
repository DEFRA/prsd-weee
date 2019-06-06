namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;

    public class AatfEditDetailsViewModel : FacilityViewModelBase
    {
        public AatfEditDetailsViewModel()
        {
            FacilityType = FacilityType.Aatf;
        }

        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/ATF", ErrorMessage = "Approval number is not in correct format")]
        public override string ApprovalNumber { get; set; }

        private string aatfName;
        [Required(ErrorMessage = "Enter name of AATF")]
        [Display(Name = "Name of AATF")]
        public override string Name
        {
            get => aatfName;

            set
            {
                aatfName = value;

                if (SiteAddressData != null)
                {
                    SiteAddressData.Name = value;
                }
            }
        }
    }
}