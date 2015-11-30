namespace EA.Weee.Core.Search.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A searcher that uses a simple string matching algorithm.
    /// Only results that wholly contain the search phrase are returned.
    /// </summary>
    public abstract class SimpleSearcher<T> : ISearcher<T> where T : SearchResult
    {
        private readonly ISearchResultProvider<T> searchResultProvider;

        public SimpleSearcher(ISearchResultProvider<T> searchResultProvider)
        {
            this.searchResultProvider = searchResultProvider;
        }

        /// <summary>
        /// Returns results where the result name contains the whole search term,
        /// using a simple invariant string match.
        /// </summary>
        /// <param name="searchTerm">The string that must be contained.</param>
        /// <param name="maxResults">The maximum number of results to return. This must be at least 1.</param>
        /// <returns></returns>
        public async Task<IList<T>> Search(string searchTerm, int maxResults, bool asYouType)
        {
            if (maxResults < 1)
            {
                throw new ArgumentOutOfRangeException("maxResults", "The maximum number of results returned must be at least one.");
            }

            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<T>();
            }

            IList<T> list = await searchResultProvider.FetchAll();

            string invariantSearchTerm = ToInvariant(searchTerm);

            IEnumerable<T> results = list
                .Where(i => ToInvariant(ResultToString(i)).Contains(invariantSearchTerm));

            IOrderedEnumerable<T> orderedResults = OrderResults(results);

            return orderedResults
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

        protected abstract string ResultToString(T result);

        protected virtual IOrderedEnumerable<T> OrderResults(IEnumerable<T> results)
        {
            return results.OrderBy(i => true);
        }
    }
}
