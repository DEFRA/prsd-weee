namespace EA.Weee.Ibis
{
    using System;

    /// <summary>
    /// Represents the address of a customer.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The first line of the invoicee's address.
        /// </summary>
        public string AddressLine1 { get; private set; }

        /// <summary>
        /// Additonal invoicee address lines.
        /// </summary>
        public string AddressLine2 { get; private set; }

        /// <summary>
        /// Additonal invoicee address lines.
        /// </summary>
        public string AddressLine3 { get; private set; }

        /// <summary>
        /// Additonal invoicee address lines.
        /// </summary>
        public string AddressLine4 { get; private set; }

        /// <summary>
        /// Town (if this information can be directly mapped).
        /// </summary>
        public string AddressLine5 { get; private set; }

        /// <summary>
        /// County (if this information can be directly mapped).
        /// </summary>
        public string AddressLine6 { get; private set; }

        /// <summary>
        /// Post code of the invoicee address.
        /// </summary>
        public string PostCode { get; private set; }

        /// <summary>
        /// Country of the invoicee address.
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Whether the invoicee address is UK-based.
        /// </summary>
        public bool IsUKAddress { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class with the specified details.
        /// </summary>
        /// <param name="addressLine1">The first line of the invoicee's address.</param>
        /// <param name="addressLine2">Additonal invoicee address lines.</param>
        /// <param name="addressLine3">Additonal invoicee address lines.</param>
        /// <param name="addressLine4">Additonal invoicee address lines.</param>
        /// <param name="addressLine5">Town (if this information can be directly mapped).</param>
        /// <param name="addressLine6">County (if this information can be directly mapped).</param>
        /// <param name="postCode">Post code of the invoicee address.</param>
        /// <param name="country">Country of the invoicee address.</param>
        /// <param name="isUKAddress">Whether the invoicee address is UK-based.</param>
        public Address(
            string addressLine1,
            string addressLine2,
            string addressLine3,
            string addressLine4,
            string addressLine5,
            string addressLine6,
            string postCode,
            string country,
            bool isUKAddress)
        {
            if (string.IsNullOrEmpty(addressLine1))
            {
                throw new ArgumentException("Address line 1 is mandatory.");
            }

            if (addressLine1.Length > 240)
            {
                throw new ArgumentException("Address line 1 cannot exceed 240 characters.");
            }

            if (addressLine2 != null && addressLine2.Length > 240)
            {
                throw new ArgumentException("Address line 2 cannot exceed 240 characters.");
            }

            if (addressLine3 != null && addressLine3.Length > 240)
            {
                throw new ArgumentException("Address line 3 cannot exceed 240 characters.");
            }

            if (addressLine4 != null && addressLine4.Length > 240)
            {
                throw new ArgumentException("Address line 4 cannot exceed 240 characters.");
            }

            if (addressLine5 != null && addressLine5.Length > 60)
            {
                throw new ArgumentException("Address line 5 cannot exceed 60 characters.");
            }

            if (addressLine6 != null && addressLine6.Length > 60)
            {
                throw new ArgumentException("Address line 6 cannot exceed 60 characters.");
            }

            if (string.IsNullOrEmpty(postCode))
            {
                throw new ArgumentException("The post code is mandatory.");
            }

            if (postCode.Length > 60)
            {
                throw new ArgumentException("The post code cannot exceed 60 characters.");
            }

            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            AddressLine3 = addressLine3;
            AddressLine4 = addressLine4;
            AddressLine5 = addressLine5;
            AddressLine6 = addressLine6;
            PostCode = postCode;
            Country = country;
            IsUKAddress = isUKAddress;
        }
    }
}