namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an Ibis customer file.
    /// </summary>
    public sealed class CustomerFile : IbisFile
    {
        private List<Customer> mCustomers = new List<Customer>();
        
        /// <summary>
        /// The collection of customers associated with the customer file.
        /// </summary>
        public IReadOnlyList<Customer> Customers
        {
            get { return mCustomers; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerFile"/> class with the specified file source and file ID.
        /// </summary>
        /// <param name="fileSource">The 3 letter code for the feeder source assigned by 1B1S income section.</param>
        /// <param name="fileID">The ID of the Ibis file.</param>
        public CustomerFile(string fileSource, ulong fileID)
            : base(fileSource, fileID)
        {
        }

        /// <summary>
        /// Adds a customer to the collection.
        /// </summary>
        /// <param name="customer">The customer to add to the collection.</param>
        public void AddCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            mCustomers.Add(customer);
        }

        /// <summary>
        /// Returns a colection of IIbisFileLine items to represent the collection of customers. 
        /// </summary>
        protected override IEnumerable<IIbisFileLine> GetLines()
        {
            foreach (Customer customer in mCustomers)
            {
                yield return new CustomerFileLine(customer);
            }
        }

        /// <summary>
        /// Returns the number of lines that will be written, excluding the header and footer lines.
        /// </summary>
        /// <returns></returns>
        protected override int GetLineCount()
        {
            return mCustomers.Count;
        }

        /// <summary>
        /// Returns the file type identifier of the Ibis file.
        /// </summary>
        protected override string GetFileTypeIdentifier()
        {
            return "C";
        }
    }
}