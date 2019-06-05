namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;

    public class AatfEditDetailsViewModel : FacilityViewModelBase
    {
        public AatfEditDetailsViewModel()
        {
            FacilityType = FacilityType.Aatf;
            this.SiteAddressData = new AatfAddressData();
        }

        [RegularExpression(@"WEE/([A-Z]{2}[0-9]{4}[A-Z]{2})/ATF", ErrorMessage = "Approval number is not in correct format")]
        public override string ApprovalNumber { get; set; }

        public Guid Id { get; set; }

        private string aatfName;
        [Display(Name = "Name of AATF")]
        public override string Name
        {
            get => this.aatfName;

            set
            {
                this.aatfName = value;

                if (this.SiteAddressData != null)
                {
                    this.SiteAddressData.Name = value;
                }
            }
        }

        public AatfAddressData SiteAddressData { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                yield return new ValidationResult(string.Format("Enter name of {0}", this.FacilityType), new[] { "Name" });
            }
        }
    }
}