namespace EA.Weee.Core.Organisations
{
    using System.Collections.Generic;

    public class OrganisationSearchDataResult
    {
        public IList<PublicOrganisationData> Results { get; private set; }
        public int TotalMatchingOrganisations { get; private set; }

        public OrganisationSearchDataResult(IList<PublicOrganisationData> results, int totalMatchingOrganisations)
        {
            Results = results;
            TotalMatchingOrganisations = totalMatchingOrganisations;
        }
    }
}
