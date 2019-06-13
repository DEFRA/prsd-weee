namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details.Base;

    public class PartnershipDetailsViewModel : OrganisationViewModel
    {
        [Required]
        public override string BusinessTradingName { get; set; }
    }
}