namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using EA.Weee.Core.Validation;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentValidation.Attributes;
    using System.ComponentModel.DataAnnotations;
    using Core.AatfReturn;

    [Validator(typeof(AatfViewModelValidator))]
    public class AddAatfViewModel : AatfViewModelBase, AddFacilityViewModelBase
    {
        private string aatfName;

        [Required(ErrorMessage = "Enter name of AATF")]
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

        public AddAatfViewModel()
        {
            FacilityType = FacilityType.Aatf;
        }
    }
}