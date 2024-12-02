namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class PublicOrganisationMapTests
    {
        private readonly IMap<Address, AddressData> addressMap;
        private readonly PublicOrganisationMap mapper;

        public PublicOrganisationMapTests()
        {
            addressMap = A.Fake<IMap<Address, AddressData>>();
            mapper = new PublicOrganisationMap(addressMap);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_ShouldMapNpwdMigrationStatus(bool npwdMigratedComplete)
        {
            // Arrange
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.NpwdMigratedComplete).Returns(npwdMigratedComplete);

            // Act
            var result = mapper.Map(organisation);

            // Assert
            result.Should().NotBeNull();
            result.NpwdMigratedComplete.Should().Be(npwdMigratedComplete);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_ShouldMapNpwdMigrationCompleteStatus(bool npwdMigratedComplete)
        {
            // Arrange
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.NpwdMigratedComplete).Returns(npwdMigratedComplete);

            // Act
            var result = mapper.Map(organisation);

            // Assert
            result.Should().NotBeNull();
            result.NpwdMigratedComplete.Should().Be(npwdMigratedComplete);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_ShouldMapNpwdMigrated(bool npwdMigrated)
        {
            // Arrange
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.NpwdMigrated).Returns(npwdMigrated);

            // Act
            var result = mapper.Map(organisation);

            // Assert
            result.Should().NotBeNull();
            result.NpwdMigrated.Should().Be(npwdMigrated);
        }
    }
}
