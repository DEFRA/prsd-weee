namespace EA.Weee.RequestHandlers.Search.FetchOrganisationSearchResultsForCache
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.Search;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Scheme;

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
            var organisations = await context.Organisations
                .Where(p => p.OrganisationStatus.Value == OrganisationStatus.Complete.Value)
                .ToListAsync();

            var schemes = await context.Schemes.ToListAsync();

            foreach (Scheme scheme in schemes)
            {
                if (scheme.SchemeStatus.Value == SchemeStatus.Rejected.Value)
                {
                    organisations.Remove(scheme.Organisation);
                }
            }

            return organisations.Select(r => new OrganisationSearchResult()
            {
                OrganisationId = r.Id,
                Name = r.OrganisationName
            })
                .OrderBy(r => r.Name)
                .ToList();
        }
    }
}
