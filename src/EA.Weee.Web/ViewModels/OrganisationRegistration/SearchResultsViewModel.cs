﻿namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using EA.Weee.Core.Search;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class SearchResultsViewModel
    {
        [Required]
        [DisplayName("Search term")]
        public string SearchTerm { get; set; }

        public IList<OrganisationSearchResult> Results { get; set; }

        [Required(ErrorMessage = "You must choose an organisation")]
        public Guid? SelectedOrganisationId { get; set; }
    }
}