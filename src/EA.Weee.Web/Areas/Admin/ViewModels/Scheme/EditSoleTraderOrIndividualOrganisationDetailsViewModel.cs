namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using Core.Organisations;
    using Core.Shared;

    public class EditSoleTraderOrIndividualOrganisationDetailsViewModel
    {
        public Guid SchemeId { get; set; }

        public Guid OrgId { get; set; }
        
        public OrganisationType OrganisationType { get; set; }

        [Required]
        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string BusinessTradingName { get; set; }

        public AddressData BusinessAddress { get; set; }
    }
}