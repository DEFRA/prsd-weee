namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RegisteredCompanyDetailsViewModel
    {
        [Required]
        [DisplayName("Company name")]
        public string CompanyName { get; set; }

        [DisplayName("Business trading name, if different to company name")]
        public string BusinessTradingName { get; set; }

        [Required]
        [Display(Name = "Company registration number (CRN)")]
        public string CompaniesRegistrationNumber { get; set; }
    }
}