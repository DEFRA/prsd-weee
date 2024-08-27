namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.Organisations.Base;

    public class PartnershipDetailsViewModel : OrganisationViewModel
    {
        [Required]
        public override string BusinessTradingName { get; set; }
    }
}