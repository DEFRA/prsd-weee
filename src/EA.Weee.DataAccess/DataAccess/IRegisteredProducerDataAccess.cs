namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;

    public interface IRegisteredProducerDataAccess
    {
        Task<RegisteredProducer> GetProducerRegistration(Guid id);

        Task<IEnumerable<RegisteredProducer>> GetProducerRegistrations(string producerRegistrationNumber, int complianceYear);
    }
}
