namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details.Base
{
    using Core.DataStandards;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

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