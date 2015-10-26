namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PartnershipDetailsViewModel 
    {
        public Guid? OrganisationId { get; set; }

        [Required(ErrorMessage = "Enter the trading name for your business")]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }
    }
}