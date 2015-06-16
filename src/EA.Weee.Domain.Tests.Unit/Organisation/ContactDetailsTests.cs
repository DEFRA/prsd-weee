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
            organisation.AddOrganisationContactDetails(contactDetails);
            Assert.Throws<InvalidOperationException>(() => organisation.AddOrganisationContactDetails(contactDetails));
        }

        private const string smallStringValue = "abcdefghijk";   // More than 10
        private const string mediumStringValue = "abcdefghijklmnopqrstuv";   // More than 20
        private const string longStringValue = "abcdefghijklmnopqrstuvwxyzabcdefghijo";  // More than 35
        [Theory]
        [InlineData(longStringValue, "address2", "townOrCity", "countyOrRegion", "postalCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", longStringValue, "townOrCity", "countyOrRegion", "postalCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", longStringValue, "countyOrRegion", "postalCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", longStringValue, "postalCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", smallStringValue, "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postalCode", longStringValue, "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postalCode", "country", mediumStringValue, "email@abc.com")]
        public void SetFieldValueforContactDetails_ExceedsMaxCharacters_ThrowException(string address1, string address2, string townOrCity, string countyOrRegion, string postalCode, string country, string telephone, string email)
        {
            Action createContactDetails = () => new Address(address1, address2, townOrCity, countyOrRegion, postalCode, country, telephone, email);
            Assert.Throws<InvalidOperationException>(createContactDetails);
        }

        [Theory]
        [InlineData(null, "address2", "townOrCity", "countyOrRegion", "postalCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", null, "countyOrRegion", "postalCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postalCode", null, "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postalCode", "country", null, "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postalCode", "country", "telephone", null)]
        public void SetFieldValueforContactDetails_ToNull_ThrowException(string address1, string address2, string townOrCity, string countyOrRegion, string postalCode, string country, string telephone, string email)
        {
            Action createContactDetails = () => new Address(address1, address2, townOrCity, countyOrRegion, postalCode, country, telephone, email);
            Assert.Throws<ArgumentNullException>(createContactDetails);
        }
        
        private static Address GetTestContactDetails()
        {
            return new Address("address1", "address2", "townOrCity", "countyOrRegion", "postalCode", "country", "telephone", "email@abc.com");
        }

        private static Organisation GetTestOrganisation()
        {
            var organisation = Organisation.CreateRegisteredCompany("Test", "AB123456");
            return organisation;
        }
    }
}
