namespace EA.Weee.RequestHandlers.Charges.FetchInvoiceRunCSV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Charges;

    public class FetchInvoiceRunCSVHandler : IRequestHandler<Requests.Charges.FetchInvoiceRunCSV, CSVFileData>
    {
        public Task<CSVFileData> HandleAsync(FetchInvoiceRunCSV message)
        {
            throw new NotImplementedException();
        }
    }
}
