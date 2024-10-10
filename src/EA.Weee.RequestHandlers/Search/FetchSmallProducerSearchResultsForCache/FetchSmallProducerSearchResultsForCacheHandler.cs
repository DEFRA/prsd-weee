namespace EA.Weee.RequestHandlers.Search.FetchSmallProducerSearchResultsForCache
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Search;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FetchSmallProducerSearchResultsForCacheHandler : IRequestHandler<Requests.Search.FetchSmallProducerSearchResultsForCache, IList<SmallProducerSearchResult>>
    {
        private readonly IFetchSmallProducerSearchResultsForCacheDataAccess dataAccess;

        public FetchSmallProducerSearchResultsForCacheHandler(IFetchSmallProducerSearchResultsForCacheDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Task<IList<SmallProducerSearchResult>> HandleAsync(Requests.Search.FetchSmallProducerSearchResultsForCache request)
        {
            return dataAccess.FetchLatestProducers();
        }
    }
}
