namespace EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation.Details
{
    using System.ComponentModel.DataAnnotations;
    using AddAatf.Details;

    public class PartnershipDetailsViewModel : OrganisationViewModel
    {
        [Required]
        public override string BusinessTradingName { get; set; }
    }
}