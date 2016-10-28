namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;

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
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        Task<List<ProducerSubmission>> Fetch(string registrationNumber, int complianceYear);

        Task<IEnumerable<ProducerEeeByQuarter>> EeeOutputBySchemeAndComplianceYear(string registrationNumber, int complianceYear,
            Guid schemeId);
    }
}
