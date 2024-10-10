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
            return await context.DirectProducerSubmissions
                .GroupBy(s => s.RegisteredProducer.ProducerRegistrationNumber)
                .Select(group => new
                {
                    ProducerRegistrationNumber = group.Key,
                    LatestSubmission = group.OrderByDescending(s => s.RegisteredProducer.ComplianceYear)
                        .FirstOrDefault(),
                })
                .OrderBy(x => x.ProducerRegistrationNumber)
                .Select(x => new
                {
                    x.ProducerRegistrationNumber,
                    Name = x.LatestSubmission.DirectRegistrant.AuthorisedRepresentativeId.HasValue ? 
                        x.LatestSubmission.DirectRegistrant.AuthorisedRepresentative.OverseasProducerName : x.LatestSubmission.DirectRegistrant.Organisation.Name,
                    Id = x.LatestSubmission.DirectRegistrantId
                }).Select(r => new SmallProducerSearchResult()
                {
                    RegistrationNumber = r.ProducerRegistrationNumber,
                    Name = r.Name,
                    Id = r.Id
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
