namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Domain;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Charges;

    public class IssuePendingChargesDataAccess : CommonDataAccess, IIssuePendingChargesDataAccess
    {
        public IssuePendingChargesDataAccess(WeeeContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets the maximum file ID already in use by invoice runs in the database.
        /// If no invoice runs exist, returns null.
        /// </summary>
        /// <returns></returns>
        public async Task<ulong?> GetMaximumIbisFileIdOrDefaultAsync()
        {
            return (ulong?)await Context.InvoiceRuns
                .MaxAsync(ir => (long?)ir.IbisFileData.FileIdDatabaseValue);
        }

        /// <summary>
        /// Returns the initial 1B1S file ID from the system configuration
        /// as defined by the SystemData table.
        /// </summary>
        public async Task<ulong> GetInitialIbisFileIdAsync()
        {
            SystemData systemData = await Context.SystemData.SingleAsync();

            return systemData.InitialIbisFileId;
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
