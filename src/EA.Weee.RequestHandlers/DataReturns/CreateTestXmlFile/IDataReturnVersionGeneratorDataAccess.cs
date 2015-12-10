namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Producer;
    using EA.Weee.Domain.Scheme;

    public interface IDataReturnVersionGeneratorDataAccess
    {
        Task<Domain.Scheme.Scheme> FetchSchemeAsync(Guid organisationID);

        Task<IList<RegisteredProducer>> FetchRegisteredProducersAsync(Scheme scheme, int year);
    }
}
