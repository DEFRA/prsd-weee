namespace EA.Weee.Ibis
{
    using System;

    /// <summary>
    /// Represents a single line item of an invoice.
    /// </summary>
    public class InvoiceLineItem
    {
        private decimal mAmountExcludingVAT;
        /// <summary>
        /// The amount of the line item in GBP, excluding VAT.
        /// </summary>
        public decimal AmountExcludingVAT 
        {
            get 
            { 
                return mAmountExcludingVAT;
            }
            private set 
            {
                // Note that limits are asymmetric to ensure that when formatted as a string to 2 
                // decimal places, the length will note exceed 13 characters.
                if (value > 9999999999)
                {
                    throw new ArgumentOutOfRangeException("The amount cannot exceed 9999999999.");
                }

                if (value < -999999999)
                {
                    throw new ArgumentOutOfRangeException("The amount cannot be less than -999999999.");
                }

                mAmountExcludingVAT = value; 
            }
        }

        private string mVatCode;
        public string VatCode
        {
            get { return mVatCode; }
            set
            {
                if (value != null && value.Length > 10)
                {
                    throw new ArgumentException("The VAT code cannot exceed 10 characters");
                }

                mVatCode = value;
            }
        }

        private string mAreaCode;
        /// <summary>
        /// The area code within the region. A list of codes used within the feeder system should be provided
        /// to EFAS income section so they can be mapped to the appropriate General Ledger Accounting Key segment values.
        /// </summary>
        public string AreaCode
        {
            get
            {
                return mAreaCode;
            }
            set
            {
                if (value != null && value.Length > 10)
                {
                    throw new ArgumentException("The area code cannot exceed 10 characters.");
                }

                mAreaCode = value;
            }
        }

        private string mDescription;
        /// <summary>
        /// Description.
        /// </summary>
        public string Description
        {
            get
            {
                return mDescription;
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("The description is mandatory.");
                }

                if (value.Length > 240)
                {
                    throw new ArgumentException("Length cannot exceed 240 characters.", "Description");
                }

                mDescription = value;
            }
        }

        private string mIncomeStreamCode;
        /// <summary>
        /// Income stream.
        /// </summary>
        public string IncomeStreamCode 
        {
            get
            {
                return mIncomeStreamCode;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("The income stream code is mandatory.");
                }

                if (value.Length > 5)
                {
                    throw new ArgumentException("The income stream code must not exceed 5 characters.");
                }

                mIncomeStreamCode = value;
            }
        }

        private string mContextCode;
        /// <summary>
        /// Context code.
        /// </summary>
        public string ContextCode
        {
            get
            {
                return mContextCode;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("The context code is mandatory.");
                }

                if (value.Length > 5)
                {
                    throw new ArgumentException("The context code must not exceed 5 characters.");
                }

                mContextCode = value;
            }
        }

        private UInt64 mQuantity;
        /// <summary>
        /// The quantity.
        /// </summary>
        public UInt64 Quantity 
        {
            get
            {
                return mQuantity;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("The quantity must be at least 1.");
                }

                // The limit is to ensure that when formatted as a string, 
                // the length will note exceed 13 characters.
                if (value > 9999999999999)
                {
                    throw new ArgumentOutOfRangeException("The quantity cannot exceed 9999999999999.");
                }

                mQuantity = value;
            }
        }
        
        /// <summary>
        /// The unit of measure.
        /// </summary>
        public UnitOfMeasure UnitOfMeasure { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItem"/> class with the specified details.
        /// </summary>
        /// <param name="amountExcludingVat">The amount of the line item in GBP, excluding VAT.</param>
        /// <param name="description">Description.</param>
        public InvoiceLineItem(
            decimal amountExcludingVat,
            string description)
        {
            // Note: lengths are checked in the set property methods.
            AmountExcludingVAT = amountExcludingVat;
            Description = description;

            // Set property default values
            VatCode = null;
            AreaCode = "H";
            IncomeStreamCode = "W";
            ContextCode = "H";
            Quantity = 1;
            UnitOfMeasure = UnitOfMeasure.Each;
        }
    }
}