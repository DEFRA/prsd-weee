namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using EA.Weee.Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGetProducerDetailsDataAccess
    {
        /// <summary>
        /// Fetches all submitted producer registrations with the specified
        /// registration number. The results will not be deterministically
        /// ordered.
        /// 
        /// All producer entities will be returned with member uploads,
        /// producer business, company and partnership relationships pre-loaded.
        /// 
        /// The returned entities will not be tracked for changes.
        /// </summary>
        /// <param name="registrationNumber"></param>
        /// <returns></returns>
        Task<List<Producer>> Fetch(string registrationNumber);
    }
}
