namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Validation;

    public class SoleTraderOrPartnershipDetailsViewModel
    {
        public string OrganisationType { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("Trading name")]
        public string BusinessTradingName { get; set; }

        public AddressData Address { get; set; }

        public SoleTraderOrPartnershipDetailsViewModel()
        {
            this.Address = new AddressData();
        }
    }
}