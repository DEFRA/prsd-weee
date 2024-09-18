﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.Organisation
{
    using EA.Weee.Core.DataStandards;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class EditRegisteredCompanyOrganisationDetailsViewModel : EditOrganisationDetailsViewModelBase
    {
        [Required]
        [DisplayName("Company name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Enter company registration number (CRN)")]
        [StringLength(maximumLength: EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber, MinimumLength = 7, ErrorMessage = "The company registration number should be 7 to 15 characters long")]
        [Display(Name = "company registration number (CRN)")]
        public string CompaniesRegistrationNumber { get; set; }

        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string BusinessTradingName { get; set; }
    }
}