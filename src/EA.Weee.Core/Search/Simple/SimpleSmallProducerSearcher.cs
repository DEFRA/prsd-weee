namespace EA.Weee.Core.Search.Simple
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A producer searcher that uses a simple string matching algorithm.
    /// </summary>
    public class SimpleSmallProducerSearcher : SimpleSearcher<SmallProducerSearchResult>
    {
        public SimpleSmallProducerSearcher(ISearchResultProvider<SmallProducerSearchResult> searchResultProvider)
            : base(searchResultProvider)
        {
        }

        protected override string ResultToString(SmallProducerSearchResult result)
        {
            return $"{result.RegistrationNumber} {result.Name}";
        }

        protected override IOrderedEnumerable<SmallProducerSearchResult> OrderResults(IEnumerable<SmallProducerSearchResult> results)
        {
            return results
                .OrderBy(r => r.Name)
                .ThenBy(r => r.RegistrationNumber);
        }
    }
}
