namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Details.Base;

    public class SoleTraderDetailsViewModel : OrganisationViewModel
    {
        [Required]
        [DisplayName("Sole trader name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }
    }
}