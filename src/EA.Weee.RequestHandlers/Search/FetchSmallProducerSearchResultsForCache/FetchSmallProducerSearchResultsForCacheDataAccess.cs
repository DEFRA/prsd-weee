namespace EA.Weee.RequestHandlers.Search.FetchSmallProducerSearchResultsForCache
{
    using EA.Weee.Core.Search;
    using EA.Weee.DataAccess;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;

    public class FetchSmallProducerSearchResultsForCacheDataAccess : IFetchSmallProducerSearchResultsForCacheDataAccess
    {
        private readonly WeeeContext context;

        public FetchSmallProducerSearchResultsForCacheDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IList<SmallProducerSearchResult>> FetchLatestProducers()
        {
            var results = await context.DirectProducerSubmissions
                .Where(s => s.DirectProducerSubmissionStatus.Value == DirectProducerSubmissionStatus.Complete.Value)
                .Select(s => s.RegisteredProducer)
                .GroupBy(rp => rp.ProducerRegistrationNumber)
                .Select(group => group.OrderByDescending(rp => rp.ComplianceYear).FirstOrDefault())
                .OrderBy(rp => rp.ProducerRegistrationNumber)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(r => new SmallProducerSearchResult()
            {
                RegistrationNumber = r.ProducerRegistrationNumber,
                Name = r.CurrentSubmission.OrganisationName
            })
                .ToList();
        }
    }
}
