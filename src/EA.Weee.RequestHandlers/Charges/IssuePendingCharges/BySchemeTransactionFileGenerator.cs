namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Ibis;

    /// <summary>
    /// This 1B1S transaction file generator creates a transaction file with one
    /// transaciton for each scheme referenced by the specified list of member uploads.
    /// Each member upload appears as a line item.
    /// </summary>
    public class BySchemeTransactionFileGenerator : IIbisTransactionFileGenerator
    {
        public Task<TransactionFile> CreateAsync(ulong fileID, IReadOnlyList<MemberUpload> memberUploads)
        {
            TransactionFile transactionFile = new TransactionFile("WEE", fileID);

            var groups = memberUploads.GroupBy(mu => mu.Scheme);

            foreach (var group in groups)
            {
                List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();

                foreach (MemberUpload memberUpload in group)
                {
                    // TODO: Add "SubmittedDate" to the domain model for a member upload.
                    DateTime submittedDate = memberUpload.UpdatedDate ?? memberUpload.CreatedDate;

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

                    lineItems.Add(lineItem);
                }

                // TODO: What format is the transaction reference?
                string transactionReference = "WEE1000001H";

                Invoice invoice;
                try
                {
                    invoice = new Invoice(
                        group.Key.IbisCustomerReference,
                        DateTime.UtcNow,
                        TransactionType.Invoice,
                        transactionReference,
                        lineItems);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "An error occured creating an 1B1S invoice to represent the member uploads for scheme with ID \"{0}\". " +
                        "See the inner exception for more details.",
                        group.Key.Id);
                    throw new Exception(errorMessage, ex);
                }

                transactionFile.AddInvoice(invoice);
            }

            return Task.FromResult(transactionFile);
        }
    }
}
