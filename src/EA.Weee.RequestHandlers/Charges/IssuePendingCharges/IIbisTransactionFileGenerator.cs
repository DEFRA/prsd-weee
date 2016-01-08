namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Charges;
    using EA.Weee.Ibis;

    /// <summary>
    /// A generator of 1B1S transaction files based on a list of member uploads.
    /// </summary>
    public interface IIbisTransactionFileGenerator
    {
        Task<TransactionFile> CreateAsync(ulong fileID, InvoiceRun invoiceRun);
    }
}