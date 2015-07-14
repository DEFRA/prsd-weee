namespace EA.Weee.Domain.Tests.Unit
{
    using System;
    using Helpers;
    using Xunit;
    using Address = Domain.Organisation.Address;

    /// <summary>
    /// Throughout this class "NR" = not required, "R" = required
    /// </summary>
    public class AddressTests
    {
        private Country GetTestCountry(Guid id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var country = ObjectInstantiator<Country>.CreateNew();
            ObjectInstantiator<Country>.SetProperty(x => x.Id, id, country);
            ObjectInstantiator<Country>.SetProperty(x => x.Name, name, country);
            return country;
        }

        [Fact]
        public void IsUkAddress()
        {
            var country = GetTestCountry(new Guid(), "UK - England");
            var addressUk = new Address("Address Line 1", "Address Line 1", "Town Or City", "County Or Region", "Postcode", country, "01234567890", "email@email.email");
       
            Assert.True(addressUk.IsUkAddress());
        }

        [Fact]
        public void IsNotUkAddress()
        {
            var country = GetTestCountry(new Guid(), "France");
            var addressNonUk = new Address("Address Line 1", "Address Line 1", "Town Or City", "County Or Region", "Postcode", country, "01234567890", "email@email.email");
            Assert.False(addressNonUk.IsUkAddress());
        }

        [Theory]
        [InlineData(null, "NR", "R", "NR", "NR", "R", "R", "R")]
        [InlineData("R", "NR", null, "NR", "NR", "R", "R", "R")]
        [InlineData("R", "NR", "R", "NR", "NR", null, "R", "R")]
        [InlineData("R", "NR", "R", "NR", "NR", "R", null, "R")]
        [InlineData("R", "NR", "R", "NR", "NR", "R", "R", null)]
        [InlineData("", "NR", "R", "NR", "NR", "R", "R", "R")]
        [InlineData("R", "NR", "", "NR", "NR", "R", "R", "R")]
        [InlineData("R", "NR", "R", "NR", "NR", "", "R", "R")]
        [InlineData("R", "NR", "R", "NR", "NR", "R", "", "R")]
        [InlineData("R", "NR", "R", "NR", "NR", "R", "R", "")]
        public void CreateAddress_RequiredPropertyIsNullOrEmpty_ShouldThrowArgumentException(string address1, string address2,
            string townOrCity, string countyOrRegion, string postcode, string country, string telephone, string email)
        {
            var countrydata = GetTestCountry(new Guid(), country);

            Assert.ThrowsAny<ArgumentException>(
                () =>
                    new Address(address1, address2, townOrCity, countyOrRegion, postcode, countrydata, telephone,
                        email));
        }

        [Fact]
        public void CreateAddress_AddressLine1Is36Characters_ShouldThrowInvalidOperationException()
        {
            var countrydata = GetTestCountry(new Guid(), "R");
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address(CharacterString(36), "NR", "R", "NR", "NR", countrydata, "R", "R"));
        }

        [Fact]
        public void CreateAddress_AddressLine2Is36Characters_ShouldThrowInvalidOperationException()
        {
            var countrydata = GetTestCountry(new Guid(), "R");
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", CharacterString(36), "R", "NR", "NR", countrydata, "R", "R"));
        }

        [Fact]
        public void CreateAddress_TownOrCityIs36Characters_ShouldThrowInvalidOperationException()
        {
            var countrydata = GetTestCountry(new Guid(), "R");
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", CharacterString(36), "NR", "NR", countrydata, "R", "R"));
        }

        [Fact]
        public void CreateAddress_CountyOrRegionIs36Characters_ShouldThrowInvalidOperationException()
        {
            var countrydata = GetTestCountry(new Guid(), "R");
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", CharacterString(36), "NR", countrydata, "R", "R"));
        }

        [Fact]
        public void CreateAddress_PostcodeIs11Characters_ShouldThrowInvalidOperationException()
        {
            var countrydata = GetTestCountry(new Guid(), "R");
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", "NR", CharacterString(11), countrydata, "R", "R"));
        }
       
        [Fact]
        public void CreateAddress_TelephoneIs21Characters_ShouldThrowInvalidOperationException()
        {
            var countrydata = GetTestCountry(new Guid(), "R");
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", "NR", "NR", countrydata, CharacterString(21), "R"));
        }

        [Fact]
        public void CreateAddress_EmailIs257Characters_ShouldThrowInvalidOperationException()
        {
            var countrydata = GetTestCountry(new Guid(), "R");
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", "NR", "NR", countrydata, "R", CharacterString(257)));
        }

        private string CharacterString(int length)
        {
            var characters = string.Empty;
            for (int i = 0; i < length; i++)
            {
                characters += "a";
            }

            return characters;
        }
    }
}
