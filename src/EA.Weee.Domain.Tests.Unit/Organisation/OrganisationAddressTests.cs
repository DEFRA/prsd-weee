namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using Xunit;

    public class OrganisationAddressTests
    {
        [Fact]
        public void AddOrganisationAddress_OrganisationAlreadyHasOrganisationAddress_Throws()
        {
            var organisation = GetTestOrganisation();
            var address = GetTestAddress();
            organisation.AddAddress(AddressType.OrganisationAddress, address);
            Assert.Throws<InvalidOperationException>(() => organisation.AddAddress(AddressType.OrganisationAddress, address));
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
