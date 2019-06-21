namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details
{
    using System.ComponentModel.DataAnnotations;

    public class PartnershipDetailsViewModel : OrganisationViewModel
    {
        [Required]
        public override string BusinessTradingName { get; set; }
    }
}