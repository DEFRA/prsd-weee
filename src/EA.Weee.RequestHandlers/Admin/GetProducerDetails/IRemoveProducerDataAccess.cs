namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRemoveProducerDataAccess
    {
        Task<IEnumerable<ProducerSubmission>> GetProducerSubmissionsForRegisteredProducer(Guid registeredProducerId);

        Task<RegisteredProducer> GetProducerRegistration(Guid registeredProducerId);

        Task SaveChangesAsync();
    }
}
