namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;

    public class AatfEditDetailsViewModel : AatfViewModelBase
    {
        public AatfEditDetailsViewModel()
        {
            this.SiteAddress = new AatfAddressData();
        }

        public Guid Id { get; set; }

        private string aatfName;
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "Name of AATF")]
        public string Name
        {
            get => this.aatfName;

            set
            {
                this.aatfName = value;

                if (this.SiteAddress != null)
                {
                    this.SiteAddress.Name = value;
                }
            }
        }

        public AatfAddressData SiteAddress { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                yield return new ValidationResult(string.Format("Enter name of {0}", this.FacilityType), new[] { "Name" });
            }
        }
    }
}