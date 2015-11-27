namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using Domain.Organisation;
    using EA.Weee.Domain.Tests.Unit.Helpers;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class OrganisationAddressTests
    {
        [Theory]
        [InlineData("Organisation Address", "Sole trader or individual")]
        [InlineData("Organisation Address", "Partnership")]
        [InlineData("Organisation Address", "Registered Company")]
        [InlineData("Registered or PPB address", "Sole trader or individual")]
        [InlineData("Registered or PPB address", "Partnership")]
        [InlineData("Registered or PPB address", "Registered Company")]
        [InlineData("Service of notice address", "Sole trader or individual")]
        [InlineData("Service of notice address", "Partnership")]
        [InlineData("Service of notice address", "Registered Company")]
        public void AddAddressToOrganisation_AddressAlreadyExists_UpdateAddressDetails(string addressType, string organisationType)
        {
            var organisation = ValidOrganisation(CastOrganisationType(organisationType));
            var validAddress = ValidAddress(addressType);
            var type = CastAddressType(addressType);
            organisation.AddOrUpdateAddress(type, validAddress);
            
            if (type == AddressType.OrganisationAddress)
            {
                Assert.Equal(validAddress, organisation.OrganisationAddress);
            }
            else if (type == AddressType.RegisteredOrPPBAddress)
            {
                Assert.Equal(validAddress, organisation.BusinessAddress);
            }
            else if (type == AddressType.ServiceOfNoticeAddress)
            {
                Assert.Equal(validAddress, organisation.NotificationAddress);
            }
        }

        [Theory]
        [InlineData("Organisation Address", "Sole trader or individual")]
        [InlineData("Organisation Address", "Partnership")]
        [InlineData("Organisation Address", "Registered Company")]
        [InlineData("Registered or PPB address", "Sole trader or individual")]
        [InlineData("Registered or PPB address", "Partnership")]
        [InlineData("Registered or PPB address", "Registered Company")]
        [InlineData("Service of notice address", "Sole trader or individual")]
        [InlineData("Service of notice address", "Partnership")]
        [InlineData("Service of notice address", "Registered Company")]
        public void AddAddressToOrganisation_AddressIsNull_ArgumentNullExceptionShouldBeThrown(string addressType, string organisationType)
        {
            var organisation = ValidOrganisation(CastOrganisationType(organisationType));

            Assert.Throws<ArgumentNullException>(() => organisation.AddOrUpdateAddress(CastAddressType(addressType), null));
        }

        [Theory]
        [InlineData("Sole trader or individual")]
        [InlineData("Partnership")]
        [InlineData("Registered Company")]
        public void AddAddressToOrganisation_AddressTypeIsNull_ArgumentNullExceptionShouldBeThrown(string organisationType)
        {
            var organisation = ValidOrganisation(CastOrganisationType(organisationType));
            Assert.Throws<ArgumentNullException>(() => organisation.AddOrUpdateAddress(null, ValidAddress(null)));
        }

        private Organisation ValidOrganisation(OrganisationType organisationType)
        {
            if (organisationType == OrganisationType.SoleTraderOrIndividual)
            {
                return Organisation.CreateSoleTrader("Trading Name");
            }

            if (organisationType == OrganisationType.Partnership)
            {
                return Organisation.CreatePartnership("Trading Name");
            }

            return Organisation.CreateRegisteredCompany("Company Name", "AB123456", "Trading Name");
        }

        private Address ValidAddress(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Equals("Registered or PPB address"))
                {
                    return new Address("Address Line 1", "Address Line 1", "Town Or City", "County Or Region",
                        "Postcode",
                        GetTestCountry(new Guid(), "UK-England"), "01234567890", "email@email.email");
                }
            }
            return new Address("Address Line 1", "Address Line 1", "Town Or City", "County Or Region", "Postcode",
                GetTestCountry(new Guid(), "France"), "01234567890", "email@email.email");
        }

        private Country GetTestCountry(Guid id, string name)
        {
            var country = ObjectInstantiator<Country>.CreateNew();
            ObjectInstantiator<Country>.SetProperty(x => x.Id, id, country);
            ObjectInstantiator<Country>.SetProperty(x => x.Name, name, country);
            return country;
        }

        private AddressType CastAddressType(string addressType)
        {
            switch (addressType)
            {
                case "Organisation Address":
                    return AddressType.OrganisationAddress;
                case "Registered or PPB address":
                    return AddressType.RegisteredOrPPBAddress;
                default:
                    return AddressType.ServiceOfNoticeAddress;
            }
        }

        private OrganisationType CastOrganisationType(string organisationType)
        {
            switch (organisationType)
            {
                case "Sole trader or individual":
                    return OrganisationType.SoleTraderOrIndividual;
                case "Partnership":
                    return OrganisationType.Partnership;
                default:
                    return OrganisationType.RegisteredCompany;
            }
        }
    }
}
