namespace EA.Weee.Core.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Searches for producers.
    /// </summary>
    public interface IProducerSearcher
    {
        /// <summary>
        /// Returns a list of search results which match the specified search term.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        Task<IList<ProducerSearchResult>> Search(string searchTerm, int maxResults);
    }
}
