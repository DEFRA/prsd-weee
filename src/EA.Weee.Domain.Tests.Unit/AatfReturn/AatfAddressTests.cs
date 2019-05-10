namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfAddressTests
    {
        [Theory]
        [InlineData("")]
        public void Aatf_GivenNameIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(value, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenNameIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(null, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GiveAddress1IsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), value, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenAddress1IsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), null, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GiveTownOrCityIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), value, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenTownOrCityIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), null, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>());
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
