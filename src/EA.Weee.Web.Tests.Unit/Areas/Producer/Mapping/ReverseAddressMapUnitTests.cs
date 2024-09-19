namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings;
    using FluentAssertions;
    using System;
    using Xunit;

    public class ReverseAddressMapTests
    {
        private readonly ReverseAddressMap reverseAddressMap = new ReverseAddressMap();

        [Fact]
        public void Map_WithValidAddressData_ReturnsCorrectExternalAddressData()
        {
            // Arrange
            var address = new AddressData
            {
                Address1 = "123 Test Street",
                Address2 = "Apt 4",
                CountryId = Guid.NewGuid(),
                CountryName = Guid.NewGuid().ToString(),
                CountyOrRegion = "Test County",
                TownOrCity = "Test City",
                WebAddress = "www.test.com",
                Postcode = "TE5T 1NG"
            };

            // Act
            var result = reverseAddressMap.Map(address);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(address.Address1);
            result.Address2.Should().Be(address.Address2);
            result.CountryId.Should().Be(address.CountryId);
            result.CountryName.Should().Be(address.CountryName);
            result.CountyOrRegion.Should().Be(address.CountyOrRegion);
            result.TownOrCity.Should().Be(address.TownOrCity);
            result.WebsiteAddress.Should().Be(address.WebAddress);
            result.Postcode.Should().Be(address.Postcode);
        }

        [Fact]
        public void Map_WithPartiallyFilledAddressData_ReturnsPartiallyFilledExternalAddressData()
        {
            // Arrange
            var partialAddress = new AddressData
            {
                Address1 = "456 Street",
                CountryId = Guid.NewGuid(),
                TownOrCity = "City"
            };

            // Act
            var result = reverseAddressMap.Map(partialAddress);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(partialAddress.Address1);
            result.CountryId.Should().Be(partialAddress.CountryId);
            result.TownOrCity.Should().Be(partialAddress.TownOrCity);
            result.Address2.Should().BeNull();
            result.CountyOrRegion.Should().BeNull();
            result.WebsiteAddress.Should().BeNull();
            result.Postcode.Should().BeNull();
        }
    }
}