namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Tests.Core;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AuthorisedRepresentitiveDataMapTests : SimpleUnitTestBase
    {
        private readonly AuthorisedRepresentitiveDataMap map = new AuthorisedRepresentitiveDataMap();

        [Fact]
        public void Map_WithValidAuthorisedRepresentative_ShouldMapCorrectly()
        {
            // Arrange
            var source = CreateValidAuthorisedRepresentative();

            // Act
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.CompanyName.Should().Be(source.OverseasProducerName);
            result.BusinessTradingName.Should().Be(source.OverseasProducerTradingName);
            result.Address1.Should().Be(source.OverseasContact.Address.PrimaryName);
            result.Address2.Should().Be(source.OverseasContact.Address.Street);
            result.CountryId.Should().Be(source.OverseasContact.Address.CountryId);
            result.CountyOrRegion.Should().Be(source.OverseasContact.Address.AdministrativeArea);
            result.TownOrCity.Should().Be(source.OverseasContact.Address.Town);
            result.Email.Should().Be(source.OverseasContact.Email);
            result.Telephone.Should().Be(source.OverseasContact.Telephone);
            result.Postcode.Should().Be(source.OverseasContact.Address.PostCode);
        }

        [Fact]
        public void Map_WithNullSource_ShouldThrowArgumentNullException()
        {
            // Arrange
            AuthorisedRepresentative source = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => map.Map(source));
        }

        private AuthorisedRepresentative CreateValidAuthorisedRepresentative()
        {
            return TestFixture.Create<AuthorisedRepresentative>();
        }
    }
}