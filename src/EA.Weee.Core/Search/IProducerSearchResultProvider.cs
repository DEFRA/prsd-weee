namespace EA.Weee.Core.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to the complete list of producer search results.
    /// </summary>
    public interface IProducerSearchResultProvider
    {
        /// <summary>
        /// Returns all possible producer search results, ordered by producer registration number.
        /// </summary>
        /// <returns></returns>
        Task<IList<ProducerSearchResult>> FetchAll();
    }
}
