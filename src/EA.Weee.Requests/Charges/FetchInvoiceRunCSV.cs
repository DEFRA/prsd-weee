namespace EA.Weee.Requests.Charges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
