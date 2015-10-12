namespace EA.Weee.Core.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Searches for complete organisations.
    /// </summary>
    public interface IOrganisationSearcher
    {
        /// <summary>
        /// Returns a list of complete organisation results which match the specified search term.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        Task<IList<OrganisationSearchResult>> Search(string searchTerm, int maxResults);
    }
}
