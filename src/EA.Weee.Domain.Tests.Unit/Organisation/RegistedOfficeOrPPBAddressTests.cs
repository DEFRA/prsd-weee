namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using Xunit;

    public class RegisteredOfficeOrPPBAddressTests
    {
        [Fact]
        public void AddRegisteredOfficeOrPPBAddress_OrganisationAlreadyHasRegisteredOfficeOrPPBAddress_Throws()
        {
            var organisation = GetTestOrganisation();
            var address = GetTestAddress();
            organisation.AddAddress(AddressType.RegisteredOrPPBAddress, address);
            Assert.Throws<InvalidOperationException>(() => organisation.AddAddress(AddressType.RegisteredOrPPBAddress, address));
        }

        private static Domain.Organisation GetTestOrganisation()
        {
            var organisation = Domain.Organisation.CreateRegisteredCompany("Test", "AB123456");
            return organisation;
        }

        private static Address GetTestAddress()
        {
            return new Address("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com");
        }
    }
}
