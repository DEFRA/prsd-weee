namespace EA.Weee.Requests.Tests.Unit.Helpers
{
    using System;
    using EA.Weee.Domain;

    internal class OrganisationHelper
    {
        internal Organisation GetOrganisationWithName(string name)
        {
            return GetOrganisationWithDetails(name, null, "1234567", OrganisationType.RegisteredCompany, OrganisationStatus.Approved);
        }

        internal Organisation GetOrganisationWithDetails(string name, string tradingName, string companyRegistrationNumber, Domain.OrganisationType type, OrganisationStatus status)
        {
            Organisation organisation;

            if (type == Domain.OrganisationType.RegisteredCompany)
            {
                organisation = Organisation.CreateRegisteredCompany(name, companyRegistrationNumber, tradingName);
            }
            else if (type == Domain.OrganisationType.Partnership)
            {
                organisation = Organisation.CreatePartnership(tradingName);
            }
            else
            {
                organisation = Organisation.CreateSoleTrader(tradingName);
            }

            organisation.AddAddress(AddressType.OrganisationAddress, GetAddress());

            organisation.AddMainContactPerson(GetContact());

            if (status == OrganisationStatus.Pending)
            {
                organisation.CompleteRegistration();
            }

            if (status == OrganisationStatus.Approved)
            {
                organisation.CompleteRegistration();
                organisation.ToApproved();
            }

            var properties = typeof(Organisation).GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                {
                    var baseProperty = typeof(Organisation).BaseType.GetProperty(propertyInfo.Name);

                    baseProperty.SetValue(organisation, Guid.NewGuid(), null);

                    break;
                }
            }

            return organisation;
        }

        private static Country MakeCountry()
        {
            return new Country(new Guid(), "Country");
        }

        private Address GetAddress()
        {
            return new Address("1", "street", "Woking", "Hampshire", "GU21 5EE", MakeCountry(), "12345678", "test@co.uk");
        }

        private Contact GetContact()
        {
            return new Contact("Test first name", "Test last name", "Test position");
        }
    }
}
