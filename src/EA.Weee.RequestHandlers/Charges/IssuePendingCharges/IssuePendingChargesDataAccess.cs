namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Domain.Scheme;

    public class IssuePendingChargesDataAccess : CommonDataAccess, IIssuePendingChargesDataAccess
    {
        public IssuePendingChargesDataAccess(WeeeContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Adds the specified invoice run to the database and saves changes.
        /// </summary>
        /// <param name="invoiceRun"></param>
        /// <returns></returns>
        public async Task SaveAsync(InvoiceRun invoiceRun)
        {
            Context.InvoiceRuns.Add(invoiceRun);
            await Context.SaveChangesAsync();
        }
    }
}
