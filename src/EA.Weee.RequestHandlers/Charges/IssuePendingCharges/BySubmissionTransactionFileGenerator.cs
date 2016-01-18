namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Charges;
    using EA.Weee.Ibis;
    using MemberUpload = EA.Weee.Domain.Scheme.MemberUpload;

    /// <summary>
    /// This 1B1S transaction file generator creates a transaction file with one invoice for each member upload.
    /// Each invoice has a single line item.
    /// </summary>
    public class BySubmissionTransactionFileGenerator : IIbisTransactionFileGenerator
    {
        private readonly ITransactionReferenceGenerator transactionReferenceGenerator;

        public BySubmissionTransactionFileGenerator(ITransactionReferenceGenerator transactionReferenceGenerator)
        {
            this.transactionReferenceGenerator = transactionReferenceGenerator;
        }

        public async Task<IbisFileGeneratorResult<TransactionFile>> CreateAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            TransactionFile transactionFile = new TransactionFile("WEE", fileID);
            var errors = new List<Exception>();

            foreach (MemberUpload memberUpload in invoiceRun.MemberUploads)
            {
                DateTime submittedDate = memberUpload.SubmittedDate.Value;

                string description = string.Format("Charge for producer registration submission made on {0:dd MMM yyyy}.",
                    submittedDate);
                try
                {
                    InvoiceLineItem lineItem = new InvoiceLineItem(
                            memberUpload.TotalCharges,
                            description);

                    string transactionReference = await transactionReferenceGenerator.GetNextTransactionReferenceAsync();

                    Invoice invoice = new Invoice(
                            memberUpload.Scheme.IbisCustomerReference,
                            invoiceRun.IssuedDate,
                            TransactionType.Invoice,
                            transactionReference,
                            new List<InvoiceLineItem>() { lineItem });

                    transactionFile.AddInvoice(invoice);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            return new IbisFileGeneratorResult<TransactionFile>(errors.Count == 0 ? transactionFile : null, errors);
        }
    }
}
