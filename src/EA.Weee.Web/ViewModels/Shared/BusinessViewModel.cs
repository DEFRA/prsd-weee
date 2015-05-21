namespace EA.Weee.Web.ViewModels.Shared
{
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core.Validation;
    using Prsd.Core.Web;

    public class BusinessViewModel
    {
        [Required]
        [Display(Name = "Organisation name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Organisation type")]
        public string EntityType { get; set; }

        [RequiredIf("EntityType", "Limited Company", "Companies House number is required")]
        [Display(Name = "Companies House number")]
        public string CompaniesHouseRegistrationNumber { get; set; }

        [RequiredIf("EntityType", "Sole Trader", "Sole trader registration number is required")]
        [Display(Name = "Registration number")]
        public string SoleTraderRegistrationNumber { get; set; }

        [RequiredIf("EntityType", "Partnership", "Partnership registration number is required")]
        [Display(Name = "Registration number")]
        public string PartnershipRegistrationNumber { get; set; }

        [Display(Name = "Additional registration number")]
        public string AdditionalRegistrationNumber { get; set; }
    }
}