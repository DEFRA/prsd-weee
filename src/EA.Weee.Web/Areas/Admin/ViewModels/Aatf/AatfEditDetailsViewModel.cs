namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Validation;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentValidation.Attributes;

    [Validator(typeof(AatfViewModelValidator))]
    public class AatfEditDetailsViewModel : AatfViewModelBase
    {
        public AatfEditDetailsViewModel()
        {
            this.SiteAddress = new AatfAddressData();
        }

        public Guid Id { get; set; }

        private string aatfName;
        [Required]
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
    }
}