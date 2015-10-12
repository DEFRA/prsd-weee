namespace EA.Weee.Core.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to the complete list of complete organisation search results.
    /// </summary>
    public interface IOrganisationSearchResultProvider
    {
        /// <summary>
        /// Returns all possible complete organisation search results, ordered by organisation name.
        /// </summary>
        /// <returns></returns>
        Task<IList<OrganisationSearchResult>> FetchAll();
    }
}
