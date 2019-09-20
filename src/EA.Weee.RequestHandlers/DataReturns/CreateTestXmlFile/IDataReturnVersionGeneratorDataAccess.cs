namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using Domain.Producer;
    using EA.Weee.Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDataReturnVersionGeneratorDataAccess
    {
        Task<Domain.Scheme.Scheme> FetchSchemeAsync(Guid organisationID);

        Task<IList<RegisteredProducer>> FetchRegisteredProducersAsync(Scheme scheme, int year);
    }
}
