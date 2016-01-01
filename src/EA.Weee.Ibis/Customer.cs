namespace EA.Weee.Ibis
{
    using System;

    /// <summary>
    /// Represents a customer that can to be added to a customer file.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// The customer number of the invoicee. This should be prefixed with the region code and
        /// suffixed with an identifier for the income stream or feeder system (as agreed with
        /// 1B1S income section).
        /// </summary>
        public string CustomerReference { get; private set; }

        /// <summary>
        /// The name of the invoicee.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The address of the invoicee.
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class with the specified details.
        /// </summary>
        /// <param name="customerReference">The customer number of the invoicee. This should be prefixed
        /// with the region code and suffixed with an identifier for the income stream or feeder system
        /// (as agreed with 1B1S income section).</param>
        /// <param name="name">The name of the invoicee.</param>
        /// <param name="address">The address of the invoicee.</param>
        public Customer(
            string customerReference,
            string name,
            Address address)
        {
            if (string.IsNullOrEmpty(customerReference))
            {
                throw new ArgumentException("The customer reference is mandatory.");
            }

            if (customerReference.Length > 12)
            {
                throw new ArgumentException("The customer reference cannot exceed 12 characters.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The customer name is mandatory.");
            }

            if (name.Length > 360)
            {
                throw new ArgumentException("The customer name cannot exceed 360 characters.");
            }

            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            CustomerReference = customerReference;
            Name = name;
            Address = address;
        }
    }
}