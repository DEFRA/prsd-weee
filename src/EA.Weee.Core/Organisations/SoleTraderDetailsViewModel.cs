namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Organisations.Base;

    public class SoleTraderDetailsViewModel : OrganisationViewModel
    {
        [Required]
        [DisplayName("Sole trader")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string CompanyName { get; set; }
    }
}