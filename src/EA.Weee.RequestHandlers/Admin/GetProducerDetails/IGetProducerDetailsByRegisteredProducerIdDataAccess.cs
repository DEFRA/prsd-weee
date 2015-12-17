namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Threading.Tasks;
    using Domain.Producer;

    public interface IGetProducerDetailsByRegisteredProducerIdDataAccess
    {
        Task<RegisteredProducer> Fetch(Guid registeredProducerId);
    }
}
