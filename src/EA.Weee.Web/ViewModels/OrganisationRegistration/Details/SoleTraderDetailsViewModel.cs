namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using Core.DataStandards;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details.Base;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SoleTraderDetailsViewModel : OrganisationViewModel
    {
        [Required]
        [DisplayName("Sole trader")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }
    }
}