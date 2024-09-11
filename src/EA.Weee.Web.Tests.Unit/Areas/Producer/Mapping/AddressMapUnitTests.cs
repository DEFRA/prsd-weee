namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Web.Areas.Producer.Mappings;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AddressMapTests
    {
        private readonly AddressMap addressMap = new AddressMap();

        [Fact]
        public void Map_WithValidExternalAddressData_ReturnsCorrectAddressData()
        {
            // Arrange
            var externalAddress = new ExternalAddressData
            {
                Address1 = "123 Test Street",
                Address2 = "Apt 4",
                CountryId = Guid.NewGuid(),
                CountyOrRegion = "Test County",
                TownOrCity = "Test City",
                WebsiteAddress = "www.test.com",
                Postcode = "TE5T 1NG"
            };

            // Act
            var result = addressMap.Map(externalAddress);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(externalAddress.Address1);
            result.Address2.Should().Be(externalAddress.Address2);
            result.CountryId.Should().Be(externalAddress.CountryId);
            result.CountyOrRegion.Should().Be(externalAddress.CountyOrRegion);
            result.TownOrCity.Should().Be(externalAddress.TownOrCity);
            result.WebAddress.Should().Be(externalAddress.WebsiteAddress);
            result.Postcode.Should().Be(externalAddress.Postcode);
        }

        [Fact]
        public void Map_WithPartiallyFilledExternalAddressData_ReturnsPartiallyFilledAddressData()
        {
            // Arrange
            var partialExternalAddress = new ExternalAddressData
            {
                Address1 = "456 Street",
                CountryId = Guid.NewGuid(),
                TownOrCity = "City"
            };

            // Act
            var result = addressMap.Map(partialExternalAddress);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(partialExternalAddress.Address1);
            result.CountryId.Should().Be(partialExternalAddress.CountryId);
            result.TownOrCity.Should().Be(partialExternalAddress.TownOrCity);

            result.Address2.Should().BeNull();
            result.CountyOrRegion.Should().BeNull();
            result.WebAddress.Should().BeNull();
            result.Postcode.Should().BeNull();
        }
    }
}