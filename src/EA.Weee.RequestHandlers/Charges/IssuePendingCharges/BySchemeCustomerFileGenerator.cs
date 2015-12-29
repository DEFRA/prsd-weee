namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Ibis;
    using MemberUpload = EA.Weee.Domain.Scheme.MemberUpload;
    using Scheme = EA.Weee.Domain.Scheme.Scheme;

    /// <summary>
    /// This 1B1S customer file generator creates a customer file with one customer for each distinct scheme
    /// referenced by the list of member uploads.
    /// </summary>
    public class BySchemeCustomerFileGenerator : IIbisCustomerFileGenerator
    {
        public CustomerFile CreateCustomerFile(ulong fileID, IReadOnlyList<MemberUpload> memberUploads)
        {
            CustomerFile customerFile = new CustomerFile("WEE", fileID);

            IEnumerable<Scheme> schemes = memberUploads
                .Select(mu => mu.Scheme)
                .Distinct();

            foreach (Scheme scheme in schemes)
            {
                Address address;
                try
                {
                    address = new Address(
                        scheme.Organisation.Contact.FullName,
                        scheme.Organisation.OrganisationAddress.Address1,
                        scheme.Organisation.OrganisationAddress.Address2,
                        null,
                        scheme.Organisation.OrganisationAddress.TownOrCity,
                        scheme.Organisation.OrganisationAddress.CountyOrRegion,
                        scheme.Organisation.OrganisationAddress.Postcode);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "An error occured creating an 1B1S address to represent the scheme with ID \"{0}\". " +
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
                        "An error occured creating an 1B1S customer to represent the scheme with ID \"{0}\". " +
                        "See the inner exception for more details.",
                        scheme.Id);
                    throw new Exception(errorMessage, ex);
                }

                customerFile.AddCustomer(customer);
            }

            return customerFile;
        }
    }
}
