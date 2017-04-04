namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Producer;

    public class RemoveProducerDataAccess : IRemoveProducerDataAccess
    {
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;
        private readonly WeeeContext context;

        public RemoveProducerDataAccess(IRegisteredProducerDataAccess registeredProducerDataAccess, WeeeContext context)
        {
            this.registeredProducerDataAccess = registeredProducerDataAccess;
            this.context = context;
        }

        public async Task<RegisteredProducer> GetProducerRegistration(Guid registeredProducerId)
        {
            return await registeredProducerDataAccess.GetProducerRegistration(registeredProducerId);
        }

        public async Task<IEnumerable<ProducerSubmission>> GetProducerSubmissionsForRegisteredProducer(Guid registeredProducerId)
        {
            return await context.ProducerSubmissions
                .Where(ps => ps.RegisteredProducer.Id == registeredProducerId)
                .Where(ps => ps.MemberUpload.IsSubmitted)
                .Include(ps => ps.MemberUpload)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
