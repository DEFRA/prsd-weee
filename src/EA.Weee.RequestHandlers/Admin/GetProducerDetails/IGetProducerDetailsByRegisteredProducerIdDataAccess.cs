namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Producer;
    using Domain.Scheme;

    public interface IGetProducerDetailsByRegisteredProducerIdDataAccess
    {
        Task<RegisteredProducer> Fetch(Guid registeredProducerId);

        /// <summary>
        /// Fetches all data returns for the specified scheme and compliance year.
        /// The current data return version, EEE output return version and EEE output amounts
        /// will be pre-loaded in the results.
        /// Results will be returned orderd by quarter type.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        Task<IEnumerable<DataReturn>> FetchDataReturns(Scheme scheme, int complianceYear);
    }
}
