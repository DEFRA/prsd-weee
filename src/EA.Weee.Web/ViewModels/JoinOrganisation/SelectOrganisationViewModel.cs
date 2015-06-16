namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.ViewModels.Shared;
    using Weee.Requests.Organisations;

    public class SelectOrganisationViewModel
    {
        public string Name { get; set; }

        public IList<OrganisationSearchData> MatchingOrganisations { get; private set; }

        public PagingViewModel PagingViewModel { get; private set; }

        [Required]
        public Guid? Selected { get; set; }

        public SelectOrganisationViewModel(string name, IList<OrganisationSearchData> matchingOrganisations, PagingViewModel pagingViewModel)
        {
            Name = name;
            MatchingOrganisations = matchingOrganisations;
            PagingViewModel = pagingViewModel;
        }

        public SelectOrganisationViewModel(PagingViewModel pagingViewModel) : this(string.Empty, new List<OrganisationSearchData>(), pagingViewModel)
        {
        }
    }
}