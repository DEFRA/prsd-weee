namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Validation;

    public class RepresentingCompanyDetailsViewModel
    {
        public string OrganisationType { get; set; }

        [Required]
        [DisplayName("Producer name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }

        [Required]
        [DisplayName("Trading name")]
        public string BusinessTradingName { get; set; }

        public AddressData Address { get; set; } = new AddressData();

        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Telephone number")]
        [GenericPhoneNumber(ErrorMessage = "The telephone number can use numbers, spaces and some special characters (-+). It must be no longer than 20 characters.")]
        public string Telephone { get; set; }

        [Display(Name = "Email address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}