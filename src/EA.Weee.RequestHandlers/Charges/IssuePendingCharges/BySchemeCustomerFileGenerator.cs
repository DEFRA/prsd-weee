namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Charges;
    using Ibis;
    using Scheme = EA.Weee.Domain.Scheme.Scheme;

    /// <summary>
    /// This 1B1S customer file generator creates a customer file with one customer for each distinct scheme
    /// referenced by the list of member uploads.
    /// </summary>
    public class BySchemeCustomerFileGenerator : IIbisCustomerFileGenerator
    {
        public Task<CustomerFile> CreateAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            CustomerFile customerFile = new CustomerFile("WEE", fileID);

            IEnumerable<Scheme> schemes = invoiceRun.MemberUploads
                .Select(mu => mu.Scheme)
                .Distinct();

            foreach (Scheme scheme in schemes)
            {
                Address address;
                try
                {
                    var postCode = GetIbisPostCode(scheme.Organisation.OrganisationAddress);

                    address = new Address(
                        scheme.Organisation.Contact.FullName,
                        scheme.Organisation.OrganisationAddress.Address1,
                        scheme.Organisation.OrganisationAddress.Address2,
                        null,
                        scheme.Organisation.OrganisationAddress.TownOrCity,
                        scheme.Organisation.OrganisationAddress.CountyOrRegion,
                        postCode);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "An error occurred creating an 1B1S address to represent the scheme with ID \"{0}\". " +
                        "See the inner exception for more details.",
                        scheme.Id);
                    throw new Exception(errorMessage, ex);
                }

                Customer customer;

                try
                {
                    customer = new Customer(
                        scheme.IbisCustomerReference,
                        scheme.Organisation.OrganisationName,
                        address);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "An error occurred creating an 1B1S customer to represent the scheme with ID \"{0}\". " +
                        "See the inner exception for more details.",
                        scheme.Id);
                    throw new Exception(errorMessage, ex);
                }

                customerFile.AddCustomer(customer);
            }

            return Task.FromResult(customerFile);
        }

        public string GetIbisPostCode(Domain.Organisation.Address address)
        {
            string postCode = null;

            if (!string.IsNullOrEmpty(address.Postcode))
            {
                postCode = address.IsUkAddress() ? address.Postcode : string.Format("{0}  {1}", address.Postcode.Trim(), address.Country.Name);
            }

            return postCode;
        }
    }
}
