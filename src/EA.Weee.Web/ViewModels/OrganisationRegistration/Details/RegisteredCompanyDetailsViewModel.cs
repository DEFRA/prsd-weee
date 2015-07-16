namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RegisteredCompanyDetailsViewModel
    {
        public Guid? OrganisationId { get; set; }

        [Required]
        [DisplayName("Company name")]
        public string CompanyName { get; set; }

        [DisplayName("Business trading name, if different to company name")]
        public string BusinessTradingName { get; set; }

        [Required]
        [StringLength(maximumLength: 8, MinimumLength = 7, ErrorMessage = "The Company registration number should be 7 or 8 characters long")]
        [Display(Name = "Company registration number (CRN)")]
        public string CompaniesRegistrationNumber { get; set; }
    }
}