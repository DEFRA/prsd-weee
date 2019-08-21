namespace EA.Weee.Web.Areas.Admin.ViewModels.AddOrganisation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.AatfReturn;
    using Core.Search;
    using Core.Shared;

    public class SearchResultsViewModel
    {
        [Required]
        [DisplayName("Search term")]
        public string SearchTerm { get; set; }

        public IList<OrganisationSearchResult> Results { get; set; }

        [Required(ErrorMessage = "You must choose an organisation")]
        [DisplayName("Select an organisation to add")]
        public Guid? SelectedOrganisationId { get; set; }

        public EntityType EntityType { get; set; }

        public bool IsAeOrAatf => EntityType == EntityType.Aatf || EntityType == EntityType.Ae;
    }
}