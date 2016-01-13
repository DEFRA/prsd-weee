namespace EA.Weee.Requests.Charges
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class FetchInvoiceRunCsv : IRequest<CSVFileData>
    {
        public Guid InvoiceRunId { get; private set; }

        public FetchInvoiceRunCsv(Guid invoiceRunId)
        {
            InvoiceRunId = invoiceRunId;
        }
    }
}
