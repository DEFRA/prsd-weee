namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Producer;
    public interface IDataReturnVersionBuilderDataAccess
    {
        Task<DataReturn> FetchDataReturnOrDefault();

        Task<RegisteredProducer> GetRegisteredProducer(string producerRegistrationNumber);
    }
}
