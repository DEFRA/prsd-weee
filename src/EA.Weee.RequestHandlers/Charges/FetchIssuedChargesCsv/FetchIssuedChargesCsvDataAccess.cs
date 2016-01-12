namespace EA.Weee.RequestHandlers.Charges.FetchIssuedChargesCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;

    public class FetchIssuedChargesCsvDataAccess : CommonDataAccess, IFetchIssuedChargesCsvDataAccess
    {
        public FetchIssuedChargesCsvDataAccess(WeeeContext context)
            : base(context)
        {
        }

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
        public async Task<IEnumerable<ProducerSubmission>> FetchInvoicedProducerSubmissionsAsync(UKCompetentAuthority authority, int complianceYear, string schemeName)
        {
            return await Context.ProducerSubmissions
                .Include(ps => ps.MemberUpload)
                .Include(ps => ps.MemberUpload.InvoiceRun)
                .Include(ps => ps.ProducerBusiness)
                .Include(ps => ps.ProducerBusiness.CompanyDetails)
                .Include(ps => ps.ProducerBusiness.Partnership)
                .Include(ps => ps.ChargeBandAmount)
                .Include(ps => ps.RegisteredProducer)
                .Include(ps => ps.RegisteredProducer.Scheme)
                .Where(ps => ps.RegisteredProducer.Scheme.CompetentAuthority.Id == authority.Id)
                .Where(ps => ps.RegisteredProducer.ComplianceYear == complianceYear)
                .Where(ps => ps.RegisteredProducer.Scheme.SchemeName == schemeName || schemeName == null)
                .Where(ps => ps.Invoiced)
                .Where(ps => ps.ChargeThisUpdate > 0)
                .OrderBy(ps => ps.RegisteredProducer.Scheme.SchemeName)
                .ThenBy(ps => ps.MemberUpload.SubmittedDate)
                .ThenBy(ps => ps.RegisteredProducer.ProducerRegistrationNumber)
                .ToListAsync();
        }
    }
}
