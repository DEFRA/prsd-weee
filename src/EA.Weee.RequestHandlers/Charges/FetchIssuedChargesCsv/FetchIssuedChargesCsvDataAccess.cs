﻿namespace EA.Weee.RequestHandlers.Charges.FetchIssuedChargesCsv
{
    using DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;
    using Shared;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class FetchIssuedChargesCsvDataAccess : CommonDataAccess, IFetchIssuedChargesCsvDataAccess
    {
        public FetchIssuedChargesCsvDataAccess(WeeeContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns all producer submissions which have been invoiced with a non-zero charge, filtered by authority and compliance year
        /// and optionally by scheme name.
        /// 
        /// Results are ordered by scheme name ascending, then member upload submitted date ascending and then by PRN ascending.
        /// 
        /// The member upload, invoice run, producer business, company details, partnership, charge band amount, registered producer
        /// and registered producer scheme will be pre-loaded with each result.
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="complianceYear"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProducerSubmission>> FetchInvoicedProducerSubmissionsAsync(UKCompetentAuthority authority, int complianceYear, Guid? schemeId)
        {
            return await Context.AllProducerSubmissions
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
                .Where(ps => ps.RegisteredProducer.Scheme.Id == schemeId || schemeId == null)
                .Where(ps => ps.Invoiced)
                .Where(ps => ps.ChargeThisUpdate > 0)
                .OrderBy(ps => ps.RegisteredProducer.Scheme.SchemeName)
                .ThenBy(ps => ps.MemberUpload.SubmittedDate)
                .ThenBy(ps => ps.RegisteredProducer.ProducerRegistrationNumber)
                .ToListAsync();
        }

        public async Task<Domain.Scheme.Scheme> FetchSchemeAsync(Guid? schemeId)
        {
            var scheme = await Context.Schemes.SingleOrDefaultAsync(s => s.Id == schemeId);

            if (scheme != null)
            {
                return scheme;
            }
            throw new InvalidOperationException(string.Format("Scheme with id '{0}' does not exist", schemeId));
        }
    }
}
