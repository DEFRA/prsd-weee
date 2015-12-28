namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Charges;
    using Domain.Scheme;
    using Ibis;

    /// <summary>
    /// Converts domain objects representing schemes, member uploads and producer submissions into
    /// IBIS object representing customers, invoices and invoice line items.
    /// </summary>
    public interface IIbisFileDataGenerator
    {
        InvoiceRunIbisFileData CreateFileData(ulong fileID, IReadOnlyList<MemberUpload> memberUploads);

        CustomerFile CreateCustomerFile(ulong fileID, IReadOnlyList<MemberUpload> memberUploads);

        TransactionFile CreateTransactionFile(ulong fileID, IReadOnlyList<MemberUpload> memberUploads);
    }
}
