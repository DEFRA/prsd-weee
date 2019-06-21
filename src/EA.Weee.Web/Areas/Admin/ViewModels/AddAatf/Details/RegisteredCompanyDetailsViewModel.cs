namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details
{
    using Core.DataStandards;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RegisteredCompanyDetailsViewModel : OrganisationViewModel
    {
        [Required]
        [DisplayName("Company name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }

        [DisplayName("Business trading name, if different to company name")]
        public override string BusinessTradingName { get; set; }

        [Required(ErrorMessage = "Enter company registration number (CRN)")]
        [StringLength(maximumLength: EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber, MinimumLength = 7, ErrorMessage = "The Company registration number should be 7 to 15 characters long")]
        [Display(Name = "Company registration number (CRN)")]
        public string CompaniesRegistrationNumber { get; set; }
    }
}