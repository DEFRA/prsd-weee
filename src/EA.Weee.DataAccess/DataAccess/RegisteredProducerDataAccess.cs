namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Producer;

    public class RegisteredProducerDataAccess : IRegisteredProducerDataAccess
    {
        private readonly WeeeContext context;

        public RegisteredProducerDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public void Add(RegisteredProducer registeredProducer)
        {
            context.AllRegisteredProducers.Add(registeredProducer);
        }

        public async Task<RegisteredProducer> GetProducerRegistration(Guid id)
        {
            var producer = await context.RegisteredProducers.SingleOrDefaultAsync(p => p.Id == id);

            if (producer == null)
            {
                throw new ArgumentException(string.Format("Producer with Id '{0}' does not exist", id));
            }

            return producer;
        }

        public async Task<IEnumerable<RegisteredProducer>> GetProducerRegistrations(string producerRegistrationNumber, int complianceYear)
        {
            return await context.RegisteredProducers
                .Where(p => p.ProducerRegistrationNumber == producerRegistrationNumber
                    && p.ComplianceYear == complianceYear)
                .ToListAsync();
        }
    }
}
