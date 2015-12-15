namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Core.Admin;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Producer;

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
    }
}
