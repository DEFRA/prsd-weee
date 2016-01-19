namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Ibis;
    using Errors;

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

        public async Task<IbisFileGeneratorResult<TransactionFile>> CreateAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            TransactionFile transactionFile = new TransactionFile("WEE", fileID);

            var errors = new List<Exception>();
            var groups = invoiceRun.MemberUploads.GroupBy(mu => mu.Scheme);

            foreach (var group in groups)
            {
                List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();
                var lineItemErrors = new List<Exception>();

                foreach (MemberUpload memberUpload in group)
                {
                    DateTime submittedDate = memberUpload.SubmittedDate.Value;

                    string description = string.Format("Charge for producer registration submission made on {0:dd MMM yyyy}.",
                        submittedDate);

                    try
                    {
                        InvoiceLineItem lineItem = new InvoiceLineItem(
                            memberUpload.TotalCharges,
                            description);

                        lineItems.Add(lineItem);
                    }
                    catch (Exception ex)
                    {
                        lineItemErrors.Add(new SchemeFieldException(group.Key, ex));
                    }
                }

                errors.AddRange(lineItemErrors);

                string transactionReference = await transactionReferenceGenerator.GetNextTransactionReferenceAsync();

                if (lineItemErrors.Count == 0)
                {
                    try
                    {
                        Invoice invoice = new Invoice(
                            group.Key.IbisCustomerReference,
                            invoiceRun.IssuedDate,
                            TransactionType.Invoice,
                            transactionReference,
                            lineItems);

                        transactionFile.AddInvoice(invoice);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new SchemeFieldException(group.Key, ex));
                    }
                }
            }

            return new IbisFileGeneratorResult<TransactionFile>(errors.Count == 0 ? transactionFile : null, errors);
        }
    }
}
