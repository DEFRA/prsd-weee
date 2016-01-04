namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represents an invoice that can be added to a transaction file.
    /// An invoice has one or more <see cref="InvoiceLineItem"/>s.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// The collection of line items associated with the invoice.
        /// </summary>
        public IReadOnlyList<InvoiceLineItem> LineItems { get; private set; }

        private string mCustomerReference;
        /// <summary>
        /// The customer number of the invoicee. This should be prefixed with the region code and
        /// suffixed with an identifier for the income stream or feeder system (as agreed with
        /// 1B1S income section).
        /// </summary>
        public string CustomerReference
        { 
            get 
            { 
                return mCustomerReference; 
            }
            private set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("The customer reference is mandatory.");
                }

                if (value.Length > 12)
                {
                    throw new ArgumentException("The customer reference cannot exceed 12 characters.");
                }

                mCustomerReference = value; 
            }
        }

        /// <summary>
        /// The date of the invoice in the feeder system.
        /// </summary>
        public DateTime TransactionDate { get; private set; }

        /// <summary>
        /// Whether it is an invoice or credit note.
        /// </summary>
        public TransactionType TransactionType { get; private set; }

        private string mTransactionReference;
        /// <summary>
        /// The invoice number. This must be unique in 1B1S, preferably in the form
        /// "AnnnnnnnnR", where A is a code for the feeder system or income stream
        /// assigned by 1B1S Income section, and R is the regional identifier.
        /// </summary>
        public string TransactionReference
        { 
            get 
            { 
                return mTransactionReference; 
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("The transaction reference is mandatory.");
                }

                if (value.Length > 18)
                {
                    throw new ArgumentException("The transaction reference cannot exceed 20 characters.");
                }

                mTransactionReference = value; 
            }
        }

        private string mRelatedTransactionReference;
        /// <summary>
        /// Blank if transaction is an invoice, optional if the transaction is a credit note.
        /// This will be the transaction reference of the invoice to which the credit note relates.
        /// </summary>
        public string RelatedTransactionReference
        {
            get 
            { 
                return mRelatedTransactionReference; 
            }
            set
            {
                if (value != null && value.Length > 18)
                {
                    throw new ArgumentException("The related transaction reference cannot exceed 20 characters.");
                }

                mRelatedTransactionReference = value;
            }
        }

        /// <summary>
        /// The currency code.
        /// </summary>
        public CurrencyCode CurrencyCode { get; set; }

        private string mTransactionHeaderNarrative;
        /// <summary>
        /// The transaction header narrative is mapped individually for each feeder system or income stream variant
        /// For example, it could be the combination of Line Income Stream Code and Line Context Code.
        /// </summary>
        public string TransactionHeaderNarrative 
        {
            get 
            { 
                return mTransactionHeaderNarrative; 
            }
            set
            {
                if (value != null && value.Length > 240)
                {
                    throw new ArgumentException("The transaction header narrative cannot exceed 240 characters.");
                }

                mTransactionHeaderNarrative = value;
            }
        }

        /// <summary>
        /// The transaction total, which is the sum of the line item amounts
        /// contained within the invoice.
        /// </summary>
        public decimal TransactionTotal
        {
            get { return LineItems.Sum(li => li.AmountExcludingVAT); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Invoice"/> class with the specified details.
        /// </summary>
        /// <param name="customerReference">The customer number of the invoicee. This should be prefixed with the
        /// region code and suffixed with an identifier for the income stream or feeder system (as agreed with
        /// 1B1S income section).</param>
        /// <param name="transactionDate">The date of the invoice in the feeder system.</param>
        /// <param name="transactionReference">The invoice number. This must be unique in 1B1S, preferably in the form
        /// "AnnnnnnnnR", where A is a code for the feeder system or income stream assigned by 1B1S Income section, and
        /// R is the regional identifier.</param>
        public Invoice(
            string customerReference,
            DateTime transactionDate,
            TransactionType transactionType,
            string transactionReference,
            IReadOnlyList<InvoiceLineItem> lineItems)
        {
            if (lineItems == null)
            {
                throw new ArgumentNullException("lineItems");
            }

            if (lineItems.Count == 0)
            {
                throw new ArgumentException("At least one line item must be provided.");
            }

            // Note: lengths are checked in the set property methods.
            CustomerReference = customerReference;
            TransactionDate = transactionDate;
            TransactionType = transactionType;
            TransactionReference = transactionReference;
            LineItems = lineItems;

            // Set default property values
            RelatedTransactionReference = null;
            CurrencyCode = CurrencyCode.GBP;
            TransactionHeaderNarrative = null;

            EnsureTotalValidForTransactionType();
        }

        /// <summary>
        /// Invoices must have a positive total. Credit notes must have a negative total.
        /// </summary>
        private void EnsureTotalValidForTransactionType()
        {
            switch (TransactionType)
            {
                case TransactionType.CreditNote:
                    if (TransactionTotal >= 0)
                    {
                        string errorMessage = "The transaction total for a credit note must be negative.";
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;

                case TransactionType.Invoice:
                    if (TransactionTotal <= 0)
                    {
                        string errorMessage = "The transaction total for an invoice must be positive.";
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}