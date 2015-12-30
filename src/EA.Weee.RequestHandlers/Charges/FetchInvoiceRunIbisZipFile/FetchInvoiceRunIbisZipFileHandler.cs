namespace EA.Weee.RequestHandlers.Charges.FetchPendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Charges;
    using Core.Shared;
    using Domain;
    using Domain.Charges;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.Scheme;
    using Ionic.Zip;
    using Security;

    public class FetchInvoiceRunIbisZipFileHandler : IRequestHandler<Requests.Charges.FetchInvoiceRunIbisZipFile, FileInfo>
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

        public async Task<FileInfo> HandleAsync(Requests.Charges.FetchInvoiceRunIbisZipFile message)
        {
            authorization.EnsureCanAccessInternalArea(true);

            InvoiceRun invoiceRun = await dataAccess.FetchInvoiceRunAsync(message.InvoiceRunId);

            if (invoiceRun.IbisFileData == null)
            {
                string errorMessage = string.Format(
                    "The invoice run 1B1S zip file cannot be fetched for the invoice run " +
                    "with ID \"{0}\" as it does not have 1B1S file data.",
                    message.InvoiceRunId);

                throw new InvalidOperationException(errorMessage);
            }

            // TODO: Add invoice run created date to the filename. E.g. "WEEE invoice files 00500 2015-12-31.zip"
            string fileName = string.Format(
                "WEEE invoice files {0:D5}.zip",
                invoiceRun.IbisFileData.FileId);

            byte[] data = null;
            using (var stream = new System.IO.MemoryStream())
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddEntry(invoiceRun.IbisFileData.CustomerFileName, invoiceRun.IbisFileData.CustomerFileData);
                    zip.AddEntry(invoiceRun.IbisFileData.TransactionFileName, invoiceRun.IbisFileData.TransactionFileData);
                    zip.Save(stream);

                    data = stream.ToArray();
                }
            }

            return new FileInfo(fileName, data);
        }
    }
}
