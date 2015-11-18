namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Producer;

    public interface IGenerateFromXmlDataAccess
    {
        Task<Country> GetCountry(string countryName);

        Task<Queue<string>> ComputePrns(int numberOfPrnsNeeded);

        Task<Producer> GetLatestProducerRecord(Guid schemeId, string producerRegistrationNumber, bool excludeSpecifiedSchemeId = false);

        Task<MigratedProducer> GetMigratedProducer(string producerRegistrationNumber);
    }
}
