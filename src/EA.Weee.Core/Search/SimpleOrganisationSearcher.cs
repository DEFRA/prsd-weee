namespace EA.Weee.Core.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An organsation searcher that uses a simple string matching algorithm.
    /// </summary>
    public class SimpleOrganisationSearcher : IOrganisationSearcher
    {
        private readonly IOrganisationSearchResultProvider searchResultProvider;

        public SimpleOrganisationSearcher(IOrganisationSearchResultProvider searchResultProvider)
        {
            this.searchResultProvider = searchResultProvider;
        }

        /// <summary>
        /// Returns results where the organisation name contains the whole search term,
        /// using a simple invariant string match.
        /// </summary>
        /// <param name="searchTerm">The string that must be contained.</param>
        /// <param name="maxResults">The maximum number of results to return. This must be at least 1.</param>
        /// <returns></returns>
        public async Task<IList<OrganisationSearchResult>> Search(string searchTerm, int maxResults)
        {
            if (maxResults < 1)
            {
                throw new ArgumentOutOfRangeException("maxResults", "The maximum number of results returned must be at least one.");
            }

            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<OrganisationSearchResult>();
            }

            IList<OrganisationSearchResult> list = await searchResultProvider.FetchAll();

            string invariantSearchTerm = ToInvariant(searchTerm);

            return list
                .Where(i => ToInvariant(i.Name).Contains(invariantSearchTerm))
                .OrderBy(i => i.Name)
                .Take(maxResults)
                .ToList();
        }

        private string ToInvariant(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return string.Empty;
            }

            return term.ToLowerInvariant();
        }
    }
}
