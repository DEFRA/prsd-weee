namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details
{
    using Core.DataStandards;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SoleTraderDetailsViewModel : OrganisationViewModel
    {
        [Required]
        [DisplayName("Sole trader name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }
    }
}