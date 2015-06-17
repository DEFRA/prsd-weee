namespace EA.Weee.Domain.Tests.Unit
{
    using System;
    using Xunit;
    using Organisation = Domain.Organisation;

    public class AddressTests
    {
        private const string LongPostcodeString = "abcdefghijk";   // More than 10
        private const string LongTelephoneString = "abcdefghijklmnopqrstuv";   // More than 20
        private const string LongAddressfieldString = "abcdefghijklmnopqrstuvwxyzabcdefghijo";  // More than 35
        [Theory]
        [InlineData(LongAddressfieldString, "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", LongAddressfieldString, "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", LongAddressfieldString, "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", LongAddressfieldString, "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", LongPostcodeString, "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", LongAddressfieldString, "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", LongTelephoneString, "email@abc.com")]
        public void SetFieldValueforAddress_ExceedsMaxCharacters_ThrowException(string address1, string address2, string townOrCity, string countyOrRegion, string postcode, string country, string telephone, string email)
        {
            Action createAddress = () => new Address(address1, address2, townOrCity, countyOrRegion, postcode, country, telephone, email);
            Assert.Throws<InvalidOperationException>(createAddress);
        }

        [Theory]
        [InlineData(null, "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", null, "countyOrRegion", "postCode", "country", "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", null, "telephone", "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", null, "email@abc.com")]
        [InlineData("address1", "address2", "townOrCity", "countyOrRegion", "postCode", "country", "telephone", null)]
        public void SetFieldValueforAddress_ToNull_ThrowException(string address1, string address2, string townOrCity, string countyOrRegion, string postcode, string country, string telephone, string email)
        {
            Action createAddress = () => new Address(address1, address2, townOrCity, countyOrRegion, postcode, country, telephone, email);
            Assert.Throws<ArgumentNullException>(createAddress);
        }
    }
}
