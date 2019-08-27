namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using EA.Weee.Core.Search;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
