namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Requests.Organisations;

    public class SelectOrganisationViewModel
    {
        public string Name { get; set; }

        public IList<OrganisationSearchData> MatchingOrganisations { get; private set; }

        [Required]
        public Guid? Selected { get; set; }

        public SelectOrganisationViewModel(string name, IList<OrganisationSearchData> matchingOrganisations)
        {
            Name = name;
            MatchingOrganisations = matchingOrganisations;
        }

        public SelectOrganisationViewModel(string name)
            : this(name, new List<OrganisationSearchData>())
        {
        }

        public SelectOrganisationViewModel()
            : this(string.Empty)
        {
        }
    }
}