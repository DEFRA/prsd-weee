namespace EA.Weee.Web.Areas.Test.ViewModels.GeneratePcsXml
{
    using Core.Organisations;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SelectOrganisationViewModel
    {
        [Required]
        [DisplayName("Organisation Name")]
        public string OrganisationName { get; set; }

        public IList<OrganisationSearchData> MatchingOrganisations { get; set; }

        public PagingViewModel PagingViewModel { get; set; }
    }
}