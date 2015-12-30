namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public async Task<TransactionFile> CreateAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            TransactionFile transactionFile = new TransactionFile("WEE", fileID);

            foreach (MemberUpload memberUpload in invoiceRun.MemberUploads)
            {
                // TODO: Add "SubmittedDate" to the domain model for a member upload.
                DateTime submittedDate = memberUpload.UpdatedDate ?? memberUpload.CreatedDate;

                List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();

                string description = string.Format("Charge for producer registration submission made on {0:dd MMM yyyy}.",
                    submittedDate);

                InvoiceLineItem lineItem;
                try
                {
                    lineItem = new InvoiceLineItem(
                        memberUpload.TotalCharges,
                        description);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "An error occured creating an 1B1S invoice line item to represent the member upload with ID \"{0}\". " +
                        "See the inner exception for more details.",
                        memberUpload.Id);
                    throw new Exception(errorMessage, ex);
                }

                string transactionReference = await transactionReferenceGenerator.GetNextTransactionReferenceAsync();

                Invoice invoice;
                try
                {
                    invoice = new Invoice(
                        memberUpload.Scheme.IbisCustomerReference,
                        invoiceRun.CreatedDate,
                        TransactionType.Invoice,
                        transactionReference,
                        new List<InvoiceLineItem>() { lineItem });
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "An error occured creating an 1B1S invoice to represent the member upload with ID \"{0}\". " +
                        "See the inner exception for more details.",
                        memberUpload.Id);
                    throw new Exception(errorMessage, ex);
                }

                transactionFile.AddInvoice(invoice);
            }

            return transactionFile;
        }
    }
}
