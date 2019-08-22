namespace EA.Weee.Requests.Charges
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class FetchInvoiceRunCsv : IRequest<CSVFileData>
    {
        public Guid InvoiceRunId { get; private set; }

        public FetchInvoiceRunCsv(Guid invoiceRunId)
        {
            InvoiceRunId = invoiceRunId;
        }
    }
}
