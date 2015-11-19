namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;

    public class SoleTraderDetailsViewModel
    {
        public Guid? OrganisationId { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }
    }
}