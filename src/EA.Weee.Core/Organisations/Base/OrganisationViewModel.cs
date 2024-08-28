namespace EA.Weee.Core.Organisations.Base
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;

    public abstract class OrganisationViewModel
    {
        public string OrganisationType { get; set; }

        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public abstract string CompanyName { get; set; }

        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }

        public ExternalAddressData Address { get; set; } = new ExternalAddressData();

        public Core.Shared.EntityType EntityType { get; set; }

        [Required(ErrorMessage = "Enter company registration number (CRN)")]
        [StringLength(maximumLength: EnvironmentAgencyMaxFieldLengths.CompanyRegistrationNumber, MinimumLength = 7, ErrorMessage = "The Company registration number should be 7 to 15 characters long")]
        [Display(Name = "Company registration number (CRN)")]
        public string CompaniesRegistrationNumber { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("If you are registering as an authorised representative of a non-UK established organisation, enter the brands they place on the market.")]
        public string EEEBrandNames { get; set; }
    }
}