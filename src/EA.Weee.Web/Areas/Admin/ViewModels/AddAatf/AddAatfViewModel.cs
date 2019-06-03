namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Validation;
    using EA.Weee.Web.Areas.Admin.ViewModels.Validation;
    using FluentValidation.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [Validator(typeof(AatfViewModelValidator))]
    public class AddAatfViewModel : AatfViewModelBase
    {
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        private string aatfName;

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "Name of AATF")]
        public string AatfName
        {
            get => this.aatfName;

            set
            {
                this.aatfName = value;

                this.SiteAddressData.Name = value;
            }
        }

        public AatfAddressData SiteAddressData { get; set; }

        public AatfContactData ContactData { get; set; }

        public IEnumerable<Int16> ComplianceYearList => new List<Int16> {(Int16)2019};

        public AddAatfViewModel()
        {
            this.ContactData = new AatfContactData();
            this.SiteAddressData = new AatfAddressData();
        }
    }
}