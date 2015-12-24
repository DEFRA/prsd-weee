namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Domain.Scheme;

    public interface IIssuePendingChargesDataAccess : ICommonDataAccess
    {
        /// <summary>
        /// Adds the specified invoice run to the database and saves changes.
        /// </summary>
        /// <param name="invoiceRun"></param>
        /// <returns></returns>
        Task SaveAsync(InvoiceRun invoiceRun);
    }
}
