namespace EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation.Details
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using AddAatf.Details;
    using Core.DataStandards;

    public class SoleTraderDetailsViewModel : OrganisationViewModel
    {
        [Required]
        [DisplayName("Sole trader / individual name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }
    }
}