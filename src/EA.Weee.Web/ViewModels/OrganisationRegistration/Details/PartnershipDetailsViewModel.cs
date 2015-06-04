namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PartnershipDetailsViewModel 
    {
        [Required]
        [DisplayName("Business trading name")]
        public virtual string BusinessTradingName { get; set; }
    }
}