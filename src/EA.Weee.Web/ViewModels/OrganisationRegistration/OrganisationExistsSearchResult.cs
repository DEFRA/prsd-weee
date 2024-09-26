namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class OrganisationExistsSearchResult
    {
        public IList<OrganisationFoundViewModel> Organisations { get; set; }
        public OrganisationFoundType FoundType { get; set; }
    }
}