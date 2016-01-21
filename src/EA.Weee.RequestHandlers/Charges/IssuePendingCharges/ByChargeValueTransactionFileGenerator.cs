namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Charges;
    using EA.Weee.Ibis;
    using MemberUpload = EA.Weee.Domain.Scheme.MemberUpload;

    /// <summary>
    /// This 1B1S transaction file generator creates a transaction file with one invoice for each member upload.
    /// Invoice line items are formed by grouping by amount the non-zero, non-deleted producer submission
    /// charges within each member upload.
    /// </summary>
    public class ByChargeValueTransactionFileGenerator : IIbisTransactionFileGenerator
    {
        private readonly ITransactionReferenceGenerator transactionReferenceGenerator;

        public ByChargeValueTransactionFileGenerator(ITransactionReferenceGenerator transactionReferenceGenerator)
        {
            this.transactionReferenceGenerator = transactionReferenceGenerator;
        }

        public async Task<IbisFileGeneratorResult<TransactionFile>> CreateAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

            var errors = new List<Exception>();
            TransactionFile transactionFile = new TransactionFile("WEE", fileID);

            foreach (MemberUpload memberUpload in invoiceRun.MemberUploads)
            {
                List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();

                var lineItemGroups = memberUpload.ProducerSubmissions
                    .Where(ps => ps.ChargeThisUpdate != 0)
                    .Where(ps => ps.Invoiced)
                    .GroupBy(ps => ps.ChargeThisUpdate)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Charge = g.Key, Quantity = g.Count() })
                    .ToList();

                if (lineItemGroups.Count > 0)
                {
                    var lineItemErrors = new List<Exception>();

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

                            lineItems.Add(lineItem);
                        }
                        catch (Exception ex)
                        {
                            lineItemErrors.Add(ex);
                        }
                    }

                    errors.AddRange(lineItemErrors);

                    if (lineItemErrors.Count == 0)
                    {
                        string transactionReference = await transactionReferenceGenerator.GetNextTransactionReferenceAsync();

                        try
                        {
                            Invoice invoice = new Invoice(
                                memberUpload.Scheme.IbisCustomerReference,
                                invoiceRun.IssuedDate,
                                TransactionType.Invoice,
                                transactionReference,
                                lineItems);

                            transactionFile.AddInvoice(invoice);
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                    }
                }
            }

            return new IbisFileGeneratorResult<TransactionFile>(errors.Count == 0 ? transactionFile : null, errors);
        }
    }
}
