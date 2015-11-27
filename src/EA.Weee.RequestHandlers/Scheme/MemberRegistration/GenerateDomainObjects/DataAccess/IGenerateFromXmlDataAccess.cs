namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess
{
    using Domain;
    using Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGenerateFromXmlDataAccess
    {
        Task<Country> GetCountry(string countryName);

        Task<Queue<string>> ComputePrns(int numberOfPrnsNeeded);

        Task<Producer> GetLatestProducerRecord(Guid schemeId, string producerRegistrationNumber);

        Task<Producer> GetLatestProducerRecordExcludeScheme(Guid schemeId, string producerRegistrationNumber);

        Task<MigratedProducer> GetMigratedProducer(string producerRegistrationNumber);
    }
}
