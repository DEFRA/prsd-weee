namespace EA.Weee.RequestHandlers.Charges
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain;
    using Domain.Charges;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Scheme;

    public class CommonDataAccess : ICommonDataAccess
    {
        protected readonly WeeeContext Context;

        public CommonDataAccess(WeeeContext context)
        {
            Context = context;
        }

        private readonly Dictionary<CompetentAuthority, string> authorityMapping = new Dictionary<CompetentAuthority, string>()
        {
            { CompetentAuthority.England, "Environment Agency" },
            { CompetentAuthority.Scotland, "Scottish Environment Protection Agency" },
            { CompetentAuthority.NorthernIreland, "Northern Ireland Environment Agency" },
            { CompetentAuthority.Wales, "Natural Resources Wales" },
        };

        /// <summary>
        /// Fetches the domain object representing the specified competent authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public async Task<UKCompetentAuthority> FetchCompetentAuthority(CompetentAuthority authority)
        {
            string authorityName = authorityMapping[authority];

            return await Context.UKCompetentAuthorities.SingleAsync(ca => ca.Name == authorityName);
        }

        /// <summary>
        /// Returns all member uploads for the specified authority which are submitted, have a positive total charge
        /// and are not yet assigned to an invoice run.
        /// Results will be ordered by scheme name ascending and then compliance year descending.
        /// The scheme and UK competent authority domain objects will be pre-loaded with each member upload returned.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<MemberUpload>> FetchSubmittedNonInvoicedMemberUploadsAsync(UKCompetentAuthority authority)
        {
            return await Context.MemberUploads
                .Include(mu => mu.Scheme)
                .Include(mu => mu.Scheme.CompetentAuthority)
                .Where(mu => mu.IsSubmitted)
                .Where(mu => mu.InvoiceRun == null)
                .Where(mu => mu.Scheme.CompetentAuthority.Id == authority.Id)
                .Where(mu => mu.TotalCharges > 0)
                .OrderBy(mu => mu.Scheme.SchemeName)
                .ThenByDescending(mu => mu.ComplianceYear)
                .ToListAsync();
        }

        /// <summary>
        /// Returns th invoice run with the specified ID.
        /// The 1B1S file data domain object will be pre-loaded where it is available.
        /// </summary>
        /// <param name="invoiceRunId"></param>
        /// <returns></returns>
        public async Task<InvoiceRun> FetchInvoiceRunAsync(Guid invoiceRunId)
        {
            InvoiceRun invoiceRun = await Context.InvoiceRuns
                .Include(ir => ir.IbisFileData)
                .SingleOrDefaultAsync(ir => ir.Id == invoiceRunId);

            if (invoiceRun == null)
            {
                string errorMessage = string.Format(
                    "No invoice run with ID \"{0}\" was found in the database",
                    invoiceRunId);

                throw new Exception(errorMessage);
            }

            return invoiceRun;
        }
    }
}
