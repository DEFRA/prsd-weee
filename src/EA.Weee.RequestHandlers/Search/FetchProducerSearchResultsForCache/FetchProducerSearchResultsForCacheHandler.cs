namespace EA.Weee.RequestHandlers.Search.FetchProducerSearchResultsForCache
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Search;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FetchProducerSearchResultsForCacheHandler : IRequestHandler<Requests.Search.FetchProducerSearchResultsForCache, IList<ProducerSearchResult>>
    {
        private readonly IFetchProducerSearchResultsForCacheDataAccess dataAccess;

        public FetchProducerSearchResultsForCacheHandler(IFetchProducerSearchResultsForCacheDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Task<IList<ProducerSearchResult>> HandleAsync(Requests.Search.FetchProducerSearchResultsForCache request)
        {
            return dataAccess.FetchLatestProducers();
        }
    }
}
