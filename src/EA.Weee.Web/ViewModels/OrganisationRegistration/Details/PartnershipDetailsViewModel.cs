namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PartnershipDetailsViewModel 
    {
        public Guid? OrganisationId { get; set; }

        [Required]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }
    }
}