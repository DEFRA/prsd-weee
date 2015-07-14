namespace EA.Weee.Core.Organisations
{
    using System.Collections.Generic;

    public class OrganisationSearchDataResult
    {
        public IList<OrganisationSearchData> Results { get; private set; }
        public int TotalMatchingOrganisations { get; private set; }

        public OrganisationSearchDataResult(IList<OrganisationSearchData> results, int totalMatchingOrganisations)
        {
            Results = results;
            TotalMatchingOrganisations = totalMatchingOrganisations;
        }
    }
}
