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

        public string TradingName { get; set; }

        public string CompaniesRegistrationNumber { get; set; }

        public OrganisationType Type { get; set; }

        public IList<OrganisationSearchData> MatchingOrganisations { get; set; }

        public PagingViewModel PagingViewModel { get; set; }

        [Required]
        public Guid? Selected { get; set; }
    }
}