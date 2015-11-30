namespace EA.Weee.Core.Search.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An organsation searcher that uses a simple string matching algorithm.
    /// </summary>
    public class SimpleOrganisationSearcher : SimpleSearcher<OrganisationSearchResult>
    {
        public SimpleOrganisationSearcher(ISearchResultProvider<OrganisationSearchResult> searchResultProvider)
            : base(searchResultProvider)
        {
        }

        protected override string ResultToString(OrganisationSearchResult result)
        {
            return result.Name;
        }

        protected override IOrderedEnumerable<OrganisationSearchResult> OrderResults(IEnumerable<OrganisationSearchResult> results)
        {
            return results.OrderBy(r => r.Name);
        }
    }
}
