namespace EA.Weee.RequestHandlers.Charges.FetchIssuedChargesCsv
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;

    public interface IFetchIssuedChargesCsvDataAccess : ICommonDataAccess
    {
        /// <summary>
        /// Returns all producer submissions which have been been invoiced with a non-zero charge, filtered by authority and compliance year
        /// and optionally by scheme name.
        /// 
        /// Results are ordered by scheme name ascending, then member upload submitted date ascending and then by PRN ascending.
        /// 
        /// The member upload, invoice run, producer business, company details, partnership, charge band amount, registered producer
        /// and registered producer scheme will be pre-loaded with each result.
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="complianceYear"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        Task<IEnumerable<ProducerSubmission>> FetchInvoicedProducerSubmissionsAsync(UKCompetentAuthority authority, int complianceYear, string schemeName);
    }
}
