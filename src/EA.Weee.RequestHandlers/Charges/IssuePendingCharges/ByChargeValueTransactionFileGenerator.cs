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
    /// This 1B1S transcation file generator creates a transaction file with one invoice for each member upload.
    /// Invoice line items are formed by grouping by amount the non-zero, non-deleted producer submission
    /// charges within each member upload.
    /// </summary>
    public class ByChargeValueTransactionFileGenerator : IIbisTransactionFileGenerator
    {
        public Task<TransactionFile> CreateAsync(ulong fileID, IReadOnlyList<MemberUpload> memberUploads)
        {
            TransactionFile transactionFile = new TransactionFile("WEE", fileID);

            foreach (MemberUpload memberUpload in memberUploads)
            {
                List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();

                var lineItemGroups = memberUpload.ProducerSubmissions
                    .Where(ps => ps.ChargeThisUpdate != 0)
                    .Where(ps => ps.RegisteredProducer.IsAligned)
                    .GroupBy(ps => ps.ChargeThisUpdate)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Charge = g.Key, Quantity = g.Count() })
                    .ToList();

                if (lineItemGroups.Count > 0)
                {
                    foreach (var lineItemGroup in lineItemGroups)
                    {
                        decimal amount = lineItemGroup.Charge * lineItemGroup.Quantity;

                        string description = string.Format("{0} producer registration charge{1} at {2:C}.",
                            lineItemGroup.Quantity,
                            lineItemGroup.Quantity != 1 ? "s" : string.Empty,
                            lineItemGroup.Charge);

                        InvoiceLineItem lineItem;
                        try
                        {
                            lineItem = new InvoiceLineItem(
                                amount,
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

                    // TODO: Add "SubmittedDate" to the domain model for a member upload.
                    DateTime submittedDate = memberUpload.UpdatedDate ?? memberUpload.CreatedDate;

                    Invoice invoice;
                    try
                    {
                        invoice = new Invoice(
                            memberUpload.Scheme.IbisCustomerReference,
                            submittedDate,
                            TransactionType.Invoice,
                            transactionReference,
                            lineItems);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = string.Format(
                            "An error occured creating an 1B1S invoice to represent the member upload with ID \"{0}\"." +
                            "See the inner exception for more details.",
                            memberUpload.Id);
                        throw new Exception(errorMessage, ex);
                    }

                    transactionFile.AddInvoice(invoice);
                }
            }

            return Task.FromResult(transactionFile);
        }
    }
}
