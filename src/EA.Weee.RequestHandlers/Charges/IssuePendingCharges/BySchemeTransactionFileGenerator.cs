﻿namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Ibis;
    using Errors;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// This 1B1S transaction file generator creates a transaction file with one
    /// transaction for each scheme referenced by the specified list of member uploads.
    /// Each member upload appears as a line item.
    /// </summary>
    public class BySchemeTransactionFileGenerator : IIbisTransactionFileGenerator
    {
        private readonly ITransactionReferenceGenerator transactionReferenceGenerator;
        private const string CommonMessageString = "Charge for producer registration submission made on {0:dd MMM yyyy}";

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
                    var submittedDate = memberUpload.SubmittedDate.Value;
                    var competantAuthorityAnnualChargeAmount = memberUpload.Scheme.CompetentAuthority.AnnualChargeAmount;

                    var description = string.Format("{0}.", CommonMessage(submittedDate));

                    if (memberUpload.HasAnnualCharge && competantAuthorityAnnualChargeAmount > 0)
                    {
                        description = string.Format("{0} and the {1} annual charge.", CommonMessage(submittedDate), competantAuthorityAnnualChargeAmount.Value.ToString("#,##0.00"));
                    }

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
        private string CommonMessage(DateTime date)
        {
            return string.Format(CommonMessageString, date);
        }
    }
}
