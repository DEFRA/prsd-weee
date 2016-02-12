namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using Core.Organisations;
    using Core.Shared;

    public class EditRegisteredCompanyOrganisationDetailsViewModel
    {
        public Guid SchemeId { get; set; }

        public Guid OrgId { get; set; }

        public OrganisationType OrganisationType { get; set; }

        [Required]
        [DisplayName("Company name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Enter company registration number (CRN)")]
        [StringLength(maximumLength: EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber, MinimumLength = 7, ErrorMessage = "The Company registration number should be 7 to 15 characters long")]
        [Display(Name = "Company registration number (CRN)")]
        public string CompaniesRegistrationNumber { get; set; }

        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string BusinessTradingName { get; set; }
        
        public AddressData BusinessAddress { get; set; }
    }
}