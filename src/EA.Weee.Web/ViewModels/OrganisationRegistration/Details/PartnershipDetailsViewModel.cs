namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details.Base;
    using System.ComponentModel.DataAnnotations;

    public class PartnershipDetailsViewModel : OrganisationViewModel
    {
        [Required]
        public override string BusinessTradingName { get; set; }
    }
}