namespace EA.Weee.Core.Search.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A producer searcher that uses a simple string matching algorithm.
    /// </summary>
    public class SimpleProducerSearcher : SimpleSearcher<ProducerSearchResult>
    {
        public SimpleProducerSearcher(ISearchResultProvider<ProducerSearchResult> searchResultProvider)
            : base(searchResultProvider)
        {
        }

        protected override string ResultToString(ProducerSearchResult result)
        {
            return string.Format("{0} {1}", result.RegistrationNumber, result.Name);
        }

        protected override IOrderedEnumerable<ProducerSearchResult> OrderResults(IEnumerable<ProducerSearchResult> results)
        {
            return results
                .OrderBy(r => r.Name)
                .ThenBy(r => r.RegistrationNumber);
        }
    }
}
