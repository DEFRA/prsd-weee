namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Core.Search;

    public interface IFetchOrganisationSearchResultsForCacheDataAccess
    {
        /// <summary>
        /// Returns a list of all complete organisations, ordered by name.
        /// For now, only organisations representing schemes will be returned, excluding
        /// any scheme that has a status of rejected.
        /// </summary>
        /// <returns></returns>
        Task<IList<OrganisationSearchResult>> FetchCompleteOrganisations();
    }
}
