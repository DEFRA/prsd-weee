namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a line in a transaction file that contains the details of a line item of an invoice..
    /// </summary>
    internal class InvoiceLineItemFileLine : IIbisFileLine
    {
        /// <summary>
        /// The invoice represented by this file line.
        /// </summary>
        public Invoice Invoice { get; private set; }

        /// <summary>
        /// The invoice line item represented by this file line.
        /// </summary>
        public InvoiceLineItem InvoiceLineItem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItemFileLine"/> class with the specified details.
        /// </summary>
        /// <param name="invoice">The invoice represented by this file line.</param>
        /// <param name="invoiceLineItem">The invoice line item represented by this file line.</param>
        public InvoiceLineItemFileLine(
            Invoice invoice,
            InvoiceLineItem invoiceLineItem)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException("invoice");
            }

            if (invoiceLineItem == null)
            {
                throw new ArgumentNullException("invoiceLineItem");
            }

            Invoice = invoice;
            InvoiceLineItem = invoiceLineItem;
        }

        /// <summary>
        /// Returns the line type identifier of the invoice line item file line.
        /// </summary>
        public string GetLineTypeIdentifier()
        {
            return "D";
        }

        /// <summary>
        /// Returns a list of data items related to the invoice line item.
        /// </summary>
        public IEnumerable<string> GetDataItems()
        {
            yield return Invoice.CustomerReference;
            yield return Invoice.TransactionDate.ToString("dd-MMM-yyyy").ToUpperInvariant();
            yield return ConvertTransactionTypeToString(Invoice.TransactionType);
            yield return Invoice.TransactionReference;
            yield return Invoice.RelatedTransactionReference;
            yield return ConvertCurrencyCodeToString(Invoice.CurrencyCode);
            yield return Invoice.TransactionHeaderNarrative;
            yield return Invoice.TransactionDate.ToString("dd-MMM-yyyy").ToUpperInvariant(); // Header Attribute 1
            yield return string.Empty; // Header Attribute 2
            yield return string.Empty; // Header Attribute 3
            yield return string.Empty; // Header Attribute 4
            yield return string.Empty; // Header Attribute 5
            yield return string.Empty; // Header Attribute 6
            yield return string.Empty; // Header Attribute 7
            yield return string.Empty; // Header Attribute 8
            yield return string.Empty; // Header Attribute 9
            yield return string.Empty; // Header Attribute 10
            yield return TransactionFile.FormatCurrencyInPence(InvoiceLineItem.AmountExcludingVAT);
            yield return InvoiceLineItem.VatCode;
            yield return InvoiceLineItem.AreaCode;
            yield return InvoiceLineItem.Description;
            yield return InvoiceLineItem.IncomeStreamCode;
            yield return InvoiceLineItem.ContextCode;
            yield return string.Empty; // Line Attribute 1
            yield return string.Empty; // Line Attribute 2
            yield return string.Empty; // Line Attribute 3
            yield return string.Empty; // Line Attribute 4
            yield return string.Empty; // Line Attribute 5
            yield return string.Empty; // Line Attribute 6
            yield return string.Empty; // Line Attribute 7
            yield return string.Empty; // Line Attribute 8
            yield return string.Empty; // Line Attribute 9
            yield return string.Empty; // Line Attribute 10
            yield return string.Empty; // Line Attribute 11
            yield return string.Empty; // Line Attribute 12
            yield return string.Empty; // Line Attribute 13
            yield return string.Empty; // Line Attribute 14
            yield return string.Empty; // Line Attribute 15
            yield return InvoiceLineItem.Quantity.ToString();
            yield return ConvertUnitOfMeasureToString(InvoiceLineItem.UnitOfMeasure);
            yield return TransactionFile.FormatCurrencyInPence(InvoiceLineItem.AmountExcludingVAT); // UOM selling price
        }

        private string ConvertTransactionTypeToString(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.CreditNote:
                    return "C";

                case TransactionType.Invoice:
                    return "I";

                default:
                    throw new NotSupportedException();
            }
        }

        private string ConvertCurrencyCodeToString(CurrencyCode currencyCode)
        {
            switch (currencyCode)
            {
                case CurrencyCode.GBP:
                    return "GBP";

                default:
                    throw new NotSupportedException();
            }
        }

        private string ConvertUnitOfMeasureToString(UnitOfMeasure unitOfMeasure)
        {
            switch (unitOfMeasure)
            {
                case UnitOfMeasure.Each:
                    return "Each";

                default:
                    throw new NotSupportedException();
            }
        }
    }
}