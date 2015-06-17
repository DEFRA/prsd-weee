namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using System.Net.Mail;
    using Xunit;
    using Organisation = Domain.Organisation;

    public class ContactDetailsTests
    {
        [Fact]
        public void AddContactDetails_OrganisationAlreadyHasContactDetails_Throws()
        {
            var organisation = GetTestOrganisation();
            var contactDetails = GetTestContactDetails();
            organisation.AddAddress(AddressType.OrganisationAddress, contactDetails);
            Assert.Throws<InvalidOperationException>(() => organisation.AddAddress(AddressType.OrganisationAddress, contactDetails));
        }

        private const string smallStringValue = "abcdefghijk";   // More than 10
        private const string mediumStringValue = "abcdefghijklmnopqrstuv";   // More than 20
        private const string longStringValue = "abcdefghijklmnopqrstuvwxyzabcdefghijo";  // More than 35
        [Theory]
        [InlineData(longStringValue, "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", longStringValue, "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", longStringValue, "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", longStringValue, "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", smallStringValue, "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", longStringValue, "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", mediumStringValue, "email@abc.com")]
        public void SetFieldValueforContactDetails_ExceedsMaxCharacters_ThrowException(string address1, string address2, string townOrCity, string countyOrRegion, string postCode, string country, string telephone, string email)
        {
            Action createContactDetails = () => new Address(address1, address2, townOrCity, countyOrRegion, postCode, country, telephone, email);
            Assert.Throws<InvalidOperationException>(createContactDetails);
        }

        [Theory]
        [InlineData(null, "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", null, "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", null, "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", null, "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", null)]
        public void SetFieldValueforContactDetails_ToNull_ThrowException(string address1, string address2, string townOrCity, string countyOrRegion, string postCode, string country, string telephone, string email)
        {
            Action createContactDetails = () => new Address(address1, address2, townOrCity, countyOrRegion, postCode, country, telephone, email);
            Assert.Throws<ArgumentNullException>(createContactDetails);
        }
        
        private static Address GetTestContactDetails()
        {
            return new Address("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com");
        }

        private static Organisation GetTestOrganisation()
        {
            var organisation = Organisation.CreateRegisteredCompany("Test", "AB123456");
            return organisation;
        }
    }
}
