namespace EA.Weee.Web.Areas.Admin.ViewModels.Organisation
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;

    public class EditPartnershipOrganisationDetailsViewModel : EditOrganisationDetailsViewModelBase
    {
        [Required]
        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string BusinessTradingName { get; set; }
    }
}