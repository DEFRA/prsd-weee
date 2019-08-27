﻿namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using Domain.Charges;
    using Errors;
    using Ibis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Scheme = EA.Weee.Domain.Scheme.Scheme;

    /// <summary>
    /// This 1B1S customer file generator creates a customer file with one customer for each distinct scheme
    /// referenced by the list of member uploads.
    /// </summary>
    public class BySchemeCustomerFileGenerator : IIbisCustomerFileGenerator
    {
        public Task<IbisFileGeneratorResult<CustomerFile>> CreateAsync(ulong fileID, InvoiceRun invoiceRun)
        {
            CustomerFile customerFile = new CustomerFile("WEE", fileID);

            var errors = new List<Exception>();

            IEnumerable<Scheme> schemes = invoiceRun.MemberUploads
                .Select(mu => mu.Scheme)
                .Distinct();

            foreach (Scheme scheme in schemes)
            {
                try
                {
                    var postCode = GetIbisPostCode(scheme.Address);

                    Address address = new Address(
                        scheme.Contact.FullName,
                        scheme.Address.Address1,
                        scheme.Address.Address2,
                        null,
                        scheme.Address.TownOrCity,
                        scheme.Address.CountyOrRegion,
                        postCode);

                    Customer customer = new Customer(
                            scheme.IbisCustomerReference,
                            scheme.Organisation.OrganisationName,
                            address);

                    customerFile.AddCustomer(customer);
                }
                catch (Exception ex)
                {
                    errors.Add(new SchemeFieldException(scheme, ex));
                }
            }

            var ibisFileGeneratorResult = new IbisFileGeneratorResult<CustomerFile>(errors.Count == 0 ? customerFile : null, errors);
            return Task.FromResult(ibisFileGeneratorResult);
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
