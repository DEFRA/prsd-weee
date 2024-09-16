namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings;
    using FluentAssertions;
    using System;
    using Xunit;

    public class ReverseServiceOfNoticeAddressMapTests
    {
        private readonly ReverseServiceOfNoticeAddressMap reverseServiceOfNoticeAddressMap = new ReverseServiceOfNoticeAddressMap();

        [Fact]
        public void Map_WithValidAddressData_ReturnsCorrectServiceOfNoticeAddressData()
        {
            // Arrange
            var address = new AddressData
            {
                Address1 = "123 Test Street",
                Address2 = "Apt 4",
                CountryId = Guid.NewGuid(),
                CountyOrRegion = "Test County",
                TownOrCity = "Test City",
                Postcode = "TE5T 1NG",
                Telephone = "12345678912"
            };

            // Act
            var result = reverseServiceOfNoticeAddressMap.Map(address);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(address.Address1);
            result.Address2.Should().Be(address.Address2);
            result.CountryId.Should().Be(address.CountryId);
            result.CountyOrRegion.Should().Be(address.CountyOrRegion);
            result.TownOrCity.Should().Be(address.TownOrCity);
            result.Postcode.Should().Be(address.Postcode);
            result.Telephone.Should().Be(address.Telephone);
        }

        [Fact]
        public void Map_WithPartiallyFilledAddressData_ReturnsPartiallyFilledServiceOfNoticeAddressData()
        {
            // Arrange
            var partialAddress = new AddressData
            {
                Address1 = "456 Street",
                CountryId = Guid.NewGuid(),
                TownOrCity = "City"
            };

            // Act
            var result = reverseServiceOfNoticeAddressMap.Map(partialAddress);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(partialAddress.Address1);
            result.CountryId.Should().Be(partialAddress.CountryId);
            result.TownOrCity.Should().Be(partialAddress.TownOrCity);
            result.Address2.Should().BeNull();
            result.CountyOrRegion.Should().BeNull();
            result.Postcode.Should().BeNull();
            result.Postcode.Should().BeNull();
            result.Telephone.Should().BeNull();
        }
    }
}