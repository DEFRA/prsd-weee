namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Search;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FetchOrganisationSearchResultsForCacheHandler : IRequestHandler<Requests.Search.FetchOrganisationSearchResultsForCache, IList<OrganisationSearchResult>>
    {
        private readonly IFetchOrganisationSearchResultsForCacheDataAccess dataAccess;

        public FetchOrganisationSearchResultsForCacheHandler(IFetchOrganisationSearchResultsForCacheDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Task<IList<OrganisationSearchResult>> HandleAsync(Requests.Search.FetchOrganisationSearchResultsForCache request)
        {
            return dataAccess.FetchCompleteOrganisations();
        }
    }
}
