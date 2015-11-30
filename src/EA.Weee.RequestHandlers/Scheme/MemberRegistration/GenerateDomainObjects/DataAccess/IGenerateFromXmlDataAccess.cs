namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Producer;

    public interface IGenerateFromXmlDataAccess
    {
        Task<Country> GetCountry(string countryName);

        Task<Queue<string>> ComputePrns(int numberOfPrnsNeeded);

        Task<bool> MigratedProducerExists(string producerRegistrationNumber);

        Task<bool> ProducerRegistrationExists(string producerRegistrationNumber);

        Task<RegisteredProducer> FetchRegisteredProducerOrDefault(string producerRegistrationNumber, int complianceYear, Guid schemeId);
    }
}
