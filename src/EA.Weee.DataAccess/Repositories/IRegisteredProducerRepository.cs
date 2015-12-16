namespace EA.Weee.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;

    public interface IRegisteredProducerRepository : IRepository
    {
        Task<RegisteredProducer> GetProducerRegistration(Guid id);

        Task<IEnumerable<RegisteredProducer>> GetProducerRegistrations(string producerRegistrationNumber, int complianceYear);
    }
}
