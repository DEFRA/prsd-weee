namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using EA.Weee.Core.Search;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IFetchOrganisationSearchResultsForCacheDataAccess
    {
        /// <summary>
        /// Returns a list of all complete organisations, ordered by name..
        /// </summary>
        /// <returns></returns>
        Task<IList<OrganisationSearchResult>> FetchCompleteOrganisations();
    }
}
