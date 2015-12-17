namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;

    public interface IRemoveProducerDataAccess
    {
        Task<IEnumerable<ProducerSubmission>> GetProducerSubmissionsForRegisteredProducer(Guid registeredProducerId);

        Task<RegisteredProducer> GetProducerRegistration(Guid registeredProducerId);

        Task SaveChangesAsync();
    }
}
