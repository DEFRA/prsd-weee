namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an Ibis transaction file.
    /// </summary>
    public sealed class TransactionFile : IbisFile
    {
        private List<Invoice> mInvoices = new List<Invoice>();
        
        /// <summary>
        /// The collection of invoices associated with the transaction file.
        /// </summary>
        public IReadOnlyList<Invoice> Invoices
        {
            get { return mInvoices; }
        }

        private string mBillRunId;
        
        /// <summary>
        /// The feeder system bill run identifier (if available).
        /// </summary>
        public string BillRunId
        {
            get { return mBillRunId; }
            set
            {
                if (value != null && value.Length > 20)
                {
                    throw new ArgumentException("The bill run ID cannot exceed 20 characters.");
                }

                mBillRunId = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionFile"/> class with the specified file source and file ID.
        /// </summary>
        /// <param name="fileSource">The 3 letter code for the feeder source assigned by 1B1S income section.</param>
        /// <param name="fileID">The ID of the Ibis file.</param>
        public TransactionFile(string fileSource, ulong fileID)
            : base(fileSource, fileID)
        {
        }

        /// <summary>
        /// Adds an invoice to the collection.
        /// </summary>
        /// <param name="invoice">The invoice to add to the collection.</param>
        public void AddInvoice(Invoice invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException("invoice");
            }

            mInvoices.Add(invoice);
        }

        /// <summary>
        /// Returns a colection of IIbisFileLine items to represent the collection of invoices. 
        /// </summary>
        protected override IEnumerable<IIbisFileLine> GetLines()
        {
            // Loop through each invoice in the collection
            foreach (Invoice invoice in mInvoices)
            {
                // Loop through each invoice line item in the invoice
                foreach (InvoiceLineItem lineItem in invoice.LineItems)
                {
                    // Return an invoice line item for each item
                    yield return new InvoiceLineItemFileLine(invoice, lineItem);
                }
            }
        }

        /// <summary>
        /// Returns the number of lines that will be written, excluding the header and footer lines.
        /// </summary>
        /// <returns></returns>
        protected override int GetLineCount()
        {
            return mInvoices.Sum(i => i.LineItems.Count);
        }

        /// <summary>
        /// Returns additional header data items specific to the transaciton file.
        /// </summary>
        protected override IEnumerable<string> GetAdditionalHeaderDataItems()
        {
            yield return BillRunId;
        }

        /// <summary>
        /// Returns additional footer data items specific to the transaciton file.
        /// </summary>
        protected override IEnumerable<string> GetAdditionalFooterDataItems()
        {
            decimal sumOfInvoices = mInvoices
                .Where(i => i.TransactionType == TransactionType.Invoice)
                .Sum(i => i.TransactionTotal);

            decimal sumOfCreditNotes = mInvoices
                .Where(i => i.TransactionType == TransactionType.CreditNote)
                .Sum(i => i.TransactionTotal);

            // Currencies are rendered in pence.
            yield return FormatCurrencyInPence(sumOfInvoices);
            yield return FormatCurrencyInPence(sumOfCreditNotes);
        }

        /// <summary>
        /// Returns the file type identifier of the Ibis file.
        /// </summary>
        /// <returns></returns>
        protected override string GetFileTypeIdentifier()
        {
            return "I";
        }

        /// <summary>
        /// Formats an amount in GBP as a string in pence.
        /// For example, £1,234.56 would be formatter as "123456".
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string FormatCurrencyInPence(decimal amount)
        {
            return ((int)(amount * 100)).ToString("D");
        }
    }
}