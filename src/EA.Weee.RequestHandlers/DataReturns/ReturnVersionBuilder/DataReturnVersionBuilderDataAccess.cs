namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Producer;
    using Scheme = Domain.Scheme.Scheme;

    public class DataReturnVersionBuilderDataAccess : IDataReturnVersionBuilderDataAccess
    {
        private readonly WeeeContext context;
        private readonly Scheme scheme;
        private readonly Quarter quarter;
        private ICollection<RegisteredProducer> schemeYearProducers;

        public DataReturnVersionBuilderDataAccess(Scheme scheme, Quarter quarter, WeeeContext context)
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
                .ToListAsync();
            }
            return schemeYearProducers;
        }

        public async Task<DataReturnVersion> GetLatestDataReturnVersionOrDefault()
        {
            return await context.DataReturnVersions
                .Where(rv => rv.DataReturn.Scheme.Id == scheme.Id)
                .Where(rv => rv.DataReturn.Quarter.Year == quarter.Year)
                .Where(rv => rv.DataReturn.Quarter.Q == quarter.Q)
                .OrderByDescending(rv => rv.CreatedDate)
                .FirstOrDefaultAsync();
        }
    }
}
