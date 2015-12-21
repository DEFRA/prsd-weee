namespace EA.Weee.RequestHandlers.Search.FetchProducerSearchResultsForCache
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Search;

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
