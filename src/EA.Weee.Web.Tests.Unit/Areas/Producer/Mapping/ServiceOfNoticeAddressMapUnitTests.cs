namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings;
    using FluentAssertions;
    using System;
    using Xunit;

    public class ServiceOfNoticeAddressMapTests
    {
        private readonly ServiceOfNoticeAddressMap addressMap = new ServiceOfNoticeAddressMap();

        [Fact]
        public void Map_WithValidServiceOfNoticesAddressData_ReturnsCorrectAddressData()
        {
            // Arrange
            var serviceOfNoticeAddress = new ServiceOfNoticeAddressData
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
            var result = addressMap.Map(serviceOfNoticeAddress);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(serviceOfNoticeAddress.Address1);
            result.Address2.Should().Be(serviceOfNoticeAddress.Address2);
            result.CountryId.Should().Be(serviceOfNoticeAddress.CountryId);
            result.CountyOrRegion.Should().Be(serviceOfNoticeAddress.CountyOrRegion);
            result.TownOrCity.Should().Be(serviceOfNoticeAddress.TownOrCity);
            result.Postcode.Should().Be(serviceOfNoticeAddress.Postcode);
            result.Telephone.Should().Be(serviceOfNoticeAddress.Telephone);
        }

        [Fact]
        public void Map_WithPartiallyFilledServiceOfNoticesAddressData_ReturnsPartiallyFilledAddressData()
        {
            // Arrange
            var partialServiceOfNoticeAddress = new ServiceOfNoticeAddressData
            {
                Address1 = "456 Street",
                CountryId = Guid.NewGuid(),
                TownOrCity = "City"
            };

            // Act
            var result = addressMap.Map(partialServiceOfNoticeAddress);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(partialServiceOfNoticeAddress.Address1);
            result.CountryId.Should().Be(partialServiceOfNoticeAddress.CountryId);
            result.TownOrCity.Should().Be(partialServiceOfNoticeAddress.TownOrCity);

            result.Address2.Should().BeNull();
            result.CountyOrRegion.Should().BeNull();
            result.WebAddress.Should().BeNull();
            result.Postcode.Should().BeNull();
        }
    }
}