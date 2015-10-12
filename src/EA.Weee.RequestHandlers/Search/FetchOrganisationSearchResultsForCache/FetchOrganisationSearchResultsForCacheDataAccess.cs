namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using EA.Weee.Core.Search;
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FetchOrganisationSearchResultsForCacheDataAccess : IFetchOrganisationSearchResultsForCacheDataAccess
    {
        private readonly WeeeContext context;

        public FetchOrganisationSearchResultsForCacheDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Returns a list of all complete organisations, ordered by organisation name.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<OrganisationSearchResult>> FetchCompleteOrganisations()
        {
            var results = await context
                .Organisations
                .Where(o => o.OrganisationStatus.Value == Domain.Organisation.OrganisationStatus.Complete.Value)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(r => new OrganisationSearchResult()
                {
                    OrganisationId = r.Id,
                    Name = r.OrganisationName
                })
                .ToList();
        }
    }
}
