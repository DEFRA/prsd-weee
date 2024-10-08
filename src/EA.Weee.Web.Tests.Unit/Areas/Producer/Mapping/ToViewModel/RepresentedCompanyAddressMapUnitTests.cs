namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FluentAssertions;
    using System;
    using Xunit;

    public class RepresentedCompanyAddressMapTests
    {
        private readonly IFixture fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
        private readonly RepresentedCompanyAddressMap map = new RepresentedCompanyAddressMap();

        [Fact]
        public void Map_ShouldMapAllProperties()
        {
            // Arrange
            var source = fixture.Create<AuthorisedRepresentitiveData>();

            // Act
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(source.Address1);
            result.Address2.Should().Be(source.Address2);
            result.CountryId.Should().Be(source.CountryId);
            result.CountyOrRegion.Should().Be(source.CountyOrRegion);
            result.Email.Should().Be(source.Email);
            result.Postcode.Should().Be(source.Postcode);
            result.Telephone.Should().Be(source.Telephone);
            result.TownOrCity.Should().Be(source.TownOrCity);
            result.CountryName.Should().Be(source.CountryName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Map_WithNullOrEmptyProperties_ShouldMapCorrectly(string emptyValue)
        {
            // Arrange
            var source = new AuthorisedRepresentitiveData
            {
                Address1 = emptyValue,
                Address2 = emptyValue,
                CountryId = Guid.Empty,
                CountyOrRegion = emptyValue,
                Email = emptyValue,
                Postcode = emptyValue,
                Telephone = emptyValue,
                TownOrCity = emptyValue,
                CountryName = emptyValue
            };

            // Act
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Address1.Should().Be(emptyValue);
            result.Address2.Should().Be(emptyValue);
            result.CountryId.Should().Be(Guid.Empty);
            result.CountyOrRegion.Should().Be(emptyValue);
            result.Email.Should().Be(emptyValue);
            result.Postcode.Should().Be(emptyValue);
            result.Telephone.Should().Be(emptyValue);
            result.TownOrCity.Should().Be(emptyValue);
            result.CountryName.Should().Be(emptyValue);
        }
    }
}