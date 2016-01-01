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
        /// Gets the maximum file ID already in use by invoice runs in the database.
        /// If no invoice runs exist, returns null.
        /// </summary>
        /// <returns></returns>
        Task<ulong?> GetMaximumIbisFileIdOrDefaultAsync();

        /// <summary>
        /// Returns the initial 1B1S file ID from the system configuration.
        /// </summary>
        /// <returns></returns>
        Task<ulong> GetInitialIbisFileIdAsync();

        /// <summary>
        /// Adds the specified invoice run to the database and saves changes.
        /// </summary>
        /// <param name="invoiceRun"></param>
        /// <returns></returns>
        Task SaveAsync(InvoiceRun invoiceRun);
    }
}
