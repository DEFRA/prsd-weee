namespace EA.Weee.RequestHandlers.Search.FetchSmallProducerSearchResultsForCache
{
    using EA.Weee.Core.Search;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFetchSmallProducerSearchResultsForCacheDataAccess
    {
        Task<IList<SmallProducerSearchResult>> FetchLatestProducers();
    }
}
