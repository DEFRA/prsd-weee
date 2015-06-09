namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Requests.Organisations;

    public class SelectOrganisationViewModel
    {
        public string Name { get; set; }

        public IList<OrganisationSearchData> MatchingOrganisations { get; set; }

        [Required]
        public Guid? Selected { get; set; }

        public int TotalPages { get; private set; }

        public int PreviousPage { get; private set; }

        public int NextPage { get; private set; }

        public int StartingAt { get; private set; }

        public SelectOrganisationViewModel(string name, IList<OrganisationSearchData> matchingOrganisations, int totalPages, int previousPage, int nextPage, int startingAt)
        {
            Name = name;
            MatchingOrganisations = matchingOrganisations;
            TotalPages = totalPages;
            PreviousPage = previousPage;
            NextPage = nextPage;
            StartingAt = startingAt;
        }

        public SelectOrganisationViewModel(int totalPages, int previousPage, int nextPage, int startingAt)
            : this(string.Empty, new List<OrganisationSearchData>(), totalPages, previousPage, nextPage, startingAt)
        {
        }

        public SelectOrganisationViewModel()
            : this(totalPages: 1, previousPage: 0, nextPage: 2, startingAt: 1)
        {
        }
    }
}