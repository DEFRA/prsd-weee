﻿namespace EA.Weee.RequestHandlers.Admin.FetchProducerSearchResultsForCache
{
    using EA.Weee.Core.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IFetchProducerSearchResultsForCacheDataAccess
    {
        /// <summary>
        /// Returns a list of all producers for the latest compliance year in which they are registered.
        /// 
        /// Producers will be grouped by registration number.
        /// Within that group only the versions for the latest compliance year will be considered.
        /// Then, within that group only the latest updated version for that year will be considered.
        /// Results will be ordered by registration number.
        /// </summary>
        /// <returns></returns>
        Task<IList<ProducerSearchResult>> FetchLatestProducers();
    }
}
