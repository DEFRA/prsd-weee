namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfAddressTests
    {
        [Fact]
        public void AatfAddress_GivenNameIsGreaterThan256Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(new string('*', 257), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenAddress1IsGreaterThan60Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), new string('*', 61), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenAddress2IsGreaterThan60Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), new string('*', 61), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenTownOrCityIsGreaterThan35Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 36), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenCountyOrRegionIsGreaterThan35Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 36), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenPostcodeIsGreaterThan10Characters_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), new string('*', 11), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GiveCountryIsNull_ThrowsArgumentException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), null);
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("", "-", "-")]
        [InlineData(null, "-", "-")]

        [InlineData("-", "", "-")]
        [InlineData("-", null, "-")]

        [InlineData("-", "-", "")]
        [InlineData("-", "-", null)]
        public void AatfContact_GivenNullOrEmptyRequiredParameters_ThrowsArgumentException(string name, string address, string town)
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(name, address, A.Dummy<string>(), town, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AatfAddress_GivenValidParameters_AatfAddressPropertiesShouldBeSet()
        {
            var name = "Name";
            var address1 = "Address1";
            var address2 = "Address2";
            var town = "Town";
            var county = "County";
            var postcode = "Postcode";
            var country = new Country(Guid.NewGuid(), "Country");

            var contact = new AatfAddress(name, address1, address2, town, county, postcode, country);

            contact.Name.Should().Be(name);
            contact.Address1.Should().Be(address1);
            contact.Address2.Should().Be(address2);
            contact.TownOrCity.Should().Be(town);
            contact.CountyOrRegion.Should().Be(county);
            contact.Postcode.Should().Be(postcode);
            contact.Country.Should().Be(country);
        }
    }
}
