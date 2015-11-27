namespace EA.Weee.RequestHandlers.Search.FetchProducerSearchResultsForCache
{
    using EA.Weee.Core.Search;
    using EA.Weee.DataAccess;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class FetchProducerSearchResultsForCacheDataAccess : IFetchProducerSearchResultsForCacheDataAccess
    {
        private readonly WeeeContext context;

        public FetchProducerSearchResultsForCacheDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Returns a list of all producers for the latest compliance year in which they are registered.
        /// 
        /// Producers will be grouped by registration number.
        /// Within that group only the versions for the latest compliance year will be considered.
        /// Then, within that group only the latest updated version for that year will be considered.
        /// Results will be ordered by registration number.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ProducerSearchResult>> FetchLatestProducers()
        {
            var results = await context
                .Producers
                .Where(p => p.IsCurrentForComplianceYear)
                .GroupBy(p => p.RegistrationNumber)
                .Select(group => group.OrderByDescending(p => p.MemberUpload.ComplianceYear).FirstOrDefault())
                .OrderBy(p => p.RegistrationNumber)
                .Include(p => p.MemberUpload)
                .Include(p => p.ProducerBusiness)
                .Include(p => p.ProducerBusiness.CompanyDetails)
                .Include(p => p.ProducerBusiness.Partnership)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(r => new ProducerSearchResult()
                {
                    RegistrationNumber = r.RegistrationNumber,
                    Name = r.OrganisationName
                })
                .ToList();
        }
    }
}
