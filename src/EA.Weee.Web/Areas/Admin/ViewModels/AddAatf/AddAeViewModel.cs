namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System.ComponentModel.DataAnnotations;
    using Core.AatfReturn;

    public class AddAeViewModel : AddFacilityViewModelBase
    {
        private string aatfName;

        [Required(ErrorMessage = "Enter name of AE")]
        [Display(Name = "Name of AE")]
        public override string Name
        {
            get => aatfName;

            set
            {
                aatfName = value;
                SiteAddressData.Name = value;
            }
        }

        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/(EXP|AE)", ErrorMessage = "Approval number is not in correct format")]
        public override string ApprovalNumber { get; set; }

        public AddAeViewModel()
        {
            FacilityType = FacilityType.Ae;
        }
    }
}