namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;

    public class AeEditDetailsViewModel : FacilityViewModelBase
    {
        public AeEditDetailsViewModel()
        {
            FacilityType = FacilityType.Ae;
        }

        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/(EXP|AE)", ErrorMessage = "Approval number is not in correct format")]
        public override string ApprovalNumber { get; set; }

        private string aatfName;
        [Required(ErrorMessage = "Enter name of AE")]
        [Display(Name = "Name of AE")]
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult(string.Format("Enter name of {0}", FacilityType), new[] { "Name" });
            }
        }
    }
}