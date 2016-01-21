namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Producer;
    using Domain.Scheme;

    public class GetProducerDetailsByRegisteredProducerIdDataAccess : IGetProducerDetailsByRegisteredProducerIdDataAccess
    {
        private readonly WeeeContext context;

        public GetProducerDetailsByRegisteredProducerIdDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<RegisteredProducer> Fetch(Guid registeredProducerId)
        {
            return await context.RegisteredProducers.Where(p => p.Id == registeredProducerId).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Fetches all data returns for the specified scheme and compliance year.
        /// The current data return version, EEE output return version and EEE output amounts
        /// will be pre-loaded in the results.
        /// Results will be returned orderd by quarter type.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DataReturn>> FetchDataReturns(Scheme scheme, int complianceYear)
        {
            return await context.DataReturns
                .Include(dr => dr.CurrentVersion)
                .Include(dr => dr.CurrentVersion.EeeOutputReturnVersion)
                .Include(dr => dr.CurrentVersion.EeeOutputReturnVersion.EeeOutputAmounts)
                .Where(dr => dr.Scheme.Id == scheme.Id)
                .Where(dr => dr.Quarter.Year == complianceYear)
                .OrderBy(dr => dr.Quarter.Q)
                .ToListAsync();
        }
    }
}
