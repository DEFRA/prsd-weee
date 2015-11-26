namespace EA.Weee.RequestHandlers.Search.FetchProducerSearchResultsForCache
{
    using EA.Weee.Core.Search;
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
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
                .RegisteredProducers
                .Where(rp => rp.CurrentSubmission != null)
                .GroupBy(rp => rp.ProducerRegistrationNumber)
                .Select(group => group.OrderByDescending(rp => rp.ComplianceYear).FirstOrDefault())
                .OrderBy(rp => rp.ProducerRegistrationNumber)
                .Include(rp => rp.CurrentSubmission)
                .Include(rp => rp.CurrentSubmission.MemberUpload)
                .Include(rp => rp.CurrentSubmission.ProducerBusiness)
                .Include(rp => rp.CurrentSubmission.ProducerBusiness.CompanyDetails)
                .Include(rp => rp.CurrentSubmission.ProducerBusiness.Partnership)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(r => new ProducerSearchResult()
                {
                    RegistrationNumber = r.ProducerRegistrationNumber,
                    Name = r.CurrentSubmission.OrganisationName
                })
                .ToList();
        }
    }
}
