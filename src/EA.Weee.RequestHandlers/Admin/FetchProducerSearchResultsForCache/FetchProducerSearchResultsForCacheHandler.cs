namespace EA.Weee.RequestHandlers.Admin.FetchProducerSearchResultsForCache
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FetchProducerSearchResultsForCacheHandler : IRequestHandler<Requests.Admin.FetchProducerSearchResultsForCache, IList<ProducerSearchResult>>
    {
        private readonly IFetchProducerSearchResultsForCacheDataAccess dataAccess;

        public FetchProducerSearchResultsForCacheHandler(IFetchProducerSearchResultsForCacheDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Task<IList<ProducerSearchResult>> HandleAsync(Requests.Admin.FetchProducerSearchResultsForCache request)
        {
            return dataAccess.FetchLatestProducers();
        }
    }
}
