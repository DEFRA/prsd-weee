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
        /// For now, only organisations representing schemes will be returned, excluding
        /// any scheme that has a status of rejected.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<OrganisationSearchResult>> FetchCompleteOrganisations()
        {
            var results = await context
                .Schemes
                .Where(s => s.SchemeStatus.Value != Domain.Scheme.SchemeStatus.Rejected.Value)
                .Select(s => s.Organisation)
                .Where(o => o.OrganisationStatus.Value == Domain.Organisation.OrganisationStatus.Complete.Value)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(r => new OrganisationSearchResult()
                {
                    OrganisationId = r.Id,
                    Name = r.OrganisationName
                })
                .OrderBy(r => r.Name)
                .ToList();
        }
    }
}
