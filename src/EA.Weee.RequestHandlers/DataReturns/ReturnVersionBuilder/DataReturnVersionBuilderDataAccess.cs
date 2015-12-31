namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Producer;

    public class DataReturnVersionBuilderDataAccess : IDataReturnVersionBuilderDataAccess
    {
        private readonly WeeeContext context;
        private readonly Domain.Scheme.Scheme scheme;
        private readonly Quarter quarter;
        private ICollection<RegisteredProducer> schemeYearProducers;

        public DataReturnVersionBuilderDataAccess(Domain.Scheme.Scheme scheme, Quarter quarter, WeeeContext context)
        {
            this.context = context;
            this.scheme = scheme;
            this.quarter = quarter;
        }

        public async Task<DataReturn> FetchDataReturnOrDefault()
        {
            return await context.DataReturns
                .Where(dr => dr.Scheme.Id == scheme.Id)
                .Where(dr => dr.Quarter.Year == quarter.Year)
                .Where(dr => dr.Quarter.Q == quarter.Q)
                .SingleOrDefaultAsync();
        }

        public async Task<RegisteredProducer> GetRegisteredProducer(string producerRegistrationNumber)
        {
            ICollection<RegisteredProducer> producers = await GetSchemeYearProducers();
            return producers.SingleOrDefault(p => p.ProducerRegistrationNumber == producerRegistrationNumber);
        }

        private async Task<ICollection<RegisteredProducer>> GetSchemeYearProducers()
        {
            if (schemeYearProducers == null)
            {
                schemeYearProducers = await context.RegisteredProducers
                .Where(p => p.Scheme.Id == scheme.Id)
                .Where(p => p.ComplianceYear == quarter.Year)
                .Where(p => p.CurrentSubmission != null)
                .AsNoTracking()
                .ToListAsync();
            }
            return schemeYearProducers;
        }
    }
}
