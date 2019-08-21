﻿namespace EA.Weee.RequestHandlers.Charges.FetchPendingCharges
{
    using Core.Shared;
    using Domain.Charges;
    using EA.Prsd.Core.Mediator;
    using Ionic.Zip;
    using Security;
    using Shared;
    using System;
    using System.Threading.Tasks;

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

            string fileName = string.Format(
                "WEEE invoice files {0:D5} {1:yyyy-MM-dd}.zip",
                invoiceRun.IbisFileData.FileId,
                invoiceRun.IssuedDate);

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
