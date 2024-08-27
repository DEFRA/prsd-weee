namespace EA.Weee.Core.Organisations.Base
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;

    public abstract class OrganisationViewModel
    {
        public string OrganisationType { get; set; }

        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string EEEBrandNames { get; set; }

        public ExternalAddressData Address { get; set; }

        public Core.Shared.EntityType EntityType { get; set; }

        public OrganisationViewModel()
        {
            Address = new ExternalAddressData();
        }
    }
}