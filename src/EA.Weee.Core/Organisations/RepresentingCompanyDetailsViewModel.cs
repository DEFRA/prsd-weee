namespace EA.Weee.Core.Organisations
{
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RepresentingCompanyDetailsViewModel
    {
        [Required]
        [DisplayName("Producer name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }

        [DisplayName("Trading name")]
        public string BusinessTradingName { get; set; }

        public RepresentingCompanyAddressData Address { get; set; } = new RepresentingCompanyAddressData();
    }
}