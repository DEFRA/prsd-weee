namespace EA.Weee.RequestHandlers.Charges.FetchPendingCharges
{
    using Domain.Charges;
    using EA.Prsd.Core.Mediator;
    using Security;
    using Shared;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Threading.Tasks;

    public class FetchInvoiceRunIbisZipFileHandler : IRequestHandler<Requests.Charges.FetchInvoiceRunIbisZipFile, Core.Shared.FileInfo>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess dataAccess;

        public FetchInvoiceRunIbisZipFileHandler(
            IWeeeAuthorization authorization,
            ICommonDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<Core.Shared.FileInfo> HandleAsync(Requests.Charges.FetchInvoiceRunIbisZipFile message)
        {
            authorization.EnsureCanAccessInternalArea(true);

            var invoiceRun = await dataAccess.FetchInvoiceRunAsync(message.InvoiceRunId);

            if (invoiceRun.IbisFileData == null)
            {
                var errorMessage = "The invoice run 1B1S zip file cannot be fetched for the invoice run " +
                                   $"with ID \"{message.InvoiceRunId}\" as it does not have 1B1S file data.";

                throw new InvalidOperationException(errorMessage);
            }

            var fileName = $"WEEE invoice files {invoiceRun.IbisFileData.FileId:D5} {invoiceRun.IssuedDate:yyyy-MM-dd}.zip";

            byte[] data = null;
            using (var stream = new MemoryStream())
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    var customerEntry = archive.CreateEntry(invoiceRun.IbisFileData.CustomerFileName);
                    using (var entryStream = customerEntry.Open())
                    {
                        var customerBytes = Encoding.UTF8.GetBytes(invoiceRun.IbisFileData.CustomerFileData);
                        await entryStream.WriteAsync(customerBytes, 0, customerBytes.Length);
                    }

                    var transactionEntry = archive.CreateEntry(invoiceRun.IbisFileData.TransactionFileName);
                    using (var entryStream = transactionEntry.Open())
                    {
                        var transactionBytes = Encoding.UTF8.GetBytes(invoiceRun.IbisFileData.TransactionFileData);
                        await entryStream.WriteAsync(transactionBytes, 0, transactionBytes.Length);
                    }

                    data = stream.ToArray();
                }
            }

            return new Core.Shared.FileInfo(fileName, data);
        }
    }
}
