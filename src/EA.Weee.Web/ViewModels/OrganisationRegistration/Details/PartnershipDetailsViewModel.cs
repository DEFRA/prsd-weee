namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;

    public class PartnershipDetailsViewModel 
    {
        public Guid? OrganisationId { get; set; }

        [Required]
        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public virtual string BusinessTradingName { get; set; }
    }
}