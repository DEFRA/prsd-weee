namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a line in a customer file that contains the details of a customer.
    /// </summary>
    internal class CustomerFileLine : IIbisFileLine
    {
        /// <summary>
        /// The customer represented by the file line.
        /// </summary>
        public Customer Customer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerFileLine"/> class with the specified customer.
        /// </summary>
        /// <param name="customer">The customer to which will be represented by this file line.</param>
        public CustomerFileLine(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            Customer = customer;
        }

        /// <summary>
        /// Returns the line type identifier of the file line.
        /// </summary>
        public string GetLineTypeIdentifier()
        {
            return "D";
        }

        /// <summary>
        /// Returns a list of data items related to the customer.
        /// </summary>
        public IEnumerable<string> GetDataItems()
        {
            yield return Customer.CustomerReference;
            yield return Customer.Name;
            yield return Customer.Address.AddressLine1;
            yield return Customer.Address.AddressLine2;
            yield return Customer.Address.AddressLine3;
            yield return Customer.Address.AddressLine4;
            yield return Customer.Address.AddressLine5;
            yield return Customer.Address.AddressLine6;
            yield return Customer.Address.IsUKAddress ?
                Customer.Address.PostCode :
                string.Format("{0}  {1}", Customer.Address.PostCode.Trim(), Customer.Address.Country); // Include 2 blank spaces when concatenating the postcode and country.
        }
    }
}