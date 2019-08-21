namespace EA.Weee.Web.Areas.Admin.ViewModels.Organisation
{
    using EA.Weee.Core.DataStandards;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class EditSoleTraderOrganisationDetailsViewModel : EditOrganisationDetailsViewModelBase
    {
        [Required]
        [DisplayName("Sole trader / individual name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }

        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string BusinessTradingName { get; set; }
    }
}