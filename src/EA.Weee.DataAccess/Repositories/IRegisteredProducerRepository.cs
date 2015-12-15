namespace EA.Weee.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;

    public interface IRegisteredProducerRepository : IRepository
    {
        Task<RegisteredProducer> Get(Guid id);

        Task<IEnumerable<RegisteredProducer>> Get(string producerRegistrationNumber, int complianceYear);
    }
}
