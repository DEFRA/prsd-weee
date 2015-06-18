namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using Xunit;
    using Organisation = Domain.Organisation;

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
        public void AddAddressToOrganisation_AddressAlreadyExists_InvalidOperationShouldBeThrown(string addressType, string organisationType)
        {
            var organisation = ValidOrganisation(CastOrganisationType(organisationType));
            organisation.AddAddress(CastAddressType(addressType), ValidAddress());

            // Already added address, should fail if we try again
            Assert.Throws<InvalidOperationException>(() => organisation.AddAddress(CastAddressType(addressType), ValidAddress())); 
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

            Assert.Throws<ArgumentNullException>(() => organisation.AddAddress(CastAddressType(addressType), null));
        }

        [Theory]
        [InlineData("Sole trader or individual")]
        [InlineData("Partnership")]
        [InlineData("Registered Company")]
        public void AddAddressToOrganisation_AddressTypeIsNull_ArgumentNullExceptionShouldBeThrown(string organisationType)
        {
            var organisation = ValidOrganisation(CastOrganisationType(organisationType));

            Assert.Throws<ArgumentNullException>(() => organisation.AddAddress(null, ValidAddress()));
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

        private Address ValidAddress()
        {
            return new Address("Address Line 1", "Address Line 1", "Town Or City", "County Or Region", "Postcode",
                "Country", "01234567890", "email@email.email");
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
