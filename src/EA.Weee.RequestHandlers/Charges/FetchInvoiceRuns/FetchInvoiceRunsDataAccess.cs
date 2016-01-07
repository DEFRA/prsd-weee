namespace EA.Weee.RequestHandlers.Charges.FetchInvoiceRuns
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Charges;

    public class FetchInvoiceRunsDataAccess : CommonDataAccess, IFetchInvoiceRunsDataAccess
    {
        private readonly WeeeContext context;

        public FetchInvoiceRunsDataAccess(WeeeContext context)
            : base(context)
        {
            this.context = context;
        }

        /// <summary>
        /// Fetches all invoice runs for the specified authority.
        /// Results are ordered by issued date ascending.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<InvoiceRun>> FetchInvoiceRunsAsync(UKCompetentAuthority authority)
        {
            return await context.InvoiceRuns
                .Where(ir => ir.CompetentAuthority.Id == authority.Id)
                .OrderBy(ir => ir.IssuedDate)
                .ToListAsync();
        }
    }
}
