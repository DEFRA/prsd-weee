namespace EA.Weee.Domain.Tests.Unit
{
    using System;
    using Xunit;
    using Address = Domain.Address;

    /// <summary>
    /// Throughout this class "NR" = not required, "R" = required
    /// </summary>
    public class AddressTests
    {
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
            Assert.ThrowsAny<ArgumentException>(
                () => new Address(address1, address2, townOrCity, countyOrRegion, postcode, country, telephone, email));
        }

        [Fact]
        public void CreateAddress_AddressLine1Is36Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address(CharacterString(36), "NR", "R", "NR", "NR", "R", "R", "R"));
        }

        [Fact]
        public void CreateAddress_AddressLine2Is36Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", CharacterString(36), "R", "NR", "NR", "R", "R", "R"));
        }

        [Fact]
        public void CreateAddress_TownOrCityIs36Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", CharacterString(36), "NR", "NR", "R", "R", "R"));
        }

        [Fact]
        public void CreateAddress_CountyOrRegionIs36Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", CharacterString(36), "NR", "R", "R", "R"));
        }

        [Fact]
        public void CreateAddress_PostcodeIs11Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", "NR", CharacterString(11), "R", "R", "R"));
        }

        [Fact]
        public void CreateAddress_CountryIs36Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", "NR", "NR", CharacterString(36), "R", "R"));
        }

        [Fact]
        public void CreateAddress_TelephoneIs21Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", "NR", "NR", "R", CharacterString(21), "R"));
        }

        [Fact]
        public void CreateAddress_EmailIs257Characters_ShouldThrowInvalidOperationException()
        {
            Assert.ThrowsAny<InvalidOperationException>(
                () => new Address("R", "R", "R", "NR", "NR", "R", "R", CharacterString(257)));
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
