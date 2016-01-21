namespace EA.Weee.Requests.Charges
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    /// <summary>
    /// A request to fetch the ZIP file continaing the 1B1S customer and transaction
    /// files for an invoice run.
    /// </summary>
    public class FetchInvoiceRunIbisZipFile : IRequest<FileInfo>
    {
        public Guid InvoiceRunId{ get; private set; }

        public FetchInvoiceRunIbisZipFile(Guid invoiceRunId)
        {
            InvoiceRunId = invoiceRunId;
        }
    }
}
