namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;

    public class SearchViewModel
    {
        [Required]
        [DisplayName("Search term")]
        [StringLength(EnvironmentAgencyMaxFieldLengths.ProducerSearchTerm)]
        public string SearchTerm { get; set; }

        public string SelectedRegistrationNumber { get; set; }
    }
}