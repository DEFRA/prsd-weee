namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using EA.Weee.Core.Search;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SearchResultsViewModel
    {
        [Required]
        [DisplayName("Search term")]
        public string SearchTerm { get; set; }

        public IList<RegisteredProducerSearchResult> Results { get; set; }

        [Required(ErrorMessage = "You must choose a producer")]
        public string SelectedRegistrationNumber { get; set; }
    }
}