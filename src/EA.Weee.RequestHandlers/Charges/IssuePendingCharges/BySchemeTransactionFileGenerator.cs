namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Ibis;

    /// <summary>
    /// This 1B1S transaction file generator creates a transaction file with one
    /// transaction for each scheme referenced by the specified list of member uploads.
    /// Each member upload appears as a line item.
    /// </summary>
    public class BySchemeTransactionFileGenerator : IIbisTransactionFileGenerator
    {
        private readonly ITransactionReferenceGenerator transactionReferenceGenerator;

        public BySchemeTransactionFileGenerator(ITransactionReferenceGenerator transactionReferenceGenerator)
        {
            this.transactionReferenceGenerator = transactionReferenceGenerator;
        }

        public async Task<TransactionFile> CreateAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            TransactionFile transactionFile = new TransactionFile("WEE", fileID);

            var groups = invoiceRun.MemberUploads.GroupBy(mu => mu.Scheme);

            foreach (var group in groups)
            {
                List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();

                foreach (MemberUpload memberUpload in group)
                {
                    DateTime submittedDate = memberUpload.SubmittedDate.Value;

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
                            "An error occurred creating an 1B1S invoice line item to represent the member upload with ID \"{0}\". " +
                            "See the inner exception for more details.",
                            memberUpload.Id);
                        throw new Exception(errorMessage, ex);
                    }

                    lineItems.Add(lineItem);
                }

                string transactionReference = await transactionReferenceGenerator.GetNextTransactionReferenceAsync();

                Invoice invoice;
                try
                {
                    invoice = new Invoice(
                        group.Key.IbisCustomerReference,
                        invoiceRun.CreatedDate,
                        TransactionType.Invoice,
                        transactionReference,
                        lineItems);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "An error occurred creating an 1B1S invoice to represent the member uploads for scheme with ID \"{0}\". " +
                        "See the inner exception for more details.",
                        group.Key.Id);
                    throw new Exception(errorMessage, ex);
                }

                transactionFile.AddInvoice(invoice);
            }

            return transactionFile;
        }
    }
}
