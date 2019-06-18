namespace EA.Weee.Web.Areas.Admin.ViewModels.Organisation
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;

    public class EditSoleTraderOrganisationDetailsViewModel : EditOrganisationDetailsViewModelBase
    {
        [Required]
        [DisplayName("Sole trader or individual")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }

        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public override string BusinessTradingName { get; set; }
    }
}